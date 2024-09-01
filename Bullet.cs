using UnityEngine;
public class Bullet : MonoBehaviour 
{ 
    public float speed = 20f; 
    public float damage = 10f; 
    public float bulletRange = 10f; 
    private Vector3 startPosition; 
    private bool isFired = false;
    public float knockBackForce = 5f;
    public void InitializeBullet(Vector3 direction, Vector3 size) 
    {
        startPosition = transform.position; 
        transform.localScale = size; // Set the bullet size
        GetComponent<Rigidbody>().velocity = direction * speed; 
        isFired = true; 
    }
    void Update()
    {
        if (isFired)
        {
            float distanceTraveled = Vector3.Distance(startPosition, transform.position);
            if (distanceTraveled >= bulletRange)
            {
                Destroy(gameObject);
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, GetComponent<Rigidbody>().velocity.normalized, out hit, speed * Time.deltaTime + 0.1f))
                {
                    Debug.Log(hit.collider.gameObject.name + " hit");

                    // Call the TakeDamage method on the hit object if it has one
                    var damageable = hit.collider.GetComponent<ZombieAnimationController>();
                    if (damageable != null)
                    {
                        damageable.ZTakeDamage((int)damage);
                    }

                    Rigidbody zombieRigidbody = hit.collider.GetComponent<Rigidbody>();
                    if (zombieRigidbody != null)
                    {
                        Vector3 knockbackDirection = (hit.point - transform.position).normalized;
                        zombieRigidbody.AddForce(knockbackDirection * knockBackForce, ForceMode.Impulse);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isFired)
        {
            Debug.Log(other.name + " hit");

            // Call the TakeDamage method on the hit object if it has one
            var damageable = other.GetComponent<ZombieAnimationController>();
            if (damageable != null)
            {
                damageable.ZTakeDamage((int)damage);
            }

            Destroy(gameObject);
        }
    }
}