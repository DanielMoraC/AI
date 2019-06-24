using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // Health
    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    //Set current health to max health
    void Awake ()
    {
        currentHealth = maxHealth;
    }

    //Damage the character
    public void TakeDamage (int damage)
    {
        //Damage the character
        currentHealth -= damage;

        //If health reaches zero
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //Destroy the character
    public virtual void Die ()
    {
        Destroy(gameObject);
    }

}
