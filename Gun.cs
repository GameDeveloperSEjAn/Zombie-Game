using StarterAssets;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{

    // Ammo Variables
    public int _currentAmmo;
    public int _maxAmmo = 60;
    public float _reloadTime = 2f;
    public bool _isReloading = false;


    public ParticleSystem _muzzleFlash;
    public GameObject muzzleFlashLight;
    public GameObject sphere; // Sphere tagged as "MuzzleFlashLight"
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.1f;
    public PickUp _pickup;
    public StarterAssetsInputs _inputs;
    public ThirdPersonController _controller;
    public bool isFiring = false;
    private Coroutine fireCoroutine;

    // Add bullet size variable
    public Vector3 bulletSize = new Vector3(1f, 1f, 1f);

    void Start()
    {
        muzzleFlashLight = GameObject.FindWithTag("MuzzleFlashLight");
        if (muzzleFlashLight != null)
        {
            Debug.Log("MuzzleFlashLight Found");
            muzzleFlashLight.SetActive(false); // Ensure it's initially off
        }
        else
        {
            Debug.LogError("MuzzleFlashLight not found. Please assign the correct tag to the light.");
        }

        sphere = GameObject.FindWithTag("Sphere");
        if (sphere != null)
        {
            Debug.Log("Sphere Found");
            sphere.SetActive(false); // Ensure it's initially off
        }
        else
        {
            Debug.LogError("Sphere not found. Please assign the correct tag to the sphere.");
        }
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Stop(); // Ensure the particle system is initially stopped
        }
        else
        {
            Debug.LogError("Particle system not assigned. Please assign the muzzle flash particle system.");
        }

        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        if (_player != null)
        {
            _pickup = _player.GetComponent<PickUp>();
            _inputs = _player.GetComponent<StarterAssetsInputs>();
            _controller = _player.GetComponent <ThirdPersonController>();

            if (_pickup == null)
            {
                Debug.LogError("PickUp component not found on Player object.");
            }

            if (_inputs == null)
            {
                Debug.LogError("StarterAssetsInputs component not found on Player object.");
            }
        }
        else
        {
            Debug.LogError("Player not found. Please assign the correct tag to the player.");
        }
        _currentAmmo = _maxAmmo;
    }
    void Update()
    {
        if (!_inputs.aim && !isFiring && _pickup.hasGun && Input.GetKeyDown (KeyCode.R ) && _currentAmmo != _maxAmmo && _controller.Grounded )
        {
            _isReloading = true;
            StartCoroutine (Reload ());
        }
        if (_isReloading )
        {
            return;
        }
        if (_currentAmmo <= 0)
        {
            Debug.Log("press R to Reload");
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }
            return;
        }
        if (Input.GetButtonDown("Fire1") && _inputs.aim && !isFiring && _pickup.hasGun)
        {
            isFiring = true;
            fireCoroutine = StartCoroutine(FireContinuously());
        }
        if (Input.GetButtonUp("Fire1") ||!_inputs.aim || !_pickup.hasGun)
        {
            isFiring = false;

            if (_muzzleFlash != null)
            {
                _muzzleFlash.Stop(); // Ensure the particle system is stopped when not firing
            }
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
            }
            if (muzzleFlashLight != null)
            {
                muzzleFlashLight.SetActive(false); // Ensure the light is off when not firing
            }
            if (sphere != null)
            {
                sphere.SetActive(false); // Ensure the sphere is off when not firing
            }
        }
    }

    private IEnumerator FireContinuously()
    {
        while (isFiring)
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }
    }

    void Fire()
    {
        if (_currentAmmo <= 0)
        {
            return;
        }
        _currentAmmo--;
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(true); // Turn on the muzzle flash light
        }
        if (sphere != null)
        {
            sphere.SetActive(true); // Turn on the sphere
        }
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Play();
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bullet.transform.localScale = bulletSize; // Set bullet size
        bulletScript.InitializeBullet(firePoint.forward, bulletSize);

        // Turn off the lights after a short delay
        StartCoroutine(TurnOffLights());
    }

    private IEnumerator TurnOffLights()
    {
        yield return new WaitForSeconds(0.05f); // Adjust the duration to match the muzzle flash effect
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(false); // Turn off the muzzle flash light
        }
        if (sphere != null)
        {
            sphere.SetActive(false); // Turn off the sphere
        }
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Stop();
        }
      
    }
    private IEnumerator Reload()
    {
        _isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds (_reloadTime);
        _currentAmmo = _maxAmmo;
        _isReloading = false;
        Debug.Log("Reloaded");
    }
}
