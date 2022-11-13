using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    // TODO : private with getters + submanagers
    [SerializeField] public Game game;
    [SerializeField] public UI ui;
    // TODO : add player : submanager for each player ?

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO : get Canvas via gameManager/UI
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
