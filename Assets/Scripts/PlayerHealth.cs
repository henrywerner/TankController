using UnityEngine;
using System.Collections;

public class PlayerHealth : Health
{
    //[SerializeField] private Player _player;
    [SerializeField] private AudioClip _deathNoise;
    [SerializeField] private ParticleSystem _deathVFX;
    [SerializeField] private Material[] _materials;
    [SerializeField] private GameObject[] _hudHearts;
    private bool _isInvincible;

    private void Start()
    {
        UpdateHud();
    }
    
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

        currentHp -= damage;
        UpdateHud();
        if (currentHp <= 0)
            Kill();
        StartCoroutine(Iframes());
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

    private void UpdateHud()
    {
        if (_hudHearts.Length < maxHp)
            print("Warning: total hudHearts is less than max health.");

        int x = 0;
        foreach (var heart in _hudHearts)
        {
            x++;
            if (x > currentHp)
            {
                heart.SetActive(false);
                continue;
            }

            heart.SetActive(true);
        }
    }
}