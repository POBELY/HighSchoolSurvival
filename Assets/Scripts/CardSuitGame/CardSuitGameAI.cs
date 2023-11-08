using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CardSuitGameAI : AI
{

    // TODO getIt by subGameManage CardSuiGameManager
    private CardSuitGame game;
    private Bot participant;

    private Dictionary<Character, CardSuitData>  answersGived = new Dictionary<Character, CardSuitData>();
    private Dictionary<Character, CardSuitData> answersReceived = new Dictionary<Character, CardSuitData>();

    public CardSuitGameAI(CardSuitGame _game, Bot _participant) /*: base(_game,_participant)*/
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
    public /*override*/ CardSuitData Ask(Character character)
    {
        //TODO : return character.getData()
        CardSuitData characterCard = game.GetData(character);
        //Debug.Log(character.name + " Ask to " + participant.name);
        //Debug.Log("Bernoulli Law p(" + participant.GetRelation(character) / (double) Character.maxByteValue + ")");

        if (answersGived.ContainsKey(character))
        {
            return answersGived[character];
        }

        CardSuitData cardAnswered = characterCard;
        // TODO : Only else statement is necessary
        if (AI.BernoulliLaw(participant.GetRelation(character) / (double) Bot.maxByteValue)) {
            // Return Truth
            cardAnswered = characterCard;
        } else
        {
            // Return Lie
            cardAnswered = game.GetAnotherRandomCardSuit(characterCard);
        }
        answersGived[character] = cardAnswered;
        return cardAnswered;
    }
    
    // TODO : Use DebugLevel to follow AI behavior
    // TODO : using method : maxConfianceValidAnswer ?
    // TODO : Init answerReceived and only use it i this method => no compute it in each case
    public override void Choice()
    {

        byte maxConfiance = 0;
        CardSuitData answer = game.GetRandomCardSuit();
        //CardSuitData participantCard = game.GetData(participant); // TODO : return participant.getData()
        switch (participant.GetStrategy())
        {
            case STRATEGY.RANDOM:
                break;
            case STRATEGY.CONFIANCE:
                // TODO : pondere par la confiance toutes les réponses obtenus != MaxConfiance Strategy
                foreach (Character character in game.GetParticipants())
                {
                    if (character != participant)
                    {
                        if (character.CompareTag("Bot"))
                        {
                            CardSuitData cardData = character.AskedBy(participant) as CardSuitData;
                            Assert.IsNotNull(cardData);
                            answersReceived[character] = cardData;

                            if (participant.GetRelation(character) > maxConfiance)
                            {
                                maxConfiance = participant.GetRelation(character);
                                answer = cardData;
                            }

                        }
                        else if (character.CompareTag("Player"))
                        {
                            if (answersReceived.ContainsKey(character))
                            {
                                if (participant.GetRelation(character) > maxConfiance)
                                {
                                    maxConfiance = participant.GetRelation(character);
                                    answer = answersReceived[character];
                                }
                            }
                        }
                    }
                }
                break;
            case STRATEGY.MEFIANCE:
                byte mefiance = (byte) (Bot.maxByteValue / 2);
                foreach (Character character in game.GetParticipants())
                {
                    if (character != participant)
                    {
                        if (character.CompareTag("Bot"))
                        {
                            CardSuitData cardData = character.AskedBy(participant) as CardSuitData; ;
                            Assert.IsNotNull(cardData);
                            answersReceived[character] = cardData;
                            if (participant.GetRelation(character) > maxConfiance)
                            {
                                maxConfiance = participant.GetRelation(character);
                                answer = cardData;
                            }

                        }
                        else if (character.CompareTag("Player"))
                        {
                            if (answersReceived.ContainsKey(character))
                            {
                                if (participant.GetRelation(character) > maxConfiance)
                                {
                                    maxConfiance = participant.GetRelation(character);
                                    answer = answersReceived[character];
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("Character Tag not recogised");
                        }
                    }

                }
                if (maxConfiance <= mefiance)
                {
                    byte minConfiance = Bot.maxByteValue;
                    foreach (Character character in answersReceived.Keys)
                    {
                        if (participant.GetRelation(character) < minConfiance)
                        {
                            minConfiance = participant.GetRelation(character);
                            answer = game.GetAnotherRandomCardSuit(answersReceived[character]);
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

    public void Response(Character character, CardSuitData card)
    {
        answersReceived[character] = card;
    }

    public override void Clear()
    {
        answersGived.Clear();
        answersReceived.Clear();
    }

    // TODO : How to Generic
    public void CheckAnswers(Dictionary<Character, CardSuitData> goodAnswers)
    {
        Debug.Log("CheckAnswers for CardSuitGameAI");
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
        CardSuitData card = data as CardSuitData;
        Assert.IsNotNull(card);
        Response(character, card);
    }

    // TODO : Return answer ?
    public override void GiveAnswer()
    {
        Choice();
    }

    public override void CheckAnswers<D>(Dictionary<Character, D> goodAnswers)
    {
        Dictionary<Character, CardSuitData> goodCardssAnswers = new Dictionary<Character, CardSuitData>();
        foreach (Character character in goodAnswers.Keys)
        {
            CardSuitData cardData = goodAnswers[character] as CardSuitData;
            Assert.IsNotNull(cardData);
            goodCardssAnswers.Add(character, cardData);
        }
        CheckAnswers(goodCardssAnswers);
    }


}
