using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour, IDamageable
{
    [SerializeField] private int _contactDamage = 1;
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

    [Header("Warp")]
    [SerializeField] private GameObject _warpIndicator;
    [SerializeField] private GameObject[] _warpLocations;
    
    
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
        _warpIndicator.SetActive(false);
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
        player.Damage(_contactDamage);
    }

    private void ImpactFeedback()
    {
        // particles
        if (_inpactParticles != null)
            _inpactParticles = Instantiate(_inpactParticles, transform.position, Quaternion.identity);

        // audio
        if (_impactSound != null)
            AudioHelper.PlayClip2D(_impactSound, 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(WavePattern());
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(BurstPattern());
        if (Input.GetKeyDown(KeyCode.Alpha1))
            StartCoroutine(WarpTo(_warpLocations[0].transform.position));
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(WarpTo(_warpLocations[1].transform.position));
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(WarpTo(_warpLocations[2].transform.position));
        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(WarpTo(_warpLocations[3].transform.position));
    }
    
    public void Damage(int amount)
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

    /* Called when the boss dies */
    public void Kill()
    {
        gameObject.SetActive(false);
        // play particles
        
        // play sounds
        AudioHelper.PlayClip2D(_deathNoise, 1f);
    }

    /* Boss movement for warping around the arena */
    private IEnumerator WarpTo(Vector3 pos)
    {
        // Create warp indicator
        _warpIndicator.transform.position = new Vector3(pos.x, 0.01f, pos.z);
        _warpIndicator.SetActive(true);

        // Play warp animation
        //gameObject.SetActive(false);
        for (int x = 0; x < 3; x++) // quick and dirty blinking animation
        {
            _warpIndicator.SetActive(true);
            yield return new WaitForSecondsRealtime(0.1f);
            _warpIndicator.SetActive(false);
            yield return new WaitForSecondsRealtime(0.1f);
        }

        // Warp to position
        _rb.MovePosition(new Vector3(pos.x, _rb.position.y, pos.z));

        // Complete warp animation
        gameObject.SetActive(true);
    }

    private void Attack(float startingRotation, int totalBullets, float attackSpread)
    {
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
            Attack(90f, 20, 360f);
            //WaveAttack();
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
            //Attack(startingRotation, 5, 20f);
            BurstAttack(startingRotation);
        }
    }
}