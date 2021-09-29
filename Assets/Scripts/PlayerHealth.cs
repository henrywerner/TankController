using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [SerializeField] private AudioClip _hurtNoise;
    [SerializeField] private AudioClip _deathNoise;
    [SerializeField] private ParticleSystem _deathVFX;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Image _redScreen;
    private bool _isInvincible;

    public event Action OnPlayerHurt;

    public override void Kill()
    {
        if (_isInvincible) return; // Ignore if the player is invincible

        gameObject.SetActive(false);
        
        // play particles
        _deathVFX = Instantiate(_deathVFX, transform.position, Quaternion.identity);
        _deathVFX.gameObject.SetActive(true);

        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }

    public override void Damage(int damage)
    {
        if (_isInvincible) return; // Ignore if the player is invincible

        base.Damage(damage);
        
        AudioHelper.PlayClip2D(_hurtNoise, 1f);
        StartCoroutine(Iframes());
        OnPlayerHurt?.Invoke();
    }

    private IEnumerator Iframes()
    {
        _isInvincible = true;
        foreach (var mat in _materials)
            mat.SetInt("shaderActive", 1);
        
        yield return new WaitForSecondsRealtime(3f);
        
        foreach (var mat in _materials)
            mat.SetInt("shaderActive", 0);
        
        _isInvincible = false;
    }
}