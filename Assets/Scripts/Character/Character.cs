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
    private AI ai;
    //public RelationsDictionary relations;
    [Serializable] public struct RelationValue
    {
        public Character character;
        public byte value;
    }

    [SerializeField] public List<RelationValue> relationsInit = new List<RelationValue>();
    private Dictionary<Character, byte> relations = new Dictionary<Character, byte>();
    public RelationsDictionary relations2;

    // Start is called before the first frame update
    void Start()
    {
        // TODO : To Read from a file (excel or xml)
        foreach (RelationValue relation in relationsInit)
        {
            relations[relation.character] = relation.value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Answer()
    {
        ai.Answer();
    }

    public void SetAI(AI _ai)
    {
        ai = _ai;
    }
}
