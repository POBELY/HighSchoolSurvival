using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

public class Game : MonoBehaviour
{

    public enum STATE {START, RUNNING, PAUSE, FINISH };

    [SerializeField] protected List<Character> participants;
    protected Dictionary<Character,Character> answers = new Dictionary<Character, Character>();
    // TODO : update to be able to have multiple players 
    protected Player player;
    [SerializeField] protected int nbParticipantsNeeded = 0;
    protected STATE state = STATE.START;

    // TODO : Create class LoadCSV
    public void LoadConfiances()
    {

        string[] studentsName = { "Student (1)", "Student (2)", "Student (3)", "Student (4)", "Student (5)", "Student (6)", "Student (7)", "Student (8)", "Student (9)", "Student (10)", "Student (11)", "Student (12)", "Student (13)", "Student (14)", "Student (15)", "Student (16)", "Student (17)", "Student (18)", "Student (19)", "Student (20)" };
        Dictionary<string, Bot> students = new Dictionary<string, Bot>();
        foreach (string student in studentsName)
        {
            students.Add(student, GameObject.Find(student).GetComponent<Bot>());
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

        // TODO : Create Player from Participant Character ? Create directly a Player in Lobby ? Can we replace character by player instead of creation + destruction ?
        // TODO 2 : Lobby method ? method in GameManager Start ?
        // Choose a random character to be player
        System.Random rand = new System.Random();
        int indexPlayer = rand.Next(0, participants.Count);
        Character character = participants[indexPlayer];
        // Create Player
        player = character.gameObject.AddComponent<Player>();
        player.Create(character);
        //player = new Player(character);

        // Replace character reference by player
        foreach (Character participant in participants)
        {
            if ( participant.CompareTag("Bot") ) {
                Bot bot = participant as Bot;
                Assert.IsNotNull(bot);
                bot.relations.Add(player, bot.relations[character]);
                bot.relations.Remove(character);
#if DEBUG
                bot.CopyRelations();
#endif
            }
        }
        participants[indexPlayer] = player;
        // Destroy unused character
        Destroy(character);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Discussion(Character sender, Character receiver) {
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
