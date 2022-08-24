using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class ColorGame : Game
{

    private List<Color> colors = new List<Color> { Color.blue, Color.green, Color.red, Color.yellow };

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

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            DisplayButton();
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
        List<Color> colors2Add = colors;
        for (int i = 0; i < (participants.Count % colors.Count); ++i)
        {
            int indexColor2Add = rand.Next(0, colors2Add.Count);
            colors2Attribute[colors2Add[indexColor2Add]]++;
            colors2Add.Remove(colors2Add[indexColor2Add]);
        }

        // Give one random color to each participant
        foreach (Character participant in participants)
        {
            int indexColor1Attribute = rand.Next(0, colors2Attribute.Keys.Count);
            Color color2Attribute = (new List<Color>(colors2Attribute.Keys))[indexColor1Attribute];
            participantsColors[participant] = color2Attribute;        
            colors2Attribute[color2Attribute]--;
            if (colors2Attribute[color2Attribute] == 0)
            {
                colors2Attribute.Remove(color2Attribute);
            }
#if DEBUG
            Debug.Log("Participant " + participant.name + "was attributed color " + color2Attribute.ToString());
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
        if (Answer(player,character))
        {
            Debug.Log("SUCCESS Round");
        } else
        {
            Debug.Log("FAILED Round");
        }
        ParticipantsAnswer();
    }

    public void ParticipantAnswer(Character participant, Character answer)
    {
        if (! Answer(participant,answer))
        {
            Debug.Log("WRONG ANSWER for participant " + participant.name + " : " + answer.name);
            participants.Remove(participant);
            participantsColors.Remove(participant);
            buttons.Remove(participant);
            Destroy(participant.gameObject);
        } else
        {
            Debug.Log("GOOD ANSWER for participant " + participant.name + " : " + answer.name);
        }
    }

    public void ParticipantsAnswer()
    {
        foreach (Character participant in participants.ToArray())
        {
            if (participant != player)
            {
                buttons[participant].gameObject.SetActive(false);
                participant.Answer();
            }
        }
        ColorsAttribution();
    }

    void DisplayButton()
    {

        grid.constraintCount =  Mathf.FloorToInt(Mathf.Sqrt(participants.Count));
        foreach (Button button in buttons.Values)
        {
            button.gameObject.SetActive(true);
        }
    }
}
