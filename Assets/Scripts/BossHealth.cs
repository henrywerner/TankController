using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BossHealth : Health
{
    [SerializeField] private Boss _boss;
    [SerializeField] private AudioClip _deathNoise;
    
    // Vars for the outer cube health calculations
    private int phase1Health;
    private int outerCubeHealth;

    private new void Awake()
    {
        currentHp = maxHp;
        
        // calculate how much health each outer cube should have
        phase1Health = maxHp - maxHp / 3;
        outerCubeHealth = phase1Health / 8;
    }
    
    public override void Damage(int damage)
    {
        base.Damage(damage);
        if (currentHp == maxHp / 3)
        {
            _boss.DestroyOuterCube(0); // Destroy final outer cube
            _boss.OnLowHealth(); // trigger boss final phase
        }
        else if (currentHp < maxHp / 3)
        {
            // Show damage effect on the current outer cube
            _boss.OnDamage(-1);
        }
        else if (currentHp >= maxHp / 3)
        {
            var currentPhase1Hp = currentHp - (maxHp - phase1Health);
            var currentCube = currentPhase1Hp / outerCubeHealth;
            
            // Show damage effect on the current outer cube
            _boss.OnDamage(currentCube);

            // Destroy outer cube
            if (currentPhase1Hp % outerCubeHealth == 0)
                _boss.DestroyOuterCube(currentCube);
        }
        
        
    }

    public override void Kill()
    {
        gameObject.SetActive(false);
        
        // play particles
        // TODO: add particles 
        
        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }
}