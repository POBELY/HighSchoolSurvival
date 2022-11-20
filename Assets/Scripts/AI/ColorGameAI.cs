using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGameAI : AI
{

    // TODO getIt by subGameManage ColorGameManager
    private ColorGame game;
    private Character participant;

    private Dictionary<Character,Color>  answersGived = new Dictionary<Character, Color>();
    private Dictionary<Character, Color> answersReceived = new Dictionary<Character, Color>();

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

    // TODO : How to Generic : DataType Ask(character)
    public /*override*/ Color Ask(Character character)
    {
        //TODO : return participant.getData()
        Color participantColor = game.GetData(participant);
        //Debug.Log(character.name + " Ask to " + participant.name);
        //Debug.Log("Bernoulli Law p(" + participant.GetRelation(character) / (double) Character.maxByteValue + ")");

        if (answersGived.ContainsKey(character))
        {
            return answersGived[character];
        }

        Color colorAnswered = participantColor;
        if (AI.BernoulliLaw(participant.GetRelation(character) / (double) Character.maxByteValue)) {
            // Return Truth
            colorAnswered = participantColor;
        } else
        {
            // Return Lie
            colorAnswered = game.GetAnotherRandomColor(participantColor);
        }
        answersGived[character] = colorAnswered;
        return colorAnswered;
    }
    
    // TODO : Use DebugLevel to follow AI behavior
    // TODO : using method : maxConfianceValidAnswer ?
    public override void Choice()
    {

        byte maxConfiance = 0;
        Character answer = game.GetAnotherRandomParticipant(participant);
        Color participantColor = game.GetData(participant); // TODO : return participant.getData()
        switch (participant.GetStrategy())
        {
            case STRATEGY.RANDOM:
                break;
            case STRATEGY.CONFIANCE:
                foreach (Character character in game.GetParticipants())
                {
                    // TODO : Take also potentially player answer in account
                    if (character != participant)
                    {
                        if (character.CompareTag("Bot"))
                        {
                            answersReceived[character] = character.Asked(participant);
                            if (answersReceived[character] == participantColor)
                            {
                                if (participant.GetRelation(character) > maxConfiance)
                                {
                                    maxConfiance = participant.GetRelation(character);
                                    answer = character;
                                }
                            }
                        }
                        else if (character.CompareTag("Player"))
                        {
                            if (answersReceived.ContainsKey(character) && answersReceived[character] == participantColor)
                            {
                                if (participant.GetRelation(character) > maxConfiance)
                                {
                                    maxConfiance = participant.GetRelation(character);
                                    answer = character;
                                }
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
                    if (character != participant)
                    {
                        if (character.CompareTag("Bot"))
                        {
                            answersReceived[character] = character.Asked(participant);
                            if (answersReceived[character] == participantColor)
                            {
                                if (participant.GetRelation(character) > maxConfiance)
                                {
                                    maxConfiance = participant.GetRelation(character);
                                    answer = character;
                                }
                            }
                        }
                        else if (character.CompareTag("Player"))
                        {
                            if (answersReceived.ContainsKey(character) && answersReceived[character] == participantColor)
                            {
                                if (participant.GetRelation(character) > maxConfiance)
                                {
                                    maxConfiance = participant.GetRelation(character);
                                    answer = character;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("Character Tag not recogised");
                        }
                    }

                }
                if (maxConfiance < mefiance)
                {
                    byte minConfiance = Character.maxByteValue;
                    foreach (Character character in answersReceived.Keys)
                    {

                        if (answersReceived[character] != participantColor)
                        {
                            if (participant.GetRelation(character) < minConfiance)
                            {
                                minConfiance = participant.GetRelation(character);
                                answer = character;
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

    public override void Clear()
    {
        answersGived.Clear();
        answersReceived.Clear();
    }

    // TODO : How to Generic
    public void CheckAnswers(Dictionary<Character, Color> goodAnswers)
    {
        // TODO : Check answerReceived Cleaning
        foreach (Character character in goodAnswers.Keys)
        {
            if (participant != character)
            {
                System.Random rand = new System.Random();
                byte confiance = (byte) (rand.Next(0, confianceDelta) + 1);
                if (answersReceived.ContainsKey(character) && answersReceived[character] == goodAnswers[character])
                {
                    // Increment confiance
                    participant.IncrementRelation(character, confiance);
                }
                else
                {
                    //Decrement confiance
                    participant.DecrementRelation(character, confiance);
                }
            }
        }
    }

}
