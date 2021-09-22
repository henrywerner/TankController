using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    private int _bossPattern= 1;
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

    private void FixedUpdate()
    {
        // rotate the funny cube
        Quaternion turnOffset = Quaternion.Euler(1f, 0, 1);
        _cube.transform.localRotation = _cube.transform.localRotation * turnOffset;

        // TODO: have some sort of check that changes the phase based off boss health

        if (!executingAction) // this code SUCKS
        {
            int prevPattern = _bossPattern;
            while (prevPattern == _bossPattern)
            {
                _bossPattern = Random.Range(1, 4);
            }

            switch (_bossPattern)
            {
                case 1:
                    StartCoroutine(BasicWavePattern());
                    break;
                case 2:
                    StartCoroutine(CoolPattern());
                    break;
                default:
                    StartCoroutine(WarpBurstPattern());
                    break;
            }
        }
        
        //Move();
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

    /* Boss movement for lerping between two spots*/
    private IEnumerator Move(Vector3 start, Vector3 destination, float duration)
    {
        float time = 0;
        duration += Time.deltaTime;
        
        while (time <= duration)
        {
        
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            transform.position = Vector3.Lerp(start, destination, t);
            time += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        
        _movementProgress = 0f;
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
    
    private void BurstAttack(float rotation)
    {
        float startingRotation = rotation;
        int totalBullets = 7;
        float attackSpread = 23;
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

    IEnumerator BasicWavePattern()
    {
        executingAction = true;

        // Find new destination from within the pool of movement locations
        if (_currentDestination + 1 >= _movementLocations.Length)
            _currentDestination = 0;
        else
            _currentDestination++;
        
        GameObject currentDestination = _movementLocations[_currentDestination];

        // Wait for boss to move to destination
        yield return StartCoroutine(Move(_rb.position, currentDestination.transform.position, 2f));

        // Attack
        for (int x = 0; x < 8; x++)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            Attack(90f, 30, 360f);
        }

        executingAction = false;
    }
    
    IEnumerator CoolPattern()
    {
        executingAction = true;
        
        yield return new WaitForSecondsRealtime(1f);
        
        TankController player = FindObjectOfType<TankController>();
        
        // Wait until boss has moved to player pos
        yield return StartCoroutine(Move(_rb.transform.position, player.transform.position, 3f));
        
        for (int x = 0; x < 10; x++)
        {
            yield return new WaitForSecondsRealtime(0.6f);
            Attack(90f + (x % 2 == 0 ? 0f : 7.5f), 24, 360f);
        }

        executingAction = false;
    }
    
    IEnumerator WarpBurstPattern()
    {
        executingAction = true;

        for (int z = 0; z < 3; z++)
        {

            // Find new destination from within pool of warp locations
            if (_currentDestination + 1 >= _warpLocations.Length)
                _currentDestination = 0;
            else
                _currentDestination++;

            GameObject currentDestination = _warpLocations[_currentDestination];

            // Wait for warp movement to complete
            yield return StartCoroutine(WarpTo(currentDestination.transform.position));

            yield return new WaitForSecondsRealtime(2f);

            TankController player = FindObjectOfType<TankController>(); // Find player

            for (int y = 0; y < 1; y++)
            {
                Vector3 direction =
                    (player.transform.position - transform.position).normalized; // Get direction facing player
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                float startingRotation = lookRotation.eulerAngles.y;

                for (int x = 0; x < 3; x++)
                {
                    BurstAttack(startingRotation);
                    yield return new WaitForSecondsRealtime(0.1f);
                }

                yield return new WaitForSecondsRealtime(0.7f);
            }
        }
        
        yield return new WaitForSecondsRealtime(2f);

        executingAction = false;
    }
}