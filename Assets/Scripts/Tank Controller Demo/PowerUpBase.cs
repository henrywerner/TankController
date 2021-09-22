using System.Collections;
using UnityEngine;

public abstract class PowerUpBase : MonoBehaviour
{
    [SerializeField] private ParticleSystem _collectParticles;
    [SerializeField] private AudioClip _collectSound;
    [SerializeField] private float _effectDurration;
    
    private void TriggerEffect(Player player)
    {
        PowerUp(player);
        StartCoroutine(PauseForEffect(player));
    }

    IEnumerator PauseForEffect(Player player)
    {
        yield return new WaitForSecondsRealtime(_effectDurration);
        PowerDown(player);
        gameObject.SetActive(false);
    }

    protected abstract void PowerUp(Player player);

    protected abstract void PowerDown(Player player);


    private void OnTriggerEnter(Collider other)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
        {
            // TODO: optimize
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            
            TriggerEffect(player);
            Feedback(); // spawn particles and sfx
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