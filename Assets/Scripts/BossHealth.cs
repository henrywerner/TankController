using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BossHealth : Health
{
    [SerializeField] private Boss _boss;
    [SerializeField] private AudioClip _deathNoise;

    public override void Kill()
    {
        gameObject.SetActive(false);
        
        // play particles
        // TODO: add particles 
        
        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }
}