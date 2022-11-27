using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Assertions;

using UnityEditor;

//[Serializable] public class RelationsDictionary : SerializableDictionary<Character, byte> { }

#if DEBUG
[Serializable] public class ConfianceTest
{
    public string character;
    public byte confiance;

    public ConfianceTest(string character, byte confiance)
    {
        this.character = character;
        this.confiance = confiance;
    }
}

[CustomPropertyDrawer(typeof(ConfianceTest))]
public class VectorPropertyDrawer : PropertyDrawer
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






//TODO : Charater and AI : friend Class ?
public class Character : MonoBehaviour
{
    public enum STATE { IDLE, MOVING, DIALOGUING };
    public enum CLASSNAME { A1, B1, C1, A2, B2, C2, A3, B3, C3 };

    [SerializeField] protected STATE state = STATE.IDLE;
    [SerializeField] protected CLASSNAME classname;

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
    public Dictionary<Character, byte> relations = new Dictionary<Character, byte>();

#if DEBUG
    [SerializeField] public List<ConfianceTest> relations2 = new List<ConfianceTest>();

    public void CopyRelations()
    {
        relations2.Clear();
        foreach (Character character in relations.Keys)
        {
            relations2.Add(new ConfianceTest(character.name, relations[character]));
        }
    }
#endif

    public Character()
    {
        state = STATE.IDLE;
        relations = new Dictionary<Character, byte>();
        relations2 = new List<ConfianceTest>();
    }

        public Character(Character character)
    {
        name = character.name;
        classname = character.classname;
        state = character.state;
        avatar = character.GetAvatar();

        

        relations = character.relations;
#if DEBUG
        relations2 = character.relations2;
#endif
        //this.tag = "Player";
        //controller = this.gameObject.AddComponent<CharacterController>();
        //nearCharacters = new List<Character>();
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    // TODO : Reanme method, we don't know who ask who when called (AskedBy ?)
    // TODO : How to Generic : Bot class
    public ColorData Asked(Character character)
    {
        return ai.Ask(character);
    }

    // TODO : How to Generic : Bot class
    public void Response(Character character, ColorData color)
    {
        ai.Response(character, color);
    }

    // TODO : How to Generic : Bot class
    public void Answer()
    {
        ai.Choice();
    }

    public void IncrementRelation(Character character, byte increment)
    {
        //Debug.Log("increment relation " + this.name + " and " + character.name + " : " + increment);
        Assert.IsTrue(relations.ContainsKey(character));
        /*if (maxByteValue - relations[character] > increment)
        {
            relations[character] += increment;
        } else
        {
            relations[character] = maxByteValue;
        }*/
        // Other method
        relations[character] += (byte) Math.Min(increment, maxByteValue - relations[character]);
    }

    public void DecrementRelation(Character character, byte decrement)
    {
        //Debug.Log("decrement relation " + this.name + " and " + character.name + " : " + decrement);
        Assert.IsTrue(relations.ContainsKey(character));
        /*if (relations[character] < decrement )
        {
            relations[character] = minByteValue;
        }
        else
        {
            relations[character] -= decrement;
        }*/
        // Other method
        relations[character] -= (byte) Math.Min(decrement, relations[character]);
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

    public CLASSNAME GetClassname()
    {
        return classname;
    }

    public STATE GetState()
    {
        return state;
    }

    // TODO : How to Generic : Bot class
    public void Clear()
    {
        ai.Clear();
    }

    // TODO : How to Generic : Bot class
    public void CheckAnswers(Dictionary<Character, ColorData> goodAnswers)
    {
        ai.CheckAnswers(goodAnswers);
    }

}
