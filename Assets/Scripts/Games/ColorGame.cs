using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;

public class ColorGame : Game
{

    private List<ColorData> colors = new List<ColorData> { new ColorData(Color.blue), new ColorData(Color.green), new ColorData(Color.red), new ColorData(Color.yellow) };

    private Dictionary<Character, ColorData> participantsColors = new Dictionary<Character, ColorData>(); // Datas

    [SerializeField] private Button buttonPrefab;

    private GridLayoutGroup grid;
    private Dictionary<Character, Button> buttons = new Dictionary<Character, Button>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        // Instantiate GridLayoutGroup
        GameObject gridGameObject = new GameObject("ColorGameGridLayout");
        grid = gridGameObject.gameObject.AddComponent<GridLayoutGroup>();
        grid.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        grid.transform.localPosition = Vector3.zero;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;

        foreach (Character participant in participants)
        {
            if (participant.gameObject.CompareTag("Bot"))
            {
                // Instantiate AI
                participant.SetAI(new ColorGameAI(this, participant));
                // Instantiate buttons
                Button button = Instantiate(buttonPrefab);
                button.transform.SetParent(grid.transform, false);
                button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = participant.name;
                button.onClick.AddListener(delegate { PlayerAnswer(participant); });
                button.gameObject.SetActive(false);
                buttons.Add(participant,button);
            }
        }
        ColorsAttribution();
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

    void ColorsAttribution()
    {
        System.Random rand = new System.Random();
        // Determine all colors to attribute to participants
        Dictionary<ColorData, int> colors2Attribute = new Dictionary<ColorData, int>();
        // Initialize with minimum number to attribute for each color 
        foreach (ColorData color in colors)
        {
            colors2Attribute[color] = participants.Count / colors.Count;
        }

        // Completing colors without repetition to have as much colors than participants
        List<ColorData> colors2Add = new List<ColorData>(colors);
        for (int i = 0; i < (participants.Count % colors.Count); ++i)
        {
            int indexColor2Add = rand.Next(0, colors2Add.Count);
            colors2Attribute[colors2Add[indexColor2Add]]++;
            colors2Add.Remove(colors2Add[indexColor2Add]);
        }
        
        // Give one random color to each participant
        foreach (Character participant in participants)
        {
            int indexColor2Attribute = rand.Next(0, colors2Attribute.Keys.Count);
            ColorData color2Attribute = (new List<ColorData>(colors2Attribute.Keys))[indexColor2Attribute];
            participantsColors[participant] = color2Attribute;        
            colors2Attribute[color2Attribute]--;
            if (colors2Attribute[color2Attribute] == 0)
            {
                colors2Attribute.Remove(color2Attribute);
            }
#if DEBUG
            //Debug.Log("Participant " + participant.name + " was attributed color " + color2Attribute);
            participant.GetComponent<Renderer>().material.color = participantsColors[participant].color;
            if (participant != player)
            {
                buttons[participant].GetComponent<Image>().color = participantsColors[participant].color;
            }
#endif
        }

    }

    private bool Answer(Character character1, Character character2)
    {
        return participantsColors[character1] == participantsColors[character2];
    }

    public void PlayerAnswer(Character character)
    {

        // TODO : ParticipantAnswer(player,character)
        if (Answer(player,character))
        {
            Debug.Log("SUCCESS Round");
            ParticipantsAnswer();
        } else
        {
            Debug.Log("FAILED Round");
            // TODO : EndGame/Losing method
            participants.Remove(player);
            participantsColors.Remove(player);
            Destroy(player.gameObject);
            state = STATE.FINISH;
        }
    }

    public void ParticipantAnswer(Character participant, Character answer)
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
                participant.Answer();
            }
        }
        CheckAnswers();
    }

    private void CheckAnswers()
    {
        List<Character> losingParticipants = new List<Character>();
        foreach (Character participant in answers.Keys.ToArray())
        {
            buttons[participant].gameObject.SetActive(false);
            Character answer = answers[participant];
            if (!Answer(participant, answer))
            {
                Debug.Log("WRONG ANSWER for participant " + participant.name + " : " + answer.name);
                losingParticipants.Add(participant);
            }
            else
            {
                Debug.Log("GOOD ANSWER for participant " + participant.name + " : " + answer.name);
            }       
        }
        answers.Clear();

        // Cleaning Round

        foreach (Character character in participants)
        {
            foreach (Character losingParticipant in losingParticipants)
            {
                if (!losingParticipants.Contains(character) && character.relations.ContainsKey(losingParticipant))
                {
                    character.relations.Remove(losingParticipant);
                }
            }
        }

        // TODO : method to eliminate losing participants
        foreach (Character losingParticipant in losingParticipants)
        {
            // TODO : override character destruction / game destroy character method
            participants.Remove(losingParticipant);
            participantsColors.Remove(losingParticipant);
            buttons.Remove(losingParticipant);
            player.UpdateRemovedParticipant(losingParticipant);
            Destroy(losingParticipant.gameObject);
        }

        //TODO : surviving participants needs to update confiance according to other participants answers
        foreach (Character character in participants)
        {
            //TODO : create Bot class and use directly Clear without Bot Condition
            if (character.CompareTag("Bot"))
            {
                character.CheckAnswers(participantsColors);
                character.Clear();
#if DEBUG
                character.CopyRelations();
#endif
            }
        }

        // TODO ? : Make it in another method or in Update ?
        // Start new Round or Finish Game
        if (participants.Count >= nbParticipantsNeeded)
        {
            Debug.Log("Reattributing colors");
            ColorsAttribution();
        } else
        {
            state = STATE.FINISH;
            Debug.Log("End ColorGame");
        }

        
    }

    //TODO ? : public override void Discussion(Player sender, Bot receiver) | or assert and cast
    public override void Discussion(Character sender, Character receiver)
    {

        // TODO : Create dialogue from source file
        List<Message> dialogue = new List<Message>();
        dialogue.Add(new Message(sender, "Hello, can you tell me your color please ?"));
        if (receiver.CompareTag("Bot"))
        {
            Debug.Log("Bot Answer");
            dialogue.Add(new Message(receiver, "Yes, It is " + receiver.Asked(sender)));
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
        Action<ColorData> action = (col) => receiver.Response(sender, col);
        //action += ActionLog;
        dialogue.Add(new ChoicesMessage<ColorData>(sender, "For me, is ", colors, action));

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

    public int GetNbColors()
    {
        return colors.Count;
    }

    public ColorData GetData(Character character)
    {
        return participantsColors[character];
    }

    public ColorData GetAnotherRandomColor(ColorData color)
    {
        int index = colors.IndexOf(color);
        System.Random rand = new System.Random();
        int randomIndex = (index + rand.Next(1, colors.Count)) % colors.Count;
        return colors[randomIndex];
    }
}
