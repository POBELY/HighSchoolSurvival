using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Linq;


public class ColorGame : Game
{

    private List<Color> colors = new List<Color> { Color.blue, Color.green, Color.red, Color.yellow };
    private Dictionary<Color,String> colorsName = new Dictionary<Color, string>() { { Color.blue, "blue" }, { Color.green, "green" }, { Color.red, "red" }, {Color.yellow, "yellow"}};

    private Dictionary<Character, Color> participantsColors = new Dictionary<Character, Color>(); // Datas

    [SerializeField] private Button buttonPrefab;

    private GridLayoutGroup grid;
    private Dictionary<Character, Button> buttons = new Dictionary<Character, Button>();

    // Start is called before the first frame update
    public override void Start()
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
            if (participant != player)
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
            if (participants.Count >= nbParticipantsNeeded)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    DisplayButton();
                }
            }
            else
            {
                Debug.Log("End ColorGame");
                state = STATE.FINISH;
            }
        }

    }

    void ColorsAttribution()
    {
        System.Random rand = new System.Random();
        // Determine all colors to attribute to participants
        Dictionary<Color, int> colors2Attribute = new Dictionary<Color, int>();
        // Initialize with minimum number to attribute for each color 
        foreach (Color color in colors)
        {
            colors2Attribute[color] = participants.Count / colors.Count;
        }

        // Completing colors without repetition to have as much colors than participants
        List<Color> colors2Add = new List<Color>(colors);
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
            Color color2Attribute = (new List<Color>(colors2Attribute.Keys))[indexColor2Attribute];
            participantsColors[participant] = color2Attribute;        
            colors2Attribute[color2Attribute]--;
            if (colors2Attribute[color2Attribute] == 0)
            {
                colors2Attribute.Remove(color2Attribute);
            }
#if DEBUG
            string colorName = $"<color=#{ColorUtility.ToHtmlStringRGB(color2Attribute)}>" + colorsName[color2Attribute] + "</color>";
            Debug.Log("Participant " + participant.name + " was attributed color " + colorName);
            participant.GetComponent<Renderer>().material.color = color2Attribute;
            if (participant != player)
            {
                buttons[participant].GetComponent<Image>().color = participantsColors[participant];
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
            if (participant != player)
            {     
                participant.Answer();
            }
        }
        CheckAnswers();
        ColorsAttribution();
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
        foreach (Character losingParticipant in losingParticipants)
        {
            // TODO : override character destruction / game destroy character method
            participants.Remove(losingParticipant);
            participantsColors.Remove(losingParticipant);
            buttons.Remove(losingParticipant);
            Destroy(losingParticipant.gameObject);
        }
        Debug.Log("End CheckAnswers");
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

    public Color GetData(Character character)
    {
        return participantsColors[character];
    }

    public Color GetAnotherRandomColor(Color color)
    {
        int index = colors.IndexOf(color);
        System.Random rand = new System.Random();
        int randomIndex = (index + rand.Next(1, colors.Count)) % colors.Count;
        return colors[randomIndex];
    }
}
