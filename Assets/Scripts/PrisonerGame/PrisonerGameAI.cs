using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

public class PrisonerGameAI : AI
{

    // TODO getIt by subGameManage SymbolGameManager
    private PrisonerGame game;
    private Bot participant;

    private Dictionary<Character, PrisonerData> answersGived = new Dictionary<Character, PrisonerData>();
    private Dictionary<Character, PrisonerData> answersReceived = new Dictionary<Character, PrisonerData>();

    public PrisonerGameAI(PrisonerGame _game, Bot _participant) /*: base(_game,_participant)*/
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
    public /*override*/ PrisonerData Ask(Character character)
    {
        //TODO : return participant.getData()
        PrisonerData participantAction = game.GetData(participant);
        //Debug.Log(character.name + " Ask to " + participant.name);
        //Debug.Log("Bernoulli Law p(" + participant.GetRelation(character) / (double) Character.maxByteValue + ")");

        if (answersGived.ContainsKey(character))
        {
            return answersGived[character];
        }

        PrisonerData actionAnswered = participantAction;
        if (AI.BernoulliLaw(participant.GetRelation(character) / (double) Bot.maxByteValue)) {
            // Return Truth
            actionAnswered = participantAction;
        } else
        {
            // Return Lie
            actionAnswered = game.GetAnotherRandomAction(participantAction);
        }
        answersGived[character] = actionAnswered;
        return actionAnswered;
    }
    
    // TODO : Use DebugLevel to follow AI behavior
    // TODO : using method : maxConfianceValidAnswer ?
    public override void Choice()
    {

        uint meanConfiance = participant.GetMeanRelation();
        Debug.Log("Participant " + participant.name + " mean confiance is " + meanConfiance);

        System.Random rand = new System.Random();
        byte randomByte = (byte) rand.Next(0, Bot.maxByteValue + 1);
        PrisonerData answer = new PrisonerData(PrisonerData.ACTION.COOPERATE);
        if (randomByte > meanConfiance)
        {
            answer = new PrisonerData(PrisonerData.ACTION.BETRAY);
        }
        int randomNeutral = rand.Next(0, 2);
        if (randomNeutral == 0)
        {
            answer = new PrisonerData(PrisonerData.ACTION.NEUTRAL);
        }

        //TODO : Take into account last round betrays and cooperations
        switch (participant.GetStrategy())
        {
            case STRATEGY.RANDOM:
                answer = game.GetRandomAction();
                break;
            case STRATEGY.CONFIANCE:
                break;
            case STRATEGY.MEFIANCE:
                break;
            default:
                Debug.LogError("Strategy not implemented");
                break;
        }

        game.ParticipantAnswer(participant, answer);
    }

    public void Response(Character character, PrisonerData action)
    {
        answersReceived[character] = action;
    }

    public override void Clear()
    {
        answersGived.Clear();
        answersReceived.Clear();
    }

    // TODO : How to Generic, Rename method
    public void CheckAnswers(Dictionary<Character, PrisonerData> goodAnswers)
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
        PrisonerData action = data as PrisonerData;
        Assert.IsNotNull(action);
        Response(character, action);
    }

    // TODO : Return answer ?
    public override void GiveAnswer()
    {
        Choice();
    }

    // TODO : Rename method
    // TODO : participants answers instead of goodAnswers for game where only answer is pertinent
    public override void CheckAnswers<D>(Dictionary<Character, D> goodAnswers)
    {
        Dictionary<Character, PrisonerData> goodActionsAnswers = new Dictionary<Character, PrisonerData>();
        foreach (Character character in goodAnswers.Keys)
        {
            PrisonerData actionData = goodAnswers[character] as PrisonerData;
            Assert.IsNotNull(actionData);
            goodActionsAnswers.Add(character, actionData);
        }
        CheckAnswers(goodActionsAnswers);
    }


}
