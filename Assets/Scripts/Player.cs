using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class Player : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private AudioClip _deathNoise;
    private int _currentHealth;

    private TankController _tankController;
    private InventoryManager _inventory;

    private void Awake()
    {
        _tankController = GetComponent<TankController>();
        _inventory = GetComponent<InventoryManager>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        _inventory.UpdateHud();
    }

    public void IncreaseHealth(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        Debug.Log("Player's health " + _currentHealth);
    }

    public void DecreaseHealth(int amount)
    {
        _currentHealth -= amount;
        Debug.Log("Player's health " + _currentHealth);
        if (_currentHealth <= 0)
            Kill();
    }

    public void Kill()
    {
        gameObject.SetActive(false);
        // play particles
        
        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }
}
