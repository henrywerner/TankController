using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TankController))]
public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private AudioClip _deathNoise;
    [SerializeField] private Light _effectLight;
    [SerializeField] private Material[] _materials;
    private Dictionary<Material, Color> _materialColors;
    [SerializeField] private GameObject[] _hudHearts;

    private int _currentHealth;

    public bool IsInvincible { get; set; }

    private TankController _tankController;
    private InventoryManager _inventory;
    private static readonly int Color1 = Shader.PropertyToID("_Color");

    private void Awake()
    {
        _tankController = GetComponent<TankController>();
        _inventory = GetComponent<InventoryManager>();
    }

    private void Start()
    {
        _currentHealth = _maxHealth;
        _inventory.UpdateHud();

        _materialColors = new Dictionary<Material, Color>();
        foreach (var mat in _materials)
        {
            _materialColors.Add(mat, mat.color);
        }

        UpdateHud();
    }

    public void IncreaseHealth(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        Debug.Log("Player's health " + _currentHealth);
    }

    public void Damage(int amount)
    {
        if (IsInvincible) return; // Ignore if the player is invincible

        _currentHealth -= amount;
        UpdateHud();
        if (_currentHealth <= 0)
            Kill();
    }

    private void UpdateHud()
    {
        if (_hudHearts.Length < _maxHealth)
            print("Warning: total hudHearts is less than max health.");

        int x = 0;
        foreach (var heart in _hudHearts)
        {
            x++;
            if (x > _currentHealth)
            {
                heart.SetActive(false);
                continue;
            }

            heart.SetActive(true);
        }
    }

    public void Kill()
    {
        if (IsInvincible) return; // Ignore if the player is invincible

        gameObject.SetActive(false);
        // play particles

        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }

    public void EnableLight(bool on)
    {
        _effectLight.enabled = on;
    }

    public void SetColor(Color c)
    {
        foreach (var mat in _materials)
        {
            mat.SetColor(Color1, c);
        }
    }

    public void ResetColor()
    {
        foreach (var mat in _materials)
        {
            Color c = _materialColors[mat];
            mat.SetColor(Color1, c);
        }
    }
}