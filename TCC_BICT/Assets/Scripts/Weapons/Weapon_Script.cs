using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Script : MonoBehaviour
{
    [SerializeField] private Transform barrel;
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject bullet;

    private Animator weaponAnimator;
    private float fireTimer; 

    // Start is called before the first frame update
    void Start()
    {
        weaponAnimator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0) && CanShoot())
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        fireTimer = Time.time + fireRate;

        Instantiate(bullet, barrel.position, barrel.rotation);

        weaponAnimator.SetTrigger("Fire");
    }

    private bool CanShoot()
    {
        return Time.time > fireTimer;
    }
}
