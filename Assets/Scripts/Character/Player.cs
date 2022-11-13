using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : Character
{


    private CharacterController controller;

    //[SerializeField] private CharacterController controller;
    private Vector3 playerVelocity;
    static private float playerSpeed = 10.0f;
    //static private float jumpHeight = 1.0f;
    static private float gravityValue = -9.81f;

    private List<Character> nearCharacters = new List<Character>();


    // TODO Use Start istead of create ?
    public Player(Character character)
    {
        name = character.name;
        classname = character.classname;
        avatar = character.GetAvatar();
        relations = character.relations;
        dialoguing = false;
        this.tag = "Player";
        controller = this.gameObject.AddComponent<CharacterController>();
        nearCharacters = new List<Character>();
    }

    public void Create(Character character)
    {
        name = character.name;
        classname = character.classname;
        avatar = character.GetAvatar();
        relations = character.relations;
        dialoguing = false;
        this.tag = "Player";
        controller = this.gameObject.AddComponent<CharacterController>();
        nearCharacters = new List<Character>();
    }

    private void Start()
    {

    }

    protected override void Update()
    {
        base.Update();
        if ( dialoguing ) {
            controller.Move(Vector3.zero);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //gameUI.NextDialogueBox();
                //state = gameUI.Dialoguing() ? STATE.DIALOGUE : STATE.RUNNING;
                // TODO : gameUI update player dialoging
                dialoguing = GameManager.Instance.ui.IsDialoguing();
            }
        } else
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (nearCharacters.Count > 0)
                {
                    dialoguing = true;
                    Character interlocutor = FindNearestCharacter();

                    // TODO : method to create specific game dialogueBox "Hors narration"
                    GameManager.Instance.game.Discussion(this, interlocutor);

                    GameManager.Instance.ui.ActivateDialogueBox();
                    GameManager.Instance.ui.ExecuteCurrentMessage();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.gameObject != this.gameObject)
        {
            //Debug.Log(name + " triggered Enter with " + other.gameObject.name);
            // TODO : check with Character childs (Bots,Player)
            nearCharacters.Add(other.GetComponent<Character>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            //Debug.Log(name + " triggered Exit with " + other.gameObject.name);
            // TODO : check with Character childs (Bots,Player)
            nearCharacters.Remove(other.GetComponent<Character>());
        }

    }

    private void Move()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        /*if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }*/

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private Character FindNearestCharacter()
    {
        Assert.IsTrue(nearCharacters.Count > 0);
        Character interlocutor = nearCharacters[0];
        float angle = Vector3.Angle(transform.forward, interlocutor.transform.position - transform.position);
        //Debug.Log("Angle between " + this.name + " and " + interlocutor.name + " is " + angle + "°");
        for (int i = 1; i < nearCharacters.Count; ++i)
        {
            float newAngle = Vector3.Angle(transform.forward, nearCharacters[i].transform.position - transform.position);
            //Debug.Log("Angle between " + this.name + " and " + nearCharacters[i].name + " is " + newAngle + "°");
            if (newAngle < angle)
            {
                angle = newAngle;
                interlocutor = nearCharacters[i];
            }
        }
        return interlocutor;
    }

    public void UpdateRemovedParticipant(Character participant)
    {
        if (nearCharacters.Contains(participant))
        {
            nearCharacters.Remove(participant);
        }
    }

}
