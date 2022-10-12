using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public class RelationsDictionary : SerializableDictionary<Character, byte> { }
//[Serializable] public class MyDictionary : SerializableDictionary<string, int> { }

public class Character : MonoBehaviour
{
    public enum CLASSNAME { A1, B1, C1, A2, B2, C2, A3, B3, C3 };

    //public string name;
    public CLASSNAME classname;
    //TODO genric AI
    //private AI ai;
    private ColorGameAI ai;
    [SerializeField] private AI.STRATEGY strategy;

    public static byte maxByteValue = 255;
    [Serializable] public struct RelationValue
    {
        public Character character;
        public byte value;
    }
    //[SerializeField] public List<RelationValue> relationsInit = new List<RelationValue>();
    // TODO : be private
    //private Dictionary<Character, byte> relations = new Dictionary<Character, byte>();
    public Dictionary<Character, byte> relations = new Dictionary<Character, byte>();
    public RelationsDictionary relations2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color Asked(Character character)
    {
        return ai.Ask(character);
    }

    public void Answer()
    {
        Debug.Log(name);
        ai.Choice();
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

    public byte GetRelation(Character character)
    {
        return relations[character];
    }

}
