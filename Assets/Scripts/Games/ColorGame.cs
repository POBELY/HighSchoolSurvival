using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ColorGame : Game
{

    private List<Color> colors = new List<Color> { Color.blue, Color.green, Color.red, Color.yellow };

    private Dictionary<Character, Color> participantsColors = new Dictionary<Character, Color>();

    // Start is called before the first frame update
    void Start()
    {
        ColorsAttribution();
    }

    // Update is called once per frame
    void Update()
    {

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

        // Give one random color to attribute by participant
        foreach (Character participant in participants)
        {
            int indexColor1Attribute = rand.Next(0, colors2Attribute.Keys.Count);
            Color color2Attribute = (new List<Color>(colors2Attribute.Keys))[indexColor1Attribute];
            participantsColors[participant] = color2Attribute;
#if DEBUG
            Debug.Log("Participant " + participant.name + "was attributed color " + color2Attribute);
            participant.GetComponent<Renderer>().material.color = color2Attribute;
            #endif
            colors2Attribute[color2Attribute]--;
            if (colors2Attribute[color2Attribute] == 0)
            {
                colors2Attribute.Remove(color2Attribute);
            }

        }

    }
}
