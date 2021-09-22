using System;
using UnityEngine;

public abstract class Health : MonoBehaviour, IDamageable
{
    public int maxHp;
    public int currentHp;
    public event Action OnHpChanged;

    public void Awake()
    {
        currentHp = maxHp;
    }

    private void Start()
    {
        OnHpChanged?.Invoke();
    }

    public virtual void Kill()
    {
        gameObject.SetActive(false);
    }

    public virtual void Damage(int damage)
    {
        currentHp -= damage;
        OnHpChanged?.Invoke();
        if (currentHp <= 0)
            Kill();
    }
}