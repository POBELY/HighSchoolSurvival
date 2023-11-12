using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class NumberGameAI : AI
{

    // TODO getIt by subGameManage SymbolGameManager
    private NumberGame game;
    private Bot participant;

    private Dictionary<Character, NumberData> answersGived = new Dictionary<Character, NumberData>();
    private Dictionary<Character, NumberData> answersReceived = new Dictionary<Character, NumberData>();

    public NumberGameAI(NumberGame _game, Bot _participant) /*: base(_game,_participant)*/
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
    public /*override*/ NumberData Ask(Character character)
    {
        //TODO : return participant.getData()
        NumberData participantNumber = game.GetData(participant);
        //Debug.Log(character.name + " Ask to " + participant.name);
        //Debug.Log("Bernoulli Law p(" + participant.GetRelation(character) / (double) Character.maxByteValue + ")");

        if (answersGived.ContainsKey(character))
        {
            return answersGived[character];
        }

        NumberData symbolAnswered = participantNumber;
        if (AI.BernoulliLaw(participant.GetRelation(character) / (double) Bot.maxByteValue)) {
            // Return Truth
            symbolAnswered = participantNumber;
        } else
        {
            // Return Lie
            symbolAnswered = game.GetAnotherRandomNumber(participantNumber);
        }
        answersGived[character] = symbolAnswered;
        return symbolAnswered;
    }
    
    // TODO : Use DebugLevel to follow AI behavior
    // TODO : using method : maxConfianceValidAnswer ?
    public override void Choice()
    {

        Dictionary<NumberData, byte> numbersReceived = new Dictionary<NumberData, byte>();
        foreach (NumberData symbol in NumberGame.numbers)
        {
            numbersReceived[symbol] = 0;
        }
        NumberData numberAnswer = game.GetRandomNumber();
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
                            NumberData numberData = character.AskedBy(participant) as NumberData;
                            Assert.IsNotNull(numberData);
                            answersReceived[character] = numberData;
                            numbersReceived[numberData]++;
                        }
                        else if (character.CompareTag("Player"))
                        {
                            if (answersReceived.ContainsKey(character))
                            {
                                numbersReceived[answersReceived[character]]++;
                            }
                        }
                    }
                }

                IEnumerable<KeyValuePair<NumberData, byte>> orderedNumbersReceived = numbersReceived.OrderBy(x => x.Value).Where(x => x.Value != 0);
                KeyValuePair<NumberData, byte> minKpv = orderedNumbersReceived.First();
                KeyValuePair<NumberData, byte> maxKpv = orderedNumbersReceived.Last();
                int val1 = orderedNumbersReceived.ToList()[1].Value - minKpv.Value;
                int val2 = maxKpv.Value - orderedNumbersReceived.ToList()[orderedNumbersReceived.Count() - 2].Value;
                if (val1 > val2)
                {
                    numberAnswer = minKpv.Key;
                } else
                {
                    numberAnswer = maxKpv.Key;
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
                            NumberData numberData = character.AskedBy(participant) as NumberData;
                            Assert.IsNotNull(numberData);
                            answersReceived[character] = numberData;
                        }

                        if (answersReceived.ContainsKey(character))
                        {
                            if (participant.GetRelation(character) > mefiance)
                            {
                                numbersReceived[answersReceived[character]]++;
                            }
                            else
                            {
                                foreach (NumberData number in NumberGame.numbers)
                                {
                                    if (number != answersReceived[character])
                                    {
                                        numbersReceived[number]++;
                                    }
                                }
                            }
                        }
                    }
                }
                // TODO : Duplication with CONFIANCE :/
                IEnumerable<KeyValuePair<NumberData, byte>> orderedNumbersReceivedMef= numbersReceived.OrderBy(x => x.Value);
                KeyValuePair<NumberData, byte> minKpvMef = orderedNumbersReceivedMef.First();
                KeyValuePair<NumberData, byte> maxKpvMef = orderedNumbersReceivedMef.Last();
                int val1Mef = orderedNumbersReceivedMef.ToList()[1].Value - minKpvMef.Value;
                int val2Mef = maxKpvMef.Value - orderedNumbersReceivedMef.ToList()[orderedNumbersReceivedMef.Count() - 2].Value;
                if (val1Mef > val2Mef)
                {
                    numberAnswer = minKpvMef.Key;
                }
                else
                {
                    numberAnswer = maxKpvMef.Key;
                }
                break;
            default:
                Debug.LogError("Strategy not implemented");
                break;
        }

        game.ParticipantAnswer(participant, numberAnswer);
    }

    public void Response(Character character, NumberData number)
    {
        answersReceived[character] = number;
    }

    public override void Clear()
    {
        answersGived.Clear();
        answersReceived.Clear();
    }

    // TODO : How to Generic, Rename method
    public void CheckAnswers(Dictionary<Character, NumberData> goodAnswers)
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
        NumberData number = data as NumberData;
        Assert.IsNotNull(number);
        Response(character, number);
    }

    // TODO : Return answer ?
    public override void GiveAnswer()
    {
        Choice();
    }

    // TODO : Rename method
    public override void CheckAnswers<D>(Dictionary<Character, D> goodAnswers)
    {
        Dictionary<Character, NumberData> goodSymbolsAnswers = new Dictionary<Character, NumberData>();
        foreach (Character character in goodAnswers.Keys)
        {
            NumberData numberData = goodAnswers[character] as NumberData;
            Assert.IsNotNull(numberData);
            goodSymbolsAnswers.Add(character, numberData);
        }
        CheckAnswers(goodSymbolsAnswers);
    }


}
