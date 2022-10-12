using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class AI
{

    public static bool BernoulliLaw(double value)
    {
        float rand = Random.value;
        if (rand <= value)
            return true;
        else
        {
            return false;
        }
    }

    public enum STRATEGY { CONFIANCE, MEFIANCE, RANDOM };

    static public STRATEGY GetStrategy(string strategy)
    {
        switch (strategy)
        {
            case "CONFIANCE":
                return STRATEGY.CONFIANCE;
            case "MEFIANCE":
                return STRATEGY.MEFIANCE;
            case "RANDOM":
                return STRATEGY.RANDOM;
            default:
                Debug.LogError("Strategy not known : RANDOM by default");
                return STRATEGY.RANDOM;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // QUESTION : return bool ?
    public abstract void Choice();

    //public abstract void Ask(Character character);

}
