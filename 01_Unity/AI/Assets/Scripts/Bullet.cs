using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    //Speed
    Rigidbody m_Rigidbody;
    float m_Speed = 20f;
    
    //Damage to enemy
    public int _damage = 40;
    
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    //Move the bullet forward
    void Update()
    {
        m_Rigidbody.velocity = transform.forward * m_Speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        //If the bullet hit an enemy do damage
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Knight")
        {
            Attack(other.GetComponent<CharacterStats>());
            Destroy(gameObject);
        }
        
        //If the bullet hit a wall destroy the bullet
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

    //Do damage
    public void Attack(CharacterStats targetStats)
    {
        targetStats.TakeDamage(_damage);
    }
}
