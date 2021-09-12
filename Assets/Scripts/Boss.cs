using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private ParticleSystem _inpactParticles;
    [SerializeField] private AudioClip _impactSound;
    [SerializeField] private AudioClip _deathNoise;

    private Rigidbody _rb;
    [SerializeField] private GameObject _bullet;

    [Header("Health")]
    [SerializeField] private Image _healthBar;
    [SerializeField] private Image _hurtBar;
    [SerializeField] private float _bossHealth;
    private float _currentHealth;

    private bool _isLerping = false;
    
    
    /*
     * Needed Vars:
     * Current Phase
     * Current Health
     */
    
    /*
     * Needed Functions:
     * Movement
     * Attack -- make a virtual script called attackPattern and then make a bunch of inherited scripts for the different attacks
     */

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _currentHealth = _bossHealth;
    }

    private void OnCollisionEnter(Collision other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            PlayerImpact(player);
            ImpactFeedback();
        }
    }

    private void PlayerImpact(Player player)
    {
        player.DecreaseHealth(_damageAmount);
    }

    private void ImpactFeedback()
    {
        // particles
        if (_inpactParticles != null)
        {
            _inpactParticles = Instantiate(_inpactParticles, transform.position, Quaternion.identity);
        }

        // audio
        if (_impactSound != null)
        {
            AudioHelper.PlayClip2D(_impactSound, 1f);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(WavePattern());
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(BurstPattern());
    }

    // TODO: Make virtual
    public void Move()
    {
    }
    
    public void DecreaseHealth(int amount)
    {
        _currentHealth -= amount;
        UpdateHud();
        if (_currentHealth <= 0)
            Kill();
    }

    private void UpdateHud()
    {
        float scaleX = _currentHealth / _bossHealth;
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

    public void Kill()
    {
        gameObject.SetActive(false);
        // play particles
        
        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }

    private void WaveAttack()
    {
        float startingRotation = 90;
        int totalBullets = 20;
        float attackSpread = 360;
        float angleOffset = attackSpread / totalBullets; // find the angle between each bullet
        float spawnDistance = 2f;
        
        Vector3 spawnPos = new Vector3(_rb.transform.position.x, 0.5f, _rb.transform.position.z); // set the spawn pos to the correct height

        for (float rot = startingRotation; rot < startingRotation + attackSpread; rot += angleOffset)
        {
            GameObject bullet = Instantiate(_bullet, spawnPos, Quaternion.AngleAxis(rot, Vector3.up));
            bullet.transform.Translate(Vector3.forward * spawnDistance);
            Physics.IgnoreCollision(bullet.transform.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
    
    private void BurstAttack(float rotation)
    {
        float startingRotation = rotation;
        int totalBullets = 5;
        float attackSpread = 20;
        float angleOffset = attackSpread / totalBullets; // find the angle between each bullet
        float spawnDistance = 4f;
        startingRotation -= 2 * angleOffset;
        
        Vector3 spawnPos = new Vector3(_rb.transform.position.x, 0.5f, _rb.transform.position.z); // set the spawn pos to the correct height

        for (float rot = startingRotation; rot < startingRotation + attackSpread; rot += angleOffset)
        {
            GameObject bullet = Instantiate(_bullet, spawnPos, Quaternion.AngleAxis(rot, Vector3.up));
            bullet.transform.Translate(Vector3.forward * spawnDistance);
            Physics.IgnoreCollision(bullet.transform.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    IEnumerator WavePattern()
    {
        for (int x = 0; x < 8; x++)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            WaveAttack();
        }
    }
    
    IEnumerator BurstPattern()
    {
        Player player = FindObjectOfType<Player>();
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float startingRotation = lookRotation.eulerAngles.y;
        
        for (int x = 0; x < 5; x++)
        {
            yield return new WaitForSecondsRealtime(0.2f);
            BurstAttack(startingRotation);
        }
    }
}