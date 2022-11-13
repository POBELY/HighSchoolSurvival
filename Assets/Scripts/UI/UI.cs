using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI : MonoBehaviour
{


    [SerializeField] private GameObject dialogueBoxPrefab;
    private GameObject dialogueBox;
    private TextMeshProUGUI dialogueName;
    private TextMeshProUGUI dialogueText;
    private RawImage avatar;
    private bool dialoguing = false;

    private List<Message> dialogue = new List<Message>();
    private int dialogueIndex = 0;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        dialogueBox = Instantiate(dialogueBoxPrefab);
        dialogueBox.SetActive(dialoguing);
        dialogueBox.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        dialogueName = dialogueBox.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        dialogueText = dialogueBox.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[1];
        avatar = dialogueBox.gameObject.GetComponentInChildren<RawImage>();

        dialoguing = false;
        dialogueIndex = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (dialoguing && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("UI Update Space Bar");
            ExecuteCurrentMessage();
        }
        
    }

    /*public void CreateDialogueBox(Character character, string message)
    {
        dialoguing = true;
        dialogueBox.SetActive(dialoguing);
        dialogueName.text = character.name;
        dialogueText.text = message;
        avatar.texture = character.GetAvatar();
    }

    public void NextDialogueBox()
    {
        dialoguing = false;
        dialogueBox.SetActive(dialoguing);
    }*/

    // TODO : 2 or 1 method ?
    public void ActivateDialogueBox()
    {
        DialogueBoxActivation(true);
    }

    public void DisactivateDialogueBox()
    {
        DialogueBoxActivation(false);
    }

    public void DialogueBoxActivation(bool activation)
    {
        dialoguing = activation;
        dialogueBox.SetActive(activation);
        dialogueIndex = 0;
    }

    public void UpdateDialogueBox(Message message)
    {
        dialogueName.text = message.GetSpeaker().name;
        dialogueText.text = message.GetMessage();
        avatar.texture = message.GetSpeaker().GetAvatar();
    }

    public void ExecuteCurrentMessage()
    {
        Debug.Log("ExecuteCurrentMessage : " + dialogueIndex);
        if (dialogue.Count > dialogueIndex)
        {
            Debug.Log("ExecuteCurrentMessage Continue");
            dialogue[dialogueIndex].Execute();
            ++dialogueIndex;
        }
        else
        {
            Debug.Log("ExecuteCurrentMessage End");
            DisactivateDialogueBox();
            dialoguing = false;
        }
    }

    public bool IsDialoguing()
    {
        return dialoguing;
    }

    public void SetDialogue(List<Message> dialogue)
    {
        this.dialogue = dialogue;
    }
}
