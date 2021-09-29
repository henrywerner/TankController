using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UI : MonoBehaviour
{
    [Header("Health Scripts")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private BossHealth _bossHealth;

    [Header("Player HealthBar")]
    [SerializeField] private GameObject[] _playerHearts;
    [SerializeField] private Image _redScreen;
    
    [Header("Boss HealthBar")]
    [SerializeField] private Image _bossHealthBar;
    [SerializeField] private Image _bossHurtBar;
    private bool _bossHpIsLerping = false;

    private void Start()
    {
        _playerHealth.OnHpChanged += UpdateHealthPlayer;
        _playerHealth.OnPlayerHurt += () => StartCoroutine(RedScreen());
        _bossHealth.OnHpChanged += UpdateHealthBoss;
        
        _redScreen.color = new Color(1, 1, 1, 0);
    }
    
    private void UpdateHealthPlayer()
    {
        if (_playerHearts.Length < _playerHealth.maxHp)
            print("Warning: total hudHearts is less than max health.");

        int x = 0;
        foreach (var heart in _playerHearts)
        {
            x++;
            if (x > _playerHealth.currentHp)
            {
                heart.SetActive(false);
                continue;
            }

            heart.SetActive(true);
        }
    }

    private void UpdateHealthBoss()
    {
        float scaleX = (float) _bossHealth.currentHp / _bossHealth.maxHp;
        _bossHealthBar.rectTransform.localScale = new Vector3(scaleX, 1, 1);

        if (!_bossHpIsLerping)
            StartCoroutine(UpdateHurtBar());
    }

    /* Red vignette animation and screen shake */
    IEnumerator RedScreen()
    {
        StartCoroutine(CameraShake.current.Shake(0.4f, 0.4f)); // Screen shake
        
        float time = 0;
        float duration = 0.8f;
        float alpha = 0f;
        
        _redScreen.color = new Color(1, 1, 1, 0);

        while (time < duration)
        {
            alpha = (1 - Math.Abs(Mathf.Lerp(-1, 1, time / duration))) * 0.6f;
            _redScreen.color = new Color(1, 1, 1, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        _redScreen.color = new Color(1, 1, 1, 0);
    }

    IEnumerator UpdateHurtBar()
    {
        _bossHpIsLerping = true;
        float timeElapsed = 0;
        float lerpDuration = 3f;

        //yield return new WaitForSecondsRealtime(0.5f);

        while (Math.Abs(_bossHurtBar.rectTransform.localScale.x - _bossHealthBar.rectTransform.localScale.x) > 0.0001) // done to avoid floating point errors
        {
            float hurtBarX = _bossHurtBar.rectTransform.localScale.x;
            float healthBarX = _bossHealthBar.rectTransform.localScale.x;
            
            float scaleX = Mathf.Lerp(hurtBarX, healthBarX, timeElapsed / lerpDuration);
            _bossHurtBar.rectTransform.localScale = new Vector3(scaleX, 1, 1);
            timeElapsed += Time.deltaTime;

            yield return null;
        }
        
        _bossHpIsLerping = false;
    }
}
