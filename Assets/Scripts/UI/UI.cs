using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assertions;

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
        if (dialoguing)
        {
            UpdateCurrentMessage();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("UI Update Space Bar");
                ReadNextMessage();
            }
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
        ReadFirstMessage();
    }

    public void DisactivateDialogueBox()
    {
        DialogueBoxActivation(false);
        // TODO : Needed ? Clean via ReadNextMessage ? How to do with choice made by ChoiceMessage
        dialogue.Clear();
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

    private void UpdateCurrentMessage()
    {
        Assert.IsTrue(dialogue.Count > dialogueIndex);
        dialogue[dialogueIndex].Update();
    }

    private void ReadFirstMessage()
    {
        Assert.IsTrue(dialogue.Count > 0);
        Debug.Log("ReadFirstMessage : " + dialogueIndex);
        dialogueIndex = 0;
        dialogue[dialogueIndex].Execute();
    }

    private void ReadNextMessage()
    {
        Debug.Log("ReadNextMessage : " + dialogueIndex);
        ++dialogueIndex;
        if (dialogue.Count > dialogueIndex)
        {
            Debug.Log("ReadNextMessage Continue");
            dialogue[dialogueIndex].Execute();
        }
        else
        {
            Debug.Log("ReadNextMessage End");
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
