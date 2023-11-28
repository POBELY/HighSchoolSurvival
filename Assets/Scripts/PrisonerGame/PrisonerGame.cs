using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class PrisonerGame : Game
{

    // static PrisonerData element ?
    public static List<PrisonerData> actions = new List<PrisonerData> { new PrisonerData(PrisonerData.ACTION.NEUTRAL), new PrisonerData(PrisonerData.ACTION.COOPERATE), new PrisonerData(PrisonerData.ACTION.BETRAY) };
    // TODO : in PrisonerData
    public static Dictionary<PrisonerData.ACTION, Color> actionsColor = new Dictionary<PrisonerData.ACTION, Color> { { PrisonerData.ACTION.NEUTRAL, Color.white }, { PrisonerData.ACTION.COOPERATE, Color.green }, { PrisonerData.ACTION.BETRAY, Color.red } };

    // TODO : what is difference between answers and participantSymbol ?
    // TODO : Dictionary<Character, PrisonerData.ACTION> ?
    protected Dictionary<Character, PrisonerData> answers = new Dictionary<Character, PrisonerData>();
    private Dictionary<Character, PrisonerData> participantsActions = new Dictionary<Character, PrisonerData>(); // Datas

    private List<Character> neutralParticipants = new List<Character>();
    private List<Character> coorporatingParticipants = new List<Character>();
    private List<Character> betrayingParticipants = new List<Character>();

    [SerializeField] private Button buttonPrefab;

    private GridLayoutGroup grid;
    private Dictionary<PrisonerData, Button> buttons = new Dictionary<PrisonerData, Button>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Instantiate GridLayoutGroup
        GameObject gridGameObject = new GameObject("PrisonerGameGridLayout");
        grid = gridGameObject.gameObject.AddComponent<GridLayoutGroup>();
        grid.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        grid.transform.localPosition = Vector3.zero;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;

        // Instantiate buttons
        foreach (PrisonerData action in actions)
        {
            Button button = Instantiate(buttonPrefab);
            button.transform.SetParent(grid.transform, false);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = action.ToString();
            button.onClick.AddListener(delegate { PlayerAnswer(action); });
            button.gameObject.SetActive(false);
            buttons.Add(action, button);
        }

        // Instantiate AI
        foreach (Character participant in participants)
        {
            if (participant.gameObject.CompareTag("Bot"))
            {
                ((Bot)participant).SetAI(new PrisonerGameAI(this, ((Bot)participant)));
            }
        }
        ActionsAttribution();
        state = STATE.RUNNING;

    }

    // Update is called once per frame
    void Update()
    {
        if (state == STATE.RUNNING)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                DisplayButton();
            }
        }
    }

    // TODO : Not necessary ?
    void ActionsAttribution()
    {
        System.Random rand = new System.Random();
        
        // Give one random symbol to each participant
        foreach (Character participant in participants)
        {
            int indexAction2Attribute = rand.Next(0, actions.Count);
            participantsActions[participant] = actions[indexAction2Attribute];
#if DEBUG
            //Debug.Log("Participant " + participant.name + " was attributed color " + color2Attribute);
            participant.GetComponent<Renderer>().material.color = actionsColor[participantsActions[participant].action];
#endif
        }
    }

    // TODO : Character method calling game method ?
    public void PlayerAnswer(PrisonerData action)
    {
        ParticipantsAnswer();
        ParticipantAnswer(player, action);
        CheckAnswers();
    }

    public void ParticipantAnswer(Character participant, PrisonerData answer)
    {
        answers.Add(participant, answer);
        participantsActions[participant] = answer;
#if DEBUG
        participant.GetComponent<Renderer>().material.color = actionsColor[answer.action];
#endif
    }

    public void ParticipantsAnswer()
    {
        foreach (Character participant in participants.ToArray())
        {
            // TODO : void Answer for player
            if (participant.CompareTag("Bot"))
            {     
                ( (Bot) participant).GiveAnswer();
            }
        }
    }

    private void CheckAnswers()
    {
        // TODO : For all games that remove all buttons, loop on all buttons
        foreach (Button button in buttons.Values)
        {
            button.gameObject.SetActive(false);
        }

        neutralParticipants = new List<Character>();
        coorporatingParticipants = new List<Character>();
        betrayingParticipants = new List<Character>();

        foreach (KeyValuePair<Character,PrisonerData> answerKvp in answers)
        {
            if (answerKvp.Value.action == PrisonerData.ACTION.NEUTRAL) {
                neutralParticipants.Add(answerKvp.Key);
            } else if (answerKvp.Value.action == PrisonerData.ACTION.COOPERATE) {
                coorporatingParticipants.Add(answerKvp.Key);
            } else if (answerKvp.Value.action == PrisonerData.ACTION.BETRAY) {
                betrayingParticipants.Add(answerKvp.Key);
            }
        }

        bool endGame = false;
        System.Random rand = new System.Random();
        List<Character> losingParticipants = new List<Character>();
        List<Character> betrayingParticipantsCopy = new List<Character>(betrayingParticipants);
        if (betrayingParticipants.Count == 0)
        {
            losingParticipants = neutralParticipants;
            endGame = true;
        } else {
            foreach (Character participant in betrayingParticipantsCopy)
            {
                // TODO : If betray.count > cooperate.Count, all cooperted lose, no need for random
                if (coorporatingParticipants.Count > 0)
                {
                    int randomIndex = rand.Next(0, coorporatingParticipants.Count);
                    losingParticipants.Add(coorporatingParticipants[randomIndex]);
                    Debug.Log(coorporatingParticipants[randomIndex].name + " (cooperating) eliminated");
                    coorporatingParticipants.RemoveAt(randomIndex);

                } else
                {
                    int randomIndex = rand.Next(0, betrayingParticipants.Count);
                    losingParticipants.Add(betrayingParticipants[randomIndex]);
                    Debug.Log(betrayingParticipants[randomIndex].name + " (betraying) eliminated");
                    betrayingParticipants.RemoveAt(randomIndex);
                }
            }
        }

        answers.Clear();

        // Cleaning Round

        // TODO : Verif and clean
        // Clear winners from losers
        //foreach (Character character in participants)
        foreach (Character character in participants.Except(losingParticipants))
            {
            foreach (Character losingParticipant in losingParticipants)
            {
                character.Clear(losingParticipant);
            }
        }

        // TODO : method to eliminate losing participants
        foreach (Character losingParticipant in losingParticipants)
        {
            if (losingParticipant.CompareTag("Bot"))
            {
                // TODO : override character destruction / game destroy character method
                participants.Remove(losingParticipant);
                participantsActions.Remove(losingParticipant);
                player.UpdateRemovedParticipant(losingParticipant);
                Destroy(losingParticipant.gameObject);
            } else if (losingParticipant.CompareTag("Player")) {
                Debug.Log("FAILED Round");
                // TODO : EndGame/Losing method
                participants.Remove(player);
                participantsActions.Remove(player);
                Destroy(player.gameObject);
                state = STATE.FINISH;
            }
        }

        //TODO : surviving participants needs to update confiance according to other participants answers
        foreach (Character character in participants)
        {
            //TODO : create Bot class and use directly Clear without Bot Condition
            if (character.CompareTag("Bot"))
            {
                ((Bot)character).CheckAnswers<PrisonerData>(participantsActions);
                ((Bot)character).Clear();
#if DEBUG
                ((Bot)character).CopyRelations();
#endif
            }
            else if (character.CompareTag("Player"))
            {
                Debug.Log("SUCCESS Round");
            }
        }

        // TODO ? : Make it in another method or in Update ?
        // Start new Round or Finish Game
        if (endGame)
        {
            state = STATE.FINISH;
            buttons.Clear();
            Debug.Log("End PrisonerGame");
        } else
        {
            Debug.Log("New Round");
        }
        
    }

    //TODO ? : public override void Discussion(Player sender, Bot receiver) | or assert and cast
    public override void Discussion(Character sender, Character receiver)
    {

        // TODO : Create dialogue from source file
        List<Message> dialogue = new List<Message>();
        dialogue.Add(new Message(sender, "Hello, can you tell me the action that you will choose please ?"));
        if (receiver.CompareTag("Bot"))
        {
            //Debug.Log("Bot Answer");
            dialogue.Add(new Message(receiver, "Yes, It will be " + receiver.AskedBy(sender)));
        }
        else
        {
            Debug.Log("Player Answer");
            // TODO : Interractions with other players
            dialogue.Add(new Message(receiver, "Yes, It will be " + GetData(receiver)));
        }

        dialogue.Add(new Message(receiver, "And you ?"));
        /*void ActionLog(ColorData col)
        {
            Debug.Log("Action " + sender.name + " to " + receiver.name + " with " + col);
        }*/
        Action<PrisonerData> action = (symbol) => receiver.GetResponse(sender, symbol);
        //action += ActionLog;
        dialogue.Add(new ChoicesMessage<PrisonerData>(sender, "For me, is ", actions, action));

        // TODO : Apply player Answer for Bot
        GameManager.Instance.ui.SetDialogue(dialogue);
        GameManager.Instance.ui.ActivateDialogueBox();

    }

    // TODO : herit from UI
    void DisplayButton()
    {

        grid.constraintCount =  Mathf.FloorToInt(Mathf.Sqrt(buttons.Count));
        if ((buttons.Count % (grid.constraintCount) != 0) && (buttons.Count % (grid.constraintCount - 1) == 0) )
        {
            --grid.constraintCount;
        }
        foreach (Button button in buttons.Values)
        {
            button.gameObject.SetActive(true);
        }
    }

    // TODO : Data method ?
    public int GetNbSymbols()
    {
        return actions.Count;
    }

    public PrisonerData GetData(Character character)
    {
        return participantsActions[character];
    }

    // TODO : Data method ?
    public PrisonerData GetRandomAction()
    {
        System.Random rand = new System.Random();
        int randomIndex = rand.Next(0, actions.Count);
        return actions[randomIndex];
    }

    // TODO : Data method ?
    public PrisonerData GetAnotherRandomAction(PrisonerData action)
    {
        int index = actions.IndexOf(action);
        System.Random rand = new System.Random();
        int randomIndex = (index + rand.Next(1, actions.Count)) % actions.Count;
        return actions[randomIndex];
    }

    public List<Character> getLastRoundParticipants(PrisonerData data)
    {
        switch (data.action) {
            case PrisonerData.ACTION.COOPERATE:
                return coorporatingParticipants;
            case PrisonerData.ACTION.NEUTRAL:
                return neutralParticipants;
            case PrisonerData.ACTION.BETRAY:
                return betrayingParticipants;
            default:
                return null;
        }

    }

}
