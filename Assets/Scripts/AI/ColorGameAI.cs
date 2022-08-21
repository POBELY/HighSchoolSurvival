using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGameAI : AI
{

    private ColorGame game;
    private Character participant;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Character Answer()
    {
        // TODO : swtch cases according to strategies
        // Random Answer
        Character partcipantSelected = game.GetRandomParticipant();
        while (partcipantSelected == participant)
        {
            partcipantSelected = game.GetRandomParticipant();
        }
        return partcipantSelected;


    }
}
