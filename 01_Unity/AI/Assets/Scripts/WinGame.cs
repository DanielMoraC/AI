using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGame : MonoBehaviour
{
    
    //Teams
    public GameObject TeamBlue;
    public GameObject TeamRed;

    //When ever a team is destroyed print who won
    void Update()
    {
        if (TeamBlue.transform.childCount == 0)
        {
            print("Red Team Wins");
        }
        else if (TeamRed.transform.childCount == 0)
        {
            print("Blue Team Wins");
        }
    }
}
