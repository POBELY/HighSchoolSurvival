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

  

    public Character()
    {
        state = STATE.IDLE;
    }

        public Character(Character character)
    {
        name = character.name;
        classname = character.classname;
        state = character.state;
        avatar = character.GetAvatar();

    }


    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    // TODO : abstract ? for Player ?
    public virtual Data AskedBy(Character character)
    {
        return null;
    }

    public virtual void GetResponse(Character character, Data data)
    {
    }

    // TODO : abstract
    public virtual void Clear(Character character)
    {

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



}
