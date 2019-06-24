using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemies : MonoBehaviour
{
    
    //Count how many enemies are nearby
    private int _enemiesNerby;

    //The character parent
    private GameObject _parent;
    
    //Set the parent
    void Start()
    {
        _parent = transform.parent.gameObject;
    }

    //Check how many enemies are nearby
    void Update()
    {
        if (_enemiesNerby >= 2)
        {
            StartRun(_parent.GetComponent<AIMovement>());
        }
    }
    
    //Increase the enemies nearby
    public void OnTriggerEnter(Collider other)
    {
        if (_parent.transform.parent.name == "BlueTeam" && other.transform.parent.name == "RedTeam")
        {
            _enemiesNerby++;
        }
        
        if (_parent.transform.parent.name == "RedTeam" && other.transform.parent.name == "BlueTeam")
        {
            _enemiesNerby++;
        }
    }
    
    //Substract the enemies nearby
    public void OnTriggerExit(Collider other)
    {
        if (_parent.transform.parent.name == "BlueTeam" && other.transform.parent.name == "RedTeam")
        {
            _enemiesNerby--;
        }
        
        if (_parent.transform.parent.name == "RedTeam" && other.transform.parent.name == "BlueTeam")
        {
            _enemiesNerby--;
        }
    }
    
    //Set the parent in run
    public void StartRun(AIMovement run)
    {
        run.StartToRun();
        _enemiesNerby = 0;
    }
}
