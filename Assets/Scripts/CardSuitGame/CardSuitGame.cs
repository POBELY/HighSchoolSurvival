using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class CardSuitGame : Game
{

    public static List<CardSuitData> cards = new List<CardSuitData> { new CardSuitData(CardSuitData.CARD.HEART), new CardSuitData(CardSuitData.CARD.DIAMOUND), new CardSuitData(CardSuitData.CARD.SPADE), new CardSuitData(CardSuitData.CARD.CLUB) };
    public static Dictionary<CardSuitData.CARD, Color> cardsColor = new Dictionary<CardSuitData.CARD, Color> { { CardSuitData.CARD.HEART, Color.red }, { CardSuitData.CARD.DIAMOUND, Color.yellow }, { CardSuitData.CARD.SPADE, Color.black }, { CardSuitData.CARD.CLUB, Color.green }};


    protected Dictionary<Character,CardSuitData> answers = new Dictionary<Character, CardSuitData>();

    // TODO : Generic participantData ?
    private Dictionary<Character, CardSuitData> participantsCardSuit = new Dictionary<Character, CardSuitData>(); // Datas

    [SerializeField] private Button buttonPrefab;

    private GridLayoutGroup grid;
    private Dictionary<CardSuitData, Button> buttons = new Dictionary<CardSuitData, Button>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Instantiate GridLayoutGroup
        GameObject gridGameObject = new GameObject("CardSuitrGameGridLayout");
        grid = gridGameObject.gameObject.AddComponent<GridLayoutGroup>();
        grid.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        grid.transform.localPosition = Vector3.zero;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;

        // Instantiate buttons
        foreach (CardSuitData card in cards)
        {
            Button button = Instantiate(buttonPrefab);
            button.transform.SetParent(grid.transform, false);
            button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = card.ToString();
            button.onClick.AddListener(delegate { PlayerAnswer(card); });
            button.gameObject.SetActive(false);
            buttons.Add(card, button);
        }

        // Instantiate AI
        foreach (Character participant in participants)
        {
            if (participant.gameObject.CompareTag("Bot"))
            {
                // Instantiate AI
                ((Bot)participant).SetAI(new CardSuitGameAI(this, ((Bot)participant)));
            }
        }
        CardSuitsAttribution();
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

    void CardSuitsAttribution()
    {
        System.Random rand = new System.Random();

        // Give one random cardSit to each participant
        foreach (Character participant in participants)
        {
            int indexSymbol2Attribute = rand.Next(0, cards.Count);
            participantsCardSuit[participant] = cards[indexSymbol2Attribute];

#if DEBUG
            //Debug.Log("Participant " + participant.name + " was attributed color " + color2Attribute);
            participant.GetComponent<Renderer>().material.color = cardsColor[participantsCardSuit[participant].card];
#endif

        }

    }

    private bool Answer(Character character, CardSuitData card)
    {
        return participantsCardSuit[character] == card;
    }

    public void PlayerAnswer(CardSuitData card)
    {

        // TODO : ParticipantAnswer(player,character)
        if (Answer(player, card))
        {
            Debug.Log("SUCCESS Round");
            ParticipantsAnswer();
        } else
        {
            Debug.Log("FAILED Round");
            // TODO : EndGame/Losing method
            participants.Remove(player);
            participantsCardSuit.Remove(player);
            Destroy(player.gameObject);
            state = STATE.FINISH;
        }
    }

    public void ParticipantAnswer(Character participant, CardSuitData answer)
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
        CheckAnswers();
    }

    private void CheckAnswers()
    {
        foreach (CardSuitData card in cards)
        {
            buttons[card].gameObject.SetActive(false);
        }


        List<Character> losingParticipants = new List<Character>();
        foreach (Character participant in answers.Keys.ToArray())
        {
            CardSuitData answer = answers[participant];
            if (!Answer(participant, answer))
            {
                Debug.Log("WRONG ANSWER for participant " + participant.name + " : " + answer);
                losingParticipants.Add(participant);
            }
            else
            {
                Debug.Log("GOOD ANSWER for participant " + participant.name + " : " + answer);
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
            // TODO : override character destruction / game destroy character method
            participants.Remove(losingParticipant);
            participantsCardSuit.Remove(losingParticipant);
            player.UpdateRemovedParticipant(losingParticipant);
            Destroy(losingParticipant.gameObject);
        }

        //TODO : surviving participants needs to update confiance according to other participants answers
        foreach (Character character in participants)
        {
            //TODO : create Bot class and use directly Clear without Bot Condition
            if (character.CompareTag("Bot"))
            {
                ( (Bot) character).CheckAnswers<CardSuitData>(participantsCardSuit);
                ( (Bot) character).Clear();
#if DEBUG
                ( (Bot) character).CopyRelations();
#endif
            }
        }

        // TODO ? : Make it in another method or in Update ?
        // Start new Round or Finish Game
        if (participants.Count >= nbParticipantsNeeded)
        {
            Debug.Log("Reattributing cards");
            CardSuitsAttribution();
        } else
        {
            state = STATE.FINISH;
            Debug.Log("End CardSuitGame");
        }
    }

    //TODO ? : public override void Discussion(Player sender, Bot receiver) | or assert and cast
    public override void Discussion(Character sender, Character receiver)
    {

        // TODO : Create dialogue from source file
        List<Message> dialogue = new List<Message>();
        dialogue.Add(new Message(sender, "Hello, can you tell me my card suit please ?"));
        if (receiver.CompareTag("Bot"))
        {
            Debug.Log("Bot Answer");
            dialogue.Add(new Message(receiver, "Yes, It is " + receiver.AskedBy(sender)));
        }
        else
        {
            Debug.Log("Player Answer");
            // TODO : Interractions with other players
            dialogue.Add(new Message(receiver, "Yes, It is " + GetData(receiver)));
        }

        dialogue.Add(new Message(receiver, "And me ?"));
        /*void ActionLog(ColorData col)
        {
            Debug.Log("Action " + sender.name + " to " + receiver.name + " with " + col);
        }*/
        Action<CardSuitData> action = (col) => receiver.GetResponse(sender, col);
        //action += ActionLog;
        dialogue.Add(new ChoicesMessage<CardSuitData>(sender, "For me, is ", cards, action));

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

    public int GetNbCardSuits()
    {
        return cards.Count;
    }

    public CardSuitData GetData(Character character)
    {
        return participantsCardSuit[character];
    }

    // TODO : Generic
    public CardSuitData GetRandomCardSuit()
    {
        System.Random rand = new System.Random();
        return cards[rand.Next(0, cards.Count)];
    }

    // TODO : Generic
    public CardSuitData GetAnotherRandomCardSuit(CardSuitData card)
    {
        int index = cards.IndexOf(card);
        System.Random rand = new System.Random();
        int randomIndex = (index + rand.Next(1, cards.Count)) % cards.Count;
        return cards[randomIndex];
    }

}
