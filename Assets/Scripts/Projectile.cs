using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class Projectile : MonoBehaviour
{
    public GameObject explosionAnimation;
    [SerializeField]
    private float selfExplosionTimer;
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, selfExplosionTimer);
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject); //destory power cell projectile
            //reduce the tripod's health
            other.transform.GetComponent<AlienHealth>().ReduceHealth();
        } else if (other.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject); //destory power cell projectile
            Destroy(other.transform.parent.gameObject);
        }
    }

    private void AnimateExplosion()
    {
        //instantiate the explosion
        var explosion = Instantiate(explosionAnimation, transform.position, transform.rotation);
        Destroy(explosion, 16.0f);
    }

    void OnDestroy()
    {
        AnimateExplosion();
    }
}
