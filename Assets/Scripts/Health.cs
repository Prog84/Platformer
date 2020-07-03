﻿using System.Collections;
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int health;
    public Action<int, GameObject> OnTakeHit;

    public int CurrentHealth { 
        get { return health; } 
    }

    private void Start()
    {
        GameManager.Instance.healthContainer.Add(gameObject, this);
    }
    public void TakeHit(int damage, GameObject attaker)
    {
        health -= damage;
        if (OnTakeHit != null)
            OnTakeHit(damage, attaker);
        if (health <= 0)
            Destroy(gameObject);
    }
    public void SetHealth(int bonusHealth)
    {
        health += bonusHealth;
        /*Debug.Log("Здоровье " + health);
        if (health > 100)
            health = 100;*/
    }
}
