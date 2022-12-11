using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Assertions;


//[Serializable] public class RelationsDictionary : SerializableDictionary<Character, byte> { }

#if DEBUG
[Serializable]
public class Confiance
{
    public Character character;
    public byte confiance;

    public Confiance(Character character, byte confiance)
    {
        this.character = character;
        this.confiance = confiance;
    }
}

[CustomPropertyDrawer(typeof(Confiance))]
public class ConfianceDrawer : PropertyDrawer
{
    /// <summary>
    /// A dictionary lookup of field counts keyed by class type name.
    /// </summary>
    private static Dictionary<string, int> _fieldCounts = new Dictionary<string, int>();

    /// <summary>
    /// Called when the GUI is drawn.
    /// </summary>
    /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
    /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
    /// <param name="label">The label of this property.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var fieldCount = GetFieldCount(property);

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        EditorGUIUtility.labelWidth = 14f;
        float fieldWidth = contentPosition.width / fieldCount;
        bool hideLabels = contentPosition.width < 185;
        contentPosition.width /= fieldCount;

        using (var indent = new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
        {
            property.NextVisible(true);
            for (int i = 1; i < fieldCount; i++)
            {
                if (!property.NextVisible(true))
                {
                    break;
                }

                label = EditorGUI.BeginProperty(contentPosition, new GUIContent(property.displayName), property);
                EditorGUI.PropertyField(contentPosition, property, hideLabels ? GUIContent.none : label);
                EditorGUI.EndProperty();

                contentPosition.x += fieldWidth;
            }
        }
    }

    /// <summary>
    /// Gets the field count for the specified property.
    /// </summary>
    /// <param name="property">The property for which to get the field count.</param>
    /// <returns>The field count of the property.</returns>
    private static int GetFieldCount(SerializedProperty property)
    {
        int count;
        if (!_fieldCounts.TryGetValue(property.type, out count))
        {
            var children = property.Copy().GetEnumerator();
            while (children.MoveNext())
            {
                count++;
            }

            _fieldCounts[property.type] = count;
        }

        return count;
    }
}
#endif




public class Bot : Character
{

    private AI ai;
    [SerializeField] private AI.STRATEGY strategy;

    public static byte maxByteValue = 255;
    public static byte minByteValue = 0;

    /*[Serializable] public struct Confiance
    {
        public Character character;
        public byte value;
    }*/

    public Dictionary<Character, byte> relations = new Dictionary<Character, byte>();

#if DEBUG
    [SerializeField] public List<Confiance> relations2 = new List<Confiance>();

    public void CopyRelations()
    {
        relations2.Clear();
        foreach (Character character in relations.Keys)
        {
            relations2.Add(new Confiance(character, relations[character]));
        }
    }
#endif

    /*public Bot() : base()
    {
        relations = new Dictionary<Character, byte>();
        relations2 = new List<Confiance>();
        strategy = AI.STRATEGY.RANDOM;
        this.tag = "Bot";
    }

    public Bot(Character character) : base(character)
    {
        relations = new Dictionary<Character, byte>();
        relations2 = new List<Confiance>();
        strategy = AI.STRATEGY.RANDOM;
        this.tag = "Bot";
    }

    public Bot(Character character, AI ai, AI.STRATEGY strategy) : this(character)
    {
        this.ai = ai;
        this.strategy = strategy;
    }*/


    // Start is called before the first frame update
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {

    }

    public override Data AskedBy(Character character)
    {
        return ai.AskedBy(character);
    }

    public override void GetResponse(Character character, Data data)
    {
        ai.GetResponse(character, data);
    }

    public void GiveAnswer()
    {
        ai.GiveAnswer();
    }

    public void IncrementRelation(Character character, byte increment)
    {
        //Debug.Log("increment relation " + this.name + " and " + character.name + " : " + increment);
        Assert.IsTrue(relations.ContainsKey(character));
        relations[character] += (byte)Math.Min(increment, maxByteValue - relations[character]);
    }

    public void DecrementRelation(Character character, byte decrement)
    {
        //Debug.Log("decrement relation " + this.name + " and " + character.name + " : " + decrement);
        Assert.IsTrue(relations.ContainsKey(character));
        relations[character] -= (byte)Math.Min(decrement, relations[character]);
    }

    public byte GetRelation(Character character)
    {
        return relations[character];
    }

    // TODO : necesarry ?
    public void SetAI(AI _ai)
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

    public void Clear()
    {
        ai.Clear();
    }

    public override void Clear(Character character)
    {
        if (relations.ContainsKey(character))
        {
            relations.Remove(character);
        }
        // TODO ? : ai.Clear(character)
    }

    public void CheckAnswers<D>(Dictionary<Character, D> goodAnswers) where D : Data
    {
        ai.CheckAnswers<D>(goodAnswers);
    }

}
