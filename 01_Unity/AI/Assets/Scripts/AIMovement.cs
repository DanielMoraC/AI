using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour
{
    
    public float duration;    //the max time of a walking session (set to ten)
    //[SerializeField]
    float elapsedTime   = 0f; //time since started walk
    //[SerializeField]
    float wait          = 0f; //wait this much time
    //[SerializeField]
    float waitTime      = 0f; //waited this much time

    private bool doneWait = false;

    float randomX;  //randomly go this X direction
    float randomZ;  //randomly go this Z direction

    bool move = true; //start moving

    void Start(){
        randomX =  Random.Range(-3,3);
        randomZ = Random.Range(-3,3);
    }

    void Update ()
    {
        

        if (elapsedTime < duration && move) {
            //if its moving and didn't move too much
            transform.Translate (new Vector3(randomX,0,randomZ) * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            doneWait = false;
        } else if(!doneWait)
        {
            //do not move and start waiting for random time
            move = false;
            wait = Random.Range (2, 5);
            waitTime = 0f;
            doneWait = true;
        }

        if (waitTime < wait && !move) {
            //you are waiting
            waitTime += Time.deltaTime;
        } else if(!move)
        {
            //done waiting. Move to these random directions
            wait = 0f;
            waitTime = 0f;
            move = true;
            elapsedTime = 0f;
            randomX = Random.Range(-3,3);
            randomZ = Random.Range(-3,3);
        }

    }
    
}
