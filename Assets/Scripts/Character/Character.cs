using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable] public class RelationsDictionary : SerializableDictionary<Character, byte> { }
//[Serializable] public class MyDictionary : SerializableDictionary<string, int> { }

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
    [Serializable] public struct RelationValue
    {
        public Character character;
        public byte value;
    }

    // TODO : be private
    //private Dictionary<Character, byte> relations = new Dictionary<Character, byte>();
    public Dictionary<Character, byte> relations = new Dictionary<Character, byte>();
    //public RelationsDictionary relations2;

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
        Debug.Log(name);
        ai.Choice();
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

}
