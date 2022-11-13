using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGameAI : AI
{

    // TODO getIt by subGameManage ColorGameManager
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

    public /*override*/ Color Ask(Character character)
    {
        Color participantColor = game.GetData(participant);
        Debug.Log(character.name + " Ask to " + participant.name);
        Debug.Log("Bernoulli Law p(" + participant.GetRelation(character) / (double) Character.maxByteValue + ")");
        if (AI.BernoulliLaw(participant.GetRelation(character) / (double) Character.maxByteValue)) {
            return participantColor;
            //TODO : return participant.getData()
        } else
        {
            return game.GetAnotherRandomColor(participantColor);
        }
    }

    public override void Choice()
    {

        byte maxConfiance = 0;
        Character answer = game.GetAnotherRandomParticipant(participant);
        Color participantColor = game.GetData(participant);
        switch (participant.GetStrategy())
        {
            case STRATEGY.RANDOM:
                break;
            case STRATEGY.CONFIANCE:
                foreach (Character character in game.GetParticipants())
                {
                    // TODO : Take also potentially player answer in account
                    if (character.CompareTag("Bot") && character != participant)
                    {
                        if (character.Asked(participant) == participantColor)
                        {
                            if (participant.GetRelation(character) > maxConfiance)
                            {
                                maxConfiance = participant.GetRelation(character);
                                answer = character;
                            }
                        }
                    }
                }
                break;
            case STRATEGY.MEFIANCE:
                byte mefiance = (byte) (Character.maxByteValue / game.GetNbColors());
                foreach (Character character in game.GetParticipants())
                {
                    // TODO : Take also potentially player answer in account
                    if (character.CompareTag("Bot") && character != participant)
                    {
                        if (character.Asked(participant) == participantColor)
                        {
                            if (participant.GetRelation(character) > maxConfiance)
                            {
                                maxConfiance = participant.GetRelation(character);
                                answer = character;
                            }
                        }
                    }
                }
                if (maxConfiance < mefiance)
                {
                    byte minConfiance = Character.maxByteValue;
                    foreach (Character character in game.GetParticipants())
                    {
                        // TODO : Take also potentially player answer in account
                        if (character.CompareTag("Bot") && character != participant)
                        {
                            if (character.Asked(participant) != participantColor)
                            {
                                if (participant.GetRelation(character) < minConfiance)
                                {
                                    minConfiance = participant.GetRelation(character);
                                    answer = character;
                                }
                            }
                        }
                    }
                }
                break;
            default:
                Debug.LogError("Strategy not implemented");
                break;
        }
        game.ParticipantAnswer(participant, answer);
    }
}
