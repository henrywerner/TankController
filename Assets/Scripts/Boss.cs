using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private int _damageAmount = 1;
    [SerializeField] private ParticleSystem _inpactParticles;
    [SerializeField] private AudioClip _impactSound;

    private Rigidbody _rb;

    [SerializeField] private float _bossHealth;
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

    // TODO: Make virtual
    public void Move()
    {
    }
}