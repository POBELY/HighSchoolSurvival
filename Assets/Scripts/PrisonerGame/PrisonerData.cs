using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrisonerData : Data
{

    public enum ACTION { NEUTRAL, COOPERATE, BETRAY };
    public static Dictionary<ACTION, string> symbolsName = new Dictionary<ACTION, string>() { { ACTION.NEUTRAL, "NEUTRAL" }, { ACTION.COOPERATE, "COOPERATE" }, { ACTION.BETRAY, "BETRAY" } };

    public ACTION action;

    public PrisonerData(ACTION action)
    {
        this.action = action;
    }

    public override string ToString()
    {
        return this.action.ToString();
    }

}
