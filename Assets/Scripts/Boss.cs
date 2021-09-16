using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] private int _contactDamage = 1;
    [SerializeField] private ParticleSystem _inpactParticles;
    [SerializeField] private AudioClip _impactSound;
    private Rigidbody _rb;
    
    [Header("Assets")]
    [SerializeField] private GameObject _cube;
    [SerializeField] private GameObject _bullet;

    [Header("Warp")]
    [SerializeField] private GameObject _warpIndicator;
    [SerializeField] private GameObject[] _warpLocations;
    
    [SerializeField] private GameObject[] _movementLocations;
    private int _currentDestination = 0;
    private Vector3 _movementStartPos;
    private float _movementProgress = 0f;
    private int _bossPhase= 1;
    private bool executingAction = false;

    [SerializeField] private Color _objectColor;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _warpIndicator.SetActive(false);
    }

    public void Start()
    {
        _movementStartPos = _rb.position;
        
        // Set all movement location nodes to the boss' height
        foreach (var node in _movementLocations)
        {
            var position = node.gameObject.transform.position;
            Vector3 newPos = new Vector3(position.x, _rb.position.y, position.z);
            node.gameObject.transform.position = newPos;
        }
    }

    /* Damage player on contact */
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

    private void FixedUpdate()
    {
        // rotate the funny cube
        Quaternion turnOffset = Quaternion.Euler(1f, 0, 1);
        _cube.transform.localRotation = _cube.transform.localRotation * turnOffset;

        // TODO: have some sort of check that changes the phase based off boss health

        if (!executingAction) // this code SUCKS
        {
            switch (_bossPhase)
            {
                case 1:
                    StartCoroutine(MoveLoop());
                    break;
                case 2:
                    StartCoroutine(MoveLoop());
                    break;
                case 3:
                    StartCoroutine(MoveLoop());
                    break;
                case 4:
                    StartCoroutine(MoveLoop());
                    break;
                default:
                    StartCoroutine(TeleportLoop());
                    break;
            }
        }
        
        //Move();
    }

    private IEnumerator MoveLoop()
    {
        executingAction = true;
        
        GameObject currentDestination = _movementLocations[_currentDestination];
        
        while (_movementProgress <= 1f)
        {
            _movementProgress += 1f * Time.deltaTime;
            _rb.position = Vector3.Lerp(_movementStartPos, currentDestination.transform.position, _movementProgress);

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(WavePattern());
        yield return new WaitForSecondsRealtime(6f);
        
        if (_currentDestination + 1 >= _movementLocations.Length)
            _currentDestination = 0;
        else
            _currentDestination++;

        _movementStartPos = _rb.position;
        _movementProgress = 0f;

        _bossPhase++;
        executingAction = false;
    }

    private IEnumerator TeleportLoop()
    {
        executingAction = true;
        
        if (_currentDestination + 1 >= _warpLocations.Length)
            _currentDestination = 0;
        else
            _currentDestination++;
        
        GameObject currentDestination = _warpLocations[_currentDestination];
        
        // This is a last min addition and not the final version please don't be mad
        // I promise I can code a lot better than this :(
        StartCoroutine(WarpTo(currentDestination.transform.position));
        yield return new WaitForSecondsRealtime(3f);

        // nightmare nightmare nightmare nightmare nightmare nightmare nightmare 
        StartCoroutine(BurstPattern());
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(BurstPattern());
        yield return new WaitForSecondsRealtime(1f);
        StartCoroutine(BurstPattern());
        yield return new WaitForSecondsRealtime(1f);
        
        _bossPhase++;
        executingAction = false;
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

    /*
    private void Move()
    {
        GameObject currentDestination = _movementLocations[_currentDestination];

        _movementProgress += 0.01f;
        
        _rb.position = Vector3.Lerp(_movementStartPos, currentDestination.transform.position, _movementProgress);

        if (_rb.position == currentDestination.transform.position)
        {
            if (_currentDestination + 1 >= _movementLocations.Length)
                _currentDestination = 0;
            else
                _currentDestination++;

            _movementStartPos = _rb.position;
            _movementProgress = 0f;
        }
    }
    */

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
            Attack(90f, 30, 360f);
            //WaveAttack();
        }
    }
    
    IEnumerator BurstPattern()
    {
        TankController player = FindObjectOfType<TankController>();
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