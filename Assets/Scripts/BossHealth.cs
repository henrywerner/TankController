using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BossHealth : Health
{
    [SerializeField] private Boss _boss;
    [SerializeField] private AudioClip _deathNoise;
    
    [Header("Health")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _hurtBar;
    private bool _isLerping = false;

    public override void Kill()
    {
        gameObject.SetActive(false);
        
        // play particles
        // TODO: add particles 
        
        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }

    // The boss needs it's own health script because it needs a way to call to update its Health Bar
    public override void Damage(int damage)
    {
        currentHp -= damage;
        UpdateHud();
        if (currentHp <= 0)
            Kill();
    }
    
    public void UpdateHud()
    {
        float scaleX = (float)currentHp / maxHp;
        _healthBar.rectTransform.localScale = new Vector3(scaleX, 1, 1);

        if (!_isLerping)
            StartCoroutine(UpdateHurtBar());
    }

    IEnumerator UpdateHurtBar()
    {
        _isLerping = true;
        float timeElapsed = 0;
        float lerpDuration = 3f;

        //yield return new WaitForSecondsRealtime(0.5f);

        while (Math.Abs(_hurtBar.rectTransform.localScale.x - _healthBar.rectTransform.localScale.x) > 0.0001) // done to avoid floating point errors
        {
            float hurtBarX = _hurtBar.rectTransform.localScale.x;
            float healthBarX = _healthBar.rectTransform.localScale.x;
            
            float scaleX = Mathf.Lerp(hurtBarX, healthBarX, timeElapsed / lerpDuration);
            _hurtBar.rectTransform.localScale = new Vector3(scaleX, 1, 1);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        
        _isLerping = false;
    }
}