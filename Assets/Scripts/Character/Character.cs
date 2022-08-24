using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Character : MonoBehaviour
{
    public enum CLASSNAME { A1, B1, C1, A2, B2, C2, A3, B3, C3 };

    //public string name;
    public CLASSNAME classname;
    private AI ai;

    // Start is called before the first frame update
    void Start()
    {
        
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
