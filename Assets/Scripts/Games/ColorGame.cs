using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;


public class ColorGame : Game
{

    private List<Color> colors = new List<Color> { Color.blue, Color.green, Color.red, Color.yellow };

    private Dictionary<Character, Color> participantsColors = new Dictionary<Character, Color>();

    [SerializeField] private Button buttonPrefab;

    private GridLayoutGroup grid;
    private Dictionary<Character, Button> buttons = new Dictionary<Character, Button>();

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        ColorsAttribution();
        // Instantiate GridLayoutGroup
        GameObject gridGameObject = new GameObject("ColorGameGridLayout");
        grid = gridGameObject.gameObject.AddComponent<GridLayoutGroup>();
        grid.transform.SetParent(GameObject.Find("Canvas").GetComponent<Canvas>().transform, false);
        grid.transform.localPosition = Vector3.zero;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedRowCount;
        // Instantiate buttons
        foreach (Character participant in participants)
        {
            if (participant != player)
            {
                Button button = Instantiate(buttonPrefab);
                button.transform.SetParent(grid.transform, false);
                button.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = participant.name;
                button.onClick.AddListener(delegate { AnswerPlayer(participant); });
                button.gameObject.SetActive(false);
                buttons.Add(participant,button);
#if DEBUG
                button.GetComponent<Image>().color = participantsColors[participant];
#endif
            }
        }
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
#endif
        }

    }

    private bool Answer(Character character1, Character character2)
    {
        return participantsColors[character1] == participantsColors[character2];
    }

    public void AnswerPlayer(Character character)
    {
        if (Answer(player,character))
        {
            Debug.Log("SUCCESS Round");
        } else
        {
            Debug.Log("FAILED Round");
        }
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
