using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class NumberGame : Game
{

    // static SymbolData element ?
    public static List<NumberData> numbers = new List<NumberData> { new NumberData(1), new NumberData(2), new NumberData(3), new NumberData(4), new NumberData(5)};

    // TODO : what is difference between answers and participantSymbol ?
    protected Dictionary<Character, NumberData> answers = new Dictionary<Character, NumberData>();
    private Dictionary<Character, NumberData> participantsNumbers = new Dictionary<Character, NumberData>(); // Datas

    [SerializeField] private Button buttonPrefab;

    private GridLayoutGroup grid;
    private Dictionary<NumberData, Button> buttons = new Dictionary<NumberData, Button>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Instantiate GridLayoutGroup
        GameObject gridGameObject = new GameObject("NumberGameGridLayout");
        grid = gridGameObject.gameObject.AddComponent<GridLayoutGroup>();
        grid.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        grid.transform.localPosition = Vector3.zero;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;

        // Instantiate buttons
        foreach (NumberData number in numbers)
        {
            Button button = Instantiate(buttonPrefab);
            button.transform.SetParent(grid.transform, false);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
            button.onClick.AddListener(delegate { PlayerAnswer(number); });
            button.gameObject.SetActive(false);
            buttons.Add(number, button);
        }

        // Instantiate AI
        foreach (Character participant in participants)
        {
            if (participant.gameObject.CompareTag("Bot"))
            {
                ((Bot)participant).SetAI(new NumberGameAI(this, ((Bot)participant)));
            }
        }
        NumbersAttribution();
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
    void NumbersAttribution()
    {
        System.Random rand = new System.Random();
        
        // Give one random symbol to each participant
        foreach (Character participant in participants)
        {
            int indexNumber2Attribute = rand.Next(0, numbers.Count);
            participantsNumbers[participant] = numbers[indexNumber2Attribute];        
        }
    }

    // TODO : Character method calling game method ?
    public void PlayerAnswer(NumberData number)
    {
        ParticipantsAnswer();
        ParticipantAnswer(player, number);
        CheckAnswers();
    }

    public void ParticipantAnswer(Character participant, NumberData answer)
    {
        answers.Add(participant, answer);
        participantsNumbers[participant] = answer;
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

        Dictionary<NumberData, byte> numbersReceived = new Dictionary<NumberData, byte>();
        foreach (NumberData symbol in numbers)
        {
            numbersReceived[symbol] = 0;
        }

        foreach (NumberData answer in answers.Values)
        {
            numbersReceived[answer]++;
        }

        // TODO : Ordered only once
        IEnumerable<KeyValuePair<NumberData, byte>> orderedNumbersReceived = numbersReceived.OrderBy(x => x.Value).Where(x => x.Value != 0);
        NumberData goodAnswer1 = orderedNumbersReceived.First().Key;
        NumberData goodAnswer2 = orderedNumbersReceived.Last().Key;       

        Debug.Log("numbersReceived : " + Assets.Scripts.Tools.Utils.ToString(numbersReceived));
        Debug.Log("goodAnswer1 : " + goodAnswer1);
        Debug.Log("goodAnswer2 : " + goodAnswer2);

        foreach (NumberData number in numbers)
        {
            buttons[number].gameObject.SetActive(false);
        }

        List<Character> losingParticipants = new List<Character>();
        foreach (Character participant in answers.Keys.ToArray())
        {
            NumberData answer = answers[participant];
            if (answer == goodAnswer1 || answer == goodAnswer2)
            {
                Debug.Log("GOOD ANSWER for participant " + participant.name + " : " + answer);
            }
            else 
            {
                Debug.Log("WRONG ANSWER for participant " + participant.name + " : " + answer);
                losingParticipants.Add(participant);
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
                participantsNumbers.Remove(losingParticipant);
                player.UpdateRemovedParticipant(losingParticipant);
                Destroy(losingParticipant.gameObject);
            } else if (losingParticipant.CompareTag("Player")) {
                Debug.Log("FAILED Round");
                // TODO : EndGame/Losing method
                participants.Remove(player);
                participantsNumbers.Remove(player);
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
                ((Bot)character).CheckAnswers<NumberData>(participantsNumbers);
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
        if (participants.Count >= nbParticipantsNeeded)
        {
            Debug.Log("New Round");
            NumbersAttribution();
        } else
        {
            state = STATE.FINISH;
            buttons.Clear();
            Debug.Log("End SymbolGame");
        }
        
    }

    //TODO ? : public override void Discussion(Player sender, Bot receiver) | or assert and cast
    public override void Discussion(Character sender, Character receiver)
    {

        // TODO : Create dialogue from source file
        List<Message> dialogue = new List<Message>();
        dialogue.Add(new Message(sender, "Hello, can you tell me the number that you will choose please ?"));
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
        Action<NumberData> action = (symbol) => receiver.GetResponse(sender, symbol);
        //action += ActionLog;
        dialogue.Add(new ChoicesMessage<NumberData>(sender, "For me, it will be ", numbers, action, true));

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
    public int GetNbNumbers()
    {
        return numbers.Count;
    }

    public NumberData GetData(Character character)
    {
        return participantsNumbers[character];
    }

    // TODO : Data method ?
    public NumberData GetRandomNumber()
    {
        System.Random rand = new System.Random();
        int randomIndex = rand.Next(0, numbers.Count);
        return numbers[randomIndex];
    }

    // TODO : Data method ?
    public NumberData GetAnotherRandomNumber(NumberData symbol)
    {
        int index = numbers.IndexOf(symbol);
        System.Random rand = new System.Random();
        int randomIndex = (index + rand.Next(1, numbers.Count)) % numbers.Count;
        return numbers[randomIndex];
    }

}
