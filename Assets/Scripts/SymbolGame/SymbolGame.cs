using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class SymbolGame : Game
{

    // static SymbolData element ?
    public static List<SymbolData> symbols = new List<SymbolData> { new SymbolData(SymbolData.SYMBOL.ROUND), new SymbolData(SymbolData.SYMBOL.TRIANGLE), new SymbolData(SymbolData.SYMBOL.SQUARE) };

    // TODO : what is difference between answers and participantSymbol ?
    protected Dictionary<Character,SymbolData> answers = new Dictionary<Character, SymbolData>();
    private Dictionary<Character, SymbolData> participantsSymbols = new Dictionary<Character, SymbolData>(); // Datas

    [SerializeField] private Button buttonPrefab;

    private GridLayoutGroup grid;
    private Dictionary<SymbolData, Button> buttons = new Dictionary<SymbolData, Button>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Instantiate GridLayoutGroup
        GameObject gridGameObject = new GameObject("SybolGameGridLayout");
        grid = gridGameObject.gameObject.AddComponent<GridLayoutGroup>();
        grid.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        grid.transform.localPosition = Vector3.zero;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;

        // Instantiate buttons
        foreach (SymbolData symbol in symbols)
        {
            Button button = Instantiate(buttonPrefab);
            button.transform.SetParent(grid.transform, false);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = symbol.ToString();
            button.onClick.AddListener(delegate { PlayerAnswer(symbol); });
            button.gameObject.SetActive(false);
            buttons.Add(symbol, button);
        }

        // Instantiate AI
        foreach (Character participant in participants)
        {
            if (participant.gameObject.CompareTag("Bot"))
            {
                ((Bot)participant).SetAI(new SymbolGameAI(this, ((Bot)participant)));
            }
        }
        SymbolsAttribution();
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
    void SymbolsAttribution()
    {
        System.Random rand = new System.Random();
        
        // Give one random symbol to each participant
        foreach (Character participant in participants)
        {
            int indexSymbol2Attribute = rand.Next(0, symbols.Count);
            participantsSymbols[participant] = symbols[indexSymbol2Attribute];        
        }
    }

    // TODO : Character method calling game method ?
    public void PlayerAnswer(SymbolData symbol)
    {
        ParticipantsAnswer();
        ParticipantAnswer(player, symbol);
        CheckAnswers();
    }

    public void ParticipantAnswer(Character participant, SymbolData answer)
    {
        answers.Add(participant, answer);
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

        Dictionary<SymbolData, uint> symbolsReceived = new Dictionary<SymbolData, uint>();
        foreach (SymbolData symbol in symbols)
        {
            symbolsReceived[symbol] = 0;
        }

        foreach (SymbolData answer in answers.Values)
        {
            symbolsReceived[answer]++;
        }
        SymbolData goodAnswer = symbolsReceived.OrderBy(x => x.Value).First().Key;

        foreach (SymbolData symbol in symbols)
        {
            buttons[symbol].gameObject.SetActive(false);
        }

        List<Character> losingParticipants = new List<Character>();
        foreach (Character participant in answers.Keys.ToArray())
        {
            SymbolData answer = answers[participant];
            if (answer == goodAnswer)
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
                participantsSymbols.Remove(losingParticipant);
                player.UpdateRemovedParticipant(losingParticipant);
                Destroy(losingParticipant.gameObject);
            } else if (losingParticipant.CompareTag("Player")) {
                Debug.Log("FAILED Round");
                // TODO : EndGame/Losing method
                participants.Remove(player);
                participantsSymbols.Remove(player);
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
                ((Bot)character).CheckAnswers<SymbolData>(participantsSymbols);
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
            SymbolsAttribution();
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
        dialogue.Add(new Message(sender, "Hello, can you tell me the symbol that you will choose please ?"));
        if (receiver.CompareTag("Bot"))
        {
            //Debug.Log("Bot Answer");
            dialogue.Add(new Message(receiver, "Yes, It will be a " + receiver.AskedBy(sender)));
        }
        else
        {
            Debug.Log("Player Answer");
            // TODO : Interractions with other players
            dialogue.Add(new Message(receiver, "Yes, It is " + GetData(receiver)));
        }

        dialogue.Add(new Message(receiver, "And you ?"));
        /*void ActionLog(ColorData col)
        {
            Debug.Log("Action " + sender.name + " to " + receiver.name + " with " + col);
        }*/
        Action<SymbolData> action = (symbol) => receiver.GetResponse(sender, symbol);
        //action += ActionLog;
        dialogue.Add(new ChoicesMessage<SymbolData>(sender, "For me, is ", symbols, action));

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
        return symbols.Count;
    }

    public SymbolData GetData(Character character)
    {
        return participantsSymbols[character];
    }

    // TODO : Data method ?
    public SymbolData GetRandomSymbol()
    {
        System.Random rand = new System.Random();
        int randomIndex = rand.Next(0, symbols.Count);
        return symbols[randomIndex];
    }

    // TODO : Data method ?
    public SymbolData GetAnotherRandomSymbol(SymbolData symbol)
    {
        int index = symbols.IndexOf(symbol);
        System.Random rand = new System.Random();
        int randomIndex = (index + rand.Next(1, symbols.Count)) % symbols.Count;
        return symbols[randomIndex];
    }

}
