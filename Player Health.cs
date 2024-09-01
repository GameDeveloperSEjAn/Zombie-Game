using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private bool isDead = false;
    public GameManager _manager;
    private Animator animator;
    public int maxHealth = 100;
    public int currentHealth;
    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("n oo its painful ");
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead )
        {
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("player died game over");
        animator.SetTrigger("Death");
        _manager.GameOver();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

}
