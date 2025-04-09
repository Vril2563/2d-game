using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Entity : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected int maxHealth;
    [SerializeField] protected int currentHealth;
    [SerializeField] protected float maxMoveSpeed;
    [SerializeField] protected float currentMoveSpeed;
    [SerializeField] protected float range;
    
    [Header("Functional Options")]
    [SerializeField] protected bool canMove = true;
    [SerializeField] protected bool canAttack = true;

    protected void Start()
    {
        currentHealth = maxHealth;
        currentMoveSpeed = maxMoveSpeed;
    }

    public int GetHealth()
    {
        return currentHealth;
    }
        
    public float GetMaxMoveSpeed()
    {
        return maxMoveSpeed;
    }
    
    public float GetCurrentMoveSpeed()
    {
        return currentMoveSpeed;
    }
        
    public float GetRange()
    {
        return range;
    }
    
    public void SetHealth(int health)
    {
        currentHealth = health;
    }
    
    public void SetCurrentMoveSpeed(float speed)
    {
        currentMoveSpeed = speed;
    }
    
    public void SetRange(float newRange)
    {
        range = newRange;
    }
    
    public void SetCanAttack(bool canAttack)
    {
        this.canAttack = canAttack;
    }
    
    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
}
