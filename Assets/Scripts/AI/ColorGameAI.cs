using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGameAI : AI
{

    private ColorGame game;
    private Character participant;

    public ColorGameAI(ColorGame _game, Character _participant) /*: base(_game,_participant)*/
    {
        game = _game;
        participant = _participant;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Answer()
    {
        // TODO : swtch cases according to strategies
        // Random Answer
        Character partcipantSelected = game.GetRandomParticipant();
        while (partcipantSelected == participant)
        {
            partcipantSelected = game.GetRandomParticipant();
        }
        game.ParticipantAnswer(participant,partcipantSelected);
    }
}
