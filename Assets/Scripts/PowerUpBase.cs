using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    protected abstract void PowerUp(Player player);
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            PowerUp(player);
           Feedback(); // spawn particles and sfx
            gameObject.SetActive(false);
        }
    }
    
    private void Feedback()
    {
        // particles
        if (_collectParticles != null)
        {
            _collectParticles = Instantiate(_collectParticles, transform.position, Quaternion.identity);
        }

        if (_collectSound != null)
        {
            AudioHelper.PlayClip2D(_collectSound, 1f);
        }
    }
}