using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ColorGameAI : AI
{

    // TODO getIt by subGameManage ColorGameManager
    private ColorGame game;
    private Bot participant;

    private Dictionary<Character, ColorData>  answersGived = new Dictionary<Character, ColorData>();
    private Dictionary<Character, ColorData> answersReceived = new Dictionary<Character, ColorData>();

    public ColorGameAI(ColorGame _game, Bot _participant) /*: base(_game,_participant)*/
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
    public /*override*/ ColorData Ask(Character character)
    {
        //TODO : return participant.getData()
        ColorData participantColor = game.GetData(participant);
        //Debug.Log(character.name + " Ask to " + participant.name);
        //Debug.Log("Bernoulli Law p(" + participant.GetRelation(character) / (double) Character.maxByteValue + ")");

        if (answersGived.ContainsKey(character))
        {
            return answersGived[character];
        }

        ColorData colorAnswered = participantColor;
        if (AI.BernoulliLaw(participant.GetRelation(character) / (double) Bot.maxByteValue)) {
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
        ColorData participantColor = game.GetData(participant); // TODO : return participant.getData()
        switch (participant.GetStrategy())
        {
            case STRATEGY.RANDOM:
                break;
            case STRATEGY.CONFIANCE:
                foreach (Character character in game.GetParticipants())
                {
                    if (character != participant)
                    {
                        if (character.CompareTag("Bot"))
                        {
                            ColorData colorData = character.AskedBy(participant) as ColorData;
                            Assert.IsNotNull(colorData);
                            answersReceived[character] = colorData;
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
                byte mefiance = (byte) (Bot.maxByteValue / game.GetNbColors());
                foreach (Character character in game.GetParticipants())
                {
                    if (character != participant)
                    {
                        if (character.CompareTag("Bot"))
                        {
                            ColorData colorData = character.AskedBy(participant) as ColorData;
                            Assert.IsNotNull(colorData);
                            answersReceived[character] = colorData;
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
                    byte minConfiance = Bot.maxByteValue;
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

    public void Response(Character character, ColorData color)
    {
        answersReceived[character] = color;
    }

    public override void Clear()
    {
        answersGived.Clear();
        answersReceived.Clear();
    }

    // TODO : How to Generic
    public void CheckAnswers(Dictionary<Character, ColorData> goodAnswers)
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
                    //Debug.Log("Increment relation between " + participant.name + " and " + character.name + " of " + confiance);
                    participant.IncrementRelation(character, confiance);
                }
                else
                {
                    //Decrement confiance
                    //Debug.Log("Decrement relation between " + participant.name + " and " + character.name + " of " + confiance);
                    participant.DecrementRelation(character, confiance);
                }
            }
        }
    }



    public override Data AskedBy(Character character)
    {
        return Ask(character);
    }

    public override void GetResponse(Character character, Data data)
    {
        ColorData color = data as ColorData;
        Assert.IsNotNull(color);
        Response(character, color);
    }

    // TODO : Return answer ?
    public override void GiveAnswer()
    {
        Choice();
    }

    public override void CheckAnswers<D>(Dictionary<Character, D> goodAnswers)
    {
        Dictionary<Character, ColorData> goodColorsAnswers = new Dictionary<Character, ColorData>();
        foreach (Character character in goodAnswers.Keys)
        {
            ColorData colorData = goodAnswers[character] as ColorData;
            Assert.IsNotNull(colorData);
            goodColorsAnswers.Add(character, colorData);
        }
        CheckAnswers(goodColorsAnswers);
    }


}
