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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        dialogueBox = Instantiate(dialogueBoxPrefab);
        dialogueBox.SetActive(dialoguing);
        dialogueBox.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        dialogueName = dialogueBox.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[0];
        dialogueText = dialogueBox.gameObject.GetComponentsInChildren<TextMeshProUGUI>()[1];
        avatar = dialogueBox.gameObject.GetComponentInChildren<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateDialogueBox(Character character, string message)
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
    }

    public bool Dialoguing()
    {
        return dialoguing;
    }
}
