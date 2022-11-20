using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoicesMessage : Message
{

    protected  bool horizontal = false;
    protected List<string> choices = new List<string>();
    private int choiceIndex = 0;
    private string rawMessage;

    public ChoicesMessage(Character speaker, string message, List<string> choices, bool horizontal = false) : base(speaker, message)
    {
        this.rawMessage = message;
        this.choices = choices;
        this.horizontal = horizontal;
        this.choiceIndex = 0;
        UpdateMessage();
    }

    private void UpdateMessage()
    {
        message = rawMessage + "\n";
        message += "<mspace=5>";

        for (int i = 0; i < choices.Count; ++i)
        {
            message += (i == choiceIndex ? " > " : "   ");
            message += choices[i];
            message += (horizontal ? "   " : "\n");           
        }
        message += "</mspace>";
        Execute();
    }

    public override void Update()
    {
        if (horizontal)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("Input Right Arrow");
                choiceIndex = Mathf.Min(choiceIndex + 1, choices.Count - 1);
                UpdateMessage();
            } else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("Input Left Arrow");
                choiceIndex = Mathf.Max(0, choiceIndex - 1);
                UpdateMessage();
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("Input Down Arrow");
                choiceIndex = Mathf.Min(choiceIndex + 1, choices.Count - 1);
                UpdateMessage();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("Input Up Arrow");
                choiceIndex = Mathf.Max(0, choiceIndex - 1);
                UpdateMessage();
            }
        }
    }

    public string GetChoice()
    {
        return choices[choiceIndex];
    }

}
