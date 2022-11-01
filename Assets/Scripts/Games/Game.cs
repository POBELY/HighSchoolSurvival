using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour
{

    public enum STATE {START, RUNNING, DIALOGUE, PAUSE, FINISH };

    [SerializeField] protected List<Character> participants;
    // TODO : protected Dictionary<Character,AnsweType> participants;
    protected Dictionary<Character,Character> answers = new Dictionary<Character, Character>();
    // protected Player player;
    protected Character player;
    // TODO : update to be able to have multiple players (playerController attribute of Player)
    protected CharacterController playerController;
    [SerializeField] protected int nbParticipantsNeeded = 0;
    protected STATE state = STATE.START;


    public void LoadConfiances()
    {

        string[] studentsName = { "Student (1)", "Student (2)", "Student (3)", "Student (4)", "Student (5)", "Student (6)", "Student (7)", "Student (8)", "Student (9)", "Student (10)", "Student (11)", "Student (12)", "Student (13)", "Student (14)", "Student (15)", "Student (16)", "Student (17)", "Student (18)", "Student (19)", "Student (20)" };
        Dictionary<string, Character> students = new Dictionary<string, Character>();
        foreach (string student in studentsName)
        {
            students.Add(student, GameObject.Find(student).GetComponent<Character>());
        }

        List<Dictionary<string, object>> data = CSVReader.Read("ConfiancesTable");
        for (var i = 0; i < data.Count; ++i)
        {
            string name = data[i]["name"].ToString();
            students[name].SetStrategy(data[i]["STRATEGY"].ToString());
            students[name].relations.Clear();
            foreach (string student in studentsName)
            {
                if (student != name)
                {
                    byte confiance = byte.Parse(data[i][student].ToString(), System.Globalization.NumberStyles.Integer);
                    students[name].relations.Add(students[student], confiance);
                }
            }
        }
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {

        LoadConfiances();

        System.Random rand = new System.Random();
        int indexPlayer = rand.Next(0, participants.Count);

        player = participants[indexPlayer];
        playerController = player.gameObject.AddComponent<CharacterController>();
        player.gameObject.AddComponent<PlayerController>().controller = playerController;;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Character> GetParticipants()
    {
        return participants;
    }

    public Character GetRandomParticipant()
    {
        System.Random rand = new System.Random();
        return participants[rand.Next(0,participants.Count)];
    }

    public Character GetAnotherRandomParticipant(Character participant)
    {
        System.Random rand = new System.Random();
        return participants[ (participants.IndexOf(participant) + rand.Next(1, participants.Count)) % participants.Count];
    }

    public Character GetPlayer()
    {
        return player;
    }

}
