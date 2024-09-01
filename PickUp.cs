using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PickUp : MonoBehaviour
{
    public float pickupRange = 5f; // The range within which the player can pick up the gun
    public GameObject gun; // The gun object that the player can pick up
    public Transform gunHoldPosition;
    public Transform gunHoldPositionWhileRunning;
    public Transform gunHoldPositionWhileJumping;
    public Transform gunHoldPositionWhileAiming;
    public Transform gunHoldPositionWhileReloading;
    
    public Animator playerAnimator;
    public bool hasGun = false;

    // Scrip References : 
    public Animations _animationsScript;
    public Animator _animator;
    public ThirdPersonController _thirdPersonController;
    public Gun _gunScript;

    //Transitions 
    public float runTransitionSpeed = 5f;
    public float attachTransitionSpeed = 5f;
    public float jumpTransitionSpeed = 5f;
    public float aimTransitionSpeed = 5f;
    public float rigTransitionSpeed = 5f;
    public float reloadTransitionSpeed = 5f;

    public bool _isAiming = false;

    public Rig _aimRig;

    public float hasGunLayerTransitionSpeed = 1f;



    private void Start()
    {
        _animationsScript = GetComponent<Animations>();
        _animator = GetComponent<Animator>();
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _aimRig = GetComponentInChildren<Rig>();
        GameObject gun = GameObject.FindGameObjectWithTag("itsGun");
        if (gun != null)
        {
            Debug.Log("Gun Found");
            _gunScript = gun.GetComponent<Gun>();
            if (_gunScript == null)
            {
                Debug.LogError("Gun component not found on Gun object.");
            }
        }
        else
        {
            Debug.LogError("Gun object not found. Please assign the correct tag to the gun.");
        }
    }

    void Update()
    {
        // Check if the player is pressing the pickup key (e.g., 'E')
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Calculate the distance between the player and the gun
            float distanceToGun = Vector3.Distance(transform.position, gun.transform.position);

            // If the gun is within pickup range, pick it up
            if (distanceToGun <= pickupRange && !hasGun )
            {
                playerAnimator.SetBool("PickingUp", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) && hasGun ) 
        {
            Drop();
        }
        if (hasGun && _animationsScript.broRan)
        {
            AttachGunWhileRunning();  
        }
        else if (hasGun && _animationsScript != null && !_animationsScript.broRan)
        {
            AttachGun();
        }
        if (hasGun && _thirdPersonController.broJumped)
        {
            AttachGunWhileJumping();
        }
        if (_animator.GetBool("isAiming"))
        {
            if (_aimRig != null)
            {
                _aimRig.weight = Mathf.Lerp (_aimRig.weight , 1f , rigTransitionSpeed * Time.deltaTime );
            }
            AttachGunWhileAiming ();
        }
        else
        {
            if (_aimRig != null)
            {
                _aimRig.weight = Mathf.Lerp (_aimRig.weight , 0f , rigTransitionSpeed * Time.deltaTime );
            }
        }
        if (_thirdPersonController.Grounded && _gunScript._isReloading )
        {
            AttachGunWhileReloading ();
        }
       

    }
    public void OnPickUpAnimationEnd()
    {
        playerAnimator.SetBool("PickingUp", false);
        StartCoroutine(SmoothLayerWeightTransition(1f));
    }
    public void PickUpGun()
    {
        hasGun = true;

        // Attach the gun to the player
        
        if (hasGun && !_animationsScript.broRan)
        {
            AttachGun();
        }


        // Disable the gun's Rigidbody so it moves with the player
        Rigidbody gunRigidbody = gun.GetComponent<Rigidbody>();
        if (gunRigidbody != null)
        {
            gunRigidbody.isKinematic = true;
        }

        // Optionally, enable any gun-related controls here
    }

    public void AttachGun()
    {
        gun.transform.parent = gunHoldPosition;
        //gun.transform.localPosition = Vector3.zero; // Position the gun at the origin of the player
        //gun.transform.localRotation = Quaternion.identity;

        gun.transform.localPosition = Vector3.Lerp(gun.transform.localPosition, Vector3.zero, attachTransitionSpeed * Time.deltaTime);
        gun.transform.localRotation = Quaternion.Lerp(gun.transform.localRotation, Quaternion.identity, attachTransitionSpeed * Time.deltaTime);

    }
    public void AttachGunWhileRunning()
    {
        gun.transform.parent = gunHoldPositionWhileRunning;
        //gun.transform.localPosition = Vector3.zero; // Position the gun at the origin of the player
        //gun.transform.localRotation = Quaternion.identity;

        gun.transform.localPosition = Vector3.Lerp(gun.transform.localPosition, Vector3.zero, runTransitionSpeed * Time.deltaTime);
        gun.transform.localRotation = Quaternion.Lerp(gun.transform.localRotation, Quaternion.identity, runTransitionSpeed * Time.deltaTime);

    }
    public void AttachGunWhileJumping()
    {
        gun.transform.parent = gunHoldPositionWhileJumping;
        //gun.transform.localPosition = Vector3.zero; // Position the gun at the origin of the player
        //gun.transform.localRotation = Quaternion.identity;

        gun.transform.localPosition = Vector3.Lerp(gun.transform.localPosition, Vector3.zero, jumpTransitionSpeed * Time.deltaTime);
        gun.transform.localRotation = Quaternion.Lerp(gun.transform.localRotation, Quaternion.identity, jumpTransitionSpeed * Time.deltaTime);

    }
    public void AttachGunWhileAiming()
    {
        gun.transform.parent = gunHoldPositionWhileAiming;
        gun.transform.localPosition = Vector3.Lerp(gun.transform.localPosition, Vector3.zero, aimTransitionSpeed * Time.deltaTime);
        gun.transform.localRotation = Quaternion.Lerp(gun.transform.localRotation, Quaternion.identity, aimTransitionSpeed * Time.deltaTime);
    }
    public void AttachGunWhileReloading()
    {
        gun.transform.parent = gunHoldPositionWhileReloading;
        gun.transform.localPosition = Vector3.Lerp(gun.transform.localPosition, Vector3.zero, reloadTransitionSpeed * Time.deltaTime);
        gun.transform.localRotation = Quaternion.Lerp(gun.transform.localRotation, Quaternion.identity, reloadTransitionSpeed * Time.deltaTime);
    }
    void Drop()
    {
        hasGun = false;
        StartCoroutine(SmoothLayerWeightTransition(0f));
        gun.transform.parent = null;
        Rigidbody gunRigidbody = gun.GetComponent<Rigidbody>();
        if (gunRigidbody != null)
        {
            gunRigidbody.isKinematic = false;
        }
        
    }
    private IEnumerator SmoothLayerWeightTransition(float targetWeight)
    {
        float currentWeight = playerAnimator.GetLayerWeight(playerAnimator.GetLayerIndex("HasGun"));
        float timeElapsed = 0f;

        while (timeElapsed < hasGunLayerTransitionSpeed)
        {
            timeElapsed += Time.deltaTime;
            float newWeight = Mathf.Lerp(currentWeight, targetWeight, timeElapsed / hasGunLayerTransitionSpeed);
            playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("HasGun"), newWeight);
            yield return null;
        }

        playerAnimator.SetLayerWeight(playerAnimator.GetLayerIndex("HasGun"), targetWeight);
    }
}
