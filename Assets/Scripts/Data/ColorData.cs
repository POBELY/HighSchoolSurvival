using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorData
{
    private static Dictionary<Color, string> colorsName = new Dictionary<Color, string>() { { Color.blue, "blue" }, { Color.green, "green" }, { Color.red, "red" }, { Color.yellow, "yellow" } };

    public Color color;

    public ColorData(Color color)
    {
        this.color = color;
    }

    public override string ToString()
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>" + colorsName[color] + "</color>";
    }
}
