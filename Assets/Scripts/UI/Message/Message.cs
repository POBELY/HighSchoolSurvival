using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Message {

    protected Character speaker;
    protected string message;

    public Message(Character speaker, string message)
    {
        this.speaker = speaker;
        this.message = message;
    }

    public virtual void Update()
    {

    }

    public virtual void Execute()
    {
        GameManager.Instance.ui.UpdateDialogueBox(this);
    }

    public Character GetSpeaker()
    {
        return speaker;
    }

    public string GetMessage()
    {
        return message;
    }
}
