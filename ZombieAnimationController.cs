using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class ZombieAnimationController : MonoBehaviour
{
    public Animator _zAnimator;
    public float idleTime = 5f;     // Time to stay in idle state
    public float walkTime = 5f;     // Time to stay in walk state
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float groundCheckDistance = 10f;
    public float maxDistance = 50f;
    public Transform referencePoint; // from where the maxDistance is Calculated

    private Vector3 walkDirection;
    private Vector3 directionToPlayer;

    // Player Detection 
    public Transform player;
    public float detectionRadius = 1f;
    public float fieldOfViewAngle = 90f;
    public float fieldOfViewDistance = 10f;
    // Chasing 
    public bool zIsAngry = false;
    public float minChaseDistance = 1f;
    private float groundLevel;
    // Health and Damage 
    public int Health = 100;
    public int attackDamage = 1;
    private PlayerHealth playerHealth;

    private void Start()
    {
        GameObject theplayer = GameObject.FindGameObjectWithTag("Player");
        if (theplayer != null)
        {
            playerHealth = theplayer.GetComponent<PlayerHealth>();
        }
        _zAnimator = GetComponent<Animator>();
        groundLevel = transform.position.y;
        StartCoroutine(AnimateZombie());
    }

    private IEnumerator AnimateZombie()
    {
        while (!zIsAngry)
        {
            // Set the animator to Idle state
            _zAnimator.SetBool("Z_Walking", false);
            yield return new WaitForSeconds(idleTime);

            walkDirection = new Vector3 (Random.Range(-1f, 1f),0, Random.Range (-1f, 1f)).normalized;

            // Set the animator to Walk state
            _zAnimator.SetBool("Z_Walking", true);
            yield return new WaitForSeconds(walkTime);
        }
    }
    private void Update()
    {
        if (_zAnimator.GetBool("Z_Walking"))
        {
            if (!IsGroundInFront())
            {
                walkDirection = new Vector3 (Random.Range (-1f , 1f),0, Random.Range (-1f, 1f) ).normalized;
            }
            transform.position += walkDirection * walkSpeed * Time.deltaTime;
            // Rotate the zombie to face the walk direction
            if (walkDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(walkDirection, Vector3.up );
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime  );
            }
            if (Vector3.Distance(transform.position, referencePoint.position) > maxDistance)
            {
                Destroy (gameObject );
            }
        }
        if (IsPlayerDetected() || IsPlayerSeen())
        {
            zIsAngry = true;
        }
        if (zIsAngry)
        {
            ZChase();
        }
        if (Health <= 0)
        {
            ZDie();
        }
    }
    
    private bool IsGroundInFront()
    {
        Vector3 origin = transform.position + walkDirection * 0.5f + Vector3.up * 0.5f;
        RaycastHit hit ;
        bool isGroundDetected = Physics.Raycast(origin, Vector3.down, out hit, groundCheckDistance);
        return isGroundDetected;

    }
    private bool IsPlayerDetected()
    {
        // distance Detection
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            Quaternion toRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime);
            _zAnimator.SetBool("Z_Attacking", true);
            return true;
        }
        return false;
    }
    private bool IsPlayerSeen()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle (transform.forward , directionToPlayer);
        if (Vector3.Distance(transform.position, player.position) <= fieldOfViewDistance)
        {
            if (angleToPlayer <= fieldOfViewAngle / 2)
            {
                return true;
            }
        }
        return false;
    }
    private void ZChase()
    {
        directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;

        if (Vector3.Distance(transform.position, player.position) >= minChaseDistance)
        {
            Quaternion toRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime);

            Vector3 newPosition = transform.position + directionToPlayer * runSpeed * Time.deltaTime;
            newPosition.y = groundLevel; // Keep y-position constant
            transform.position = newPosition;

            _zAnimator.SetBool("Z_Attacking", false );
            _zAnimator.SetBool("Z_Running", true);
        }
        else
        {
            Quaternion toRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime);
            _zAnimator.SetBool("Z_Attacking", true);
        }
    }
    public void ZTakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            ZDie();
        }
    }
    private void ZDie()
    {
        _zAnimator.SetTrigger("Z_Death");
        this.enabled = false;
        Destroy(gameObject, 2f);
    }
    private void AttackPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("yeehh leee");
        }
    }
}