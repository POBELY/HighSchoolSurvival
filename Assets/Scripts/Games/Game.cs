using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] protected List<Character> participants;
    //protected Player player;
    protected Character player;

    // Start is called before the first frame update
    public virtual void Start()
    {
       
        System.Random rand = new System.Random();
        int indexPlayer = rand.Next(0, participants.Count);

        player = participants[indexPlayer];
        CharacterController controller = player.gameObject.AddComponent<CharacterController>();
        player.gameObject.AddComponent<PlayerController>().controller = controller;
        Debug.Log(indexPlayer);
        //player = Player(participants[indexPlayer]);
        
        player.name = "Player";
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Character GetParticipantByIndex(int index)
    {
        return participants[index];
    }
    public Character GetRandomParticipant()
    {
        System.Random rand = new System.Random();
        return participants[rand.Next(0,participants.Count)];
    }
}
