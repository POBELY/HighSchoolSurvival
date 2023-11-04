using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class SymbolGameAI : AI
{

    // TODO getIt by subGameManage SymbolGameManager
    private SymbolGame game;
    private Bot participant;

    private Dictionary<Character, SymbolData> answersGived = new Dictionary<Character, SymbolData>();
    private Dictionary<Character, SymbolData> answersReceived = new Dictionary<Character, SymbolData>();

    public SymbolGameAI(SymbolGame _game, Bot _participant) /*: base(_game,_participant)*/
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
    public /*override*/ SymbolData Ask(Character character)
    {
        //TODO : return participant.getData()
        SymbolData participantSymbol = game.GetData(participant);
        //Debug.Log(character.name + " Ask to " + participant.name);
        //Debug.Log("Bernoulli Law p(" + participant.GetRelation(character) / (double) Character.maxByteValue + ")");

        if (answersGived.ContainsKey(character))
        {
            return answersGived[character];
        }

        SymbolData symbolAnswered = participantSymbol;
        if (AI.BernoulliLaw(participant.GetRelation(character) / (double) Bot.maxByteValue)) {
            // Return Truth
            symbolAnswered = participantSymbol;
        } else
        {
            // Return Lie
            symbolAnswered = game.GetAnotherRandomSymbol(participantSymbol);
        }
        answersGived[character] = symbolAnswered;
        return symbolAnswered;
    }
    
    // TODO : Use DebugLevel to follow AI behavior
    // TODO : using method : maxConfianceValidAnswer ?
    public override void Choice()
    {

        Dictionary<SymbolData, uint> symbolsReceived = new Dictionary<SymbolData, uint>();
        foreach (SymbolData symbol in SymbolGame.symbols)
        {
            symbolsReceived[symbol] = 0;
        }

            switch (participant.GetStrategy())
        {
            case STRATEGY.RANDOM:
                SymbolData randomSymbol = game.GetRandomSymbol();
                foreach (SymbolData symbol in SymbolGame.symbols)
                {
                    if (symbol != randomSymbol)
                    {
                        symbolsReceived[symbol]++;
                    }
                }
                break;
            case STRATEGY.CONFIANCE:
                foreach (Character character in game.GetParticipants())
                {
                    if (character != participant)
                    {
                        if (character.CompareTag("Bot"))
                        {
                            SymbolData symbolData = character.AskedBy(participant) as SymbolData;
                            Assert.IsNotNull(symbolData);
                            answersReceived[character] = symbolData;
                            symbolsReceived[symbolData]++;
                        }
                        else if (character.CompareTag("Player"))
                        {
                            if (answersReceived.ContainsKey(character))
                            {
                                symbolsReceived[answersReceived[character]]++;
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
                            SymbolData symbolData = character.AskedBy(participant) as SymbolData;
                            Assert.IsNotNull(symbolData);
                            answersReceived[character] = symbolData;

                        }

                        if (answersReceived.ContainsKey(character))
                        {
                            if (participant.GetRelation(character) > mefiance)
                            {
                                symbolsReceived[answersReceived[character]]++;
                            }
                            else
                            {
                                foreach (SymbolData symbol in SymbolGame.symbols)
                                {
                                    if (symbol != answersReceived[character])
                                    {
                                        symbolsReceived[symbol]++;
                                    }
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



        SymbolData symbolAnswer = symbolsReceived.OrderBy(x => x.Value).First().Key;
        game.ParticipantAnswer(participant, symbolAnswer);
    }

    public void Response(Character character, SymbolData symbol)
    {
        answersReceived[character] = symbol;
    }

    public override void Clear()
    {
        answersGived.Clear();
        answersReceived.Clear();
    }

    // TODO : How to Generic
    public void CheckAnswers(Dictionary<Character, SymbolData> goodAnswers)
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
        SymbolData symbol = data as SymbolData;
        Assert.IsNotNull(symbol);
        Response(character, symbol);
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
