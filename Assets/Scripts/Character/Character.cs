using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Assertions;

[Serializable] public class RelationsDictionary : SerializableDictionary<Character, byte> { }
//[Serializable] public class MyDictionary : SerializableDictionary<string, int> { }

//TODO : Charater and AI : friend Class ?

public class Character : MonoBehaviour
{
    public enum CLASSNAME { A1, B1, C1, A2, B2, C2, A3, B3, C3 };

    public CLASSNAME classname;

    [SerializeField] protected Texture avatar;

    // ai is specific bot attribute
    //TODO genric AI
    //private AI ai;
    private ColorGameAI ai;
    [SerializeField] private AI.STRATEGY strategy;

    // TODO : Confiance instead of Realtion
    public static byte maxByteValue = 255;
    public static byte minByteValue = 0;
    [Serializable] public struct RelationValue
    {
        public Character character;
        public byte value;
    }

    // TODO : be private, specific Bot attribute
    //private Dictionary<Character, byte> relations = new Dictionary<Character, byte>();
    public Dictionary<Character, byte> relations = new Dictionary<Character, byte>();
    //public RelationsDictionary relations2;

#if DEBUG
    [SerializeField] public List<byte> relations2 = new List<byte>();

    public void CopyRelations()
    {
        relations2.Clear();
        foreach (Character character in relations.Keys)
        {
            relations2.Add(relations[character]);
        }
    }
#endif


    // TODO : Define Character State : (IDLE, DIALOGUING, PLAYING, PAUSING)
    protected bool dialoguing = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    // TODO : Reanme method, we don't know who ask who when called (AskedBy ?)
    public Color Asked(Character character)
    {
        return ai.Ask(character);
    }

    public void Answer()
    {
        ai.Choice();
    }

    public void IncrementRelation(Character character, byte increment)
    {
        //Debug.Log("increment relation " + this.name + " and " + character.name + " : " + increment);
        if (maxByteValue - relations[character] > increment)
        {
            relations[character] += increment;
        } else
        {
            relations[character] = maxByteValue;
        }
        // Other method
        //relations[character] += (byte) Math.Min(increment, maxByteValue - relations[character]);
    }

    public void DecrementRelation(Character character, byte decrement)
    {
        //Debug.Log("decrement relation " + this.name + " and " + character.name + " : " + decrement);
        Assert.IsTrue(relations.ContainsKey(character));
        if (relations[character] < decrement )
        {
            relations[character] = minByteValue;
        }
        else
        {
            relations[character] -= decrement;
        }
        // Other method
        //relations[character] -= (byte) Math.Min(decrement, relations[character]);
    }

    public byte GetRelation(Character character)
    {
        return relations[character];
    }

    //public void SetAI(AI _ai)
    public void SetAI(ColorGameAI _ai)
    {
        ai = _ai;
    }

    public AI.STRATEGY GetStrategy()
    {
        return strategy;
    }

    public void SetStrategy(string _strategy)
    {
        strategy = AI.GetStrategy(_strategy);
    }

    public Texture GetAvatar()
    {
        return avatar;
    }

    public void SetDialoguing(bool _dialoguing)
    {
        dialoguing = _dialoguing;
    }

    // TODO : How to Generic : Bot class
    public void Clear()
    {
        ai.Clear();
    }

    // TODO : How to Generic : Bot class
    public void CheckAnswers(Dictionary<Character, Color> goodAnswers)
    {
        ai.CheckAnswers(goodAnswers);
    }

}
