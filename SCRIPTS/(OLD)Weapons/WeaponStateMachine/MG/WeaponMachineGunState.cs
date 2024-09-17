using System;
using System.Collections;
using UnityEngine;
using Cinemachine;

public class WeaponMachineGunState : WeaponBaseState
{
    //Camera
    Camera playerCam;

    //Animation Controller
    Animator animController;

    //Fire point
    GameObject firePoint;

    //Weapon Stats
    float damage = 18f;
    float lastFired;
    float fireRate = 7f;
    int magCapacity = 42;
    
    //private int AmmoCount { get; set; }

    //Prefabs
    GameObject MuzzleFlashPrefab;
    GameObject BulletHitPrefab;
    GameObject BulletMissPrefab;
    GameObject weaponPrefab;

    //MachineGun active object
    GameObject MachineGunObject;
    
    //Damage Multipler 
    float dmgBoost;


    //Temp recoil settings
    Vector3 weaponBasePosition;
    Vector3 weaponRecoil;
    float elapsedTime = 0.0f;
    float duration = 0.25f;
    float currenvelocity;


    //Other
    IDamageHandler damageHandler;

    RaycastHit hit;


    public override void EnterState(WeaponStateManager weapon)
    {
        GetComponents(weapon);
        WeaponEquip(weapon);
    }
    
    private void GetComponents(WeaponStateManager weapon)
    {
        //Camera
        playerCam = weapon.PlayerCam;

        //Weapon prefab
        weaponPrefab = weapon.MachineGunPrefab;

        //VFX Prefabs
        MuzzleFlashPrefab = weapon.MG_MuzzleFlashPrefab;
        BulletHitPrefab = weapon.MG_BulletHitPrefab;
        BulletMissPrefab = weapon.MG_BulletMissPrefab;

        //Weapon stats
        AmmoCount = magCapacity;

        //Temp weapon recoil
        weaponRecoil = new Vector3(0f,0f,-0.5f);
    }

    public override void UpdateState(WeaponStateManager weapon)
    {
        //lol
    }

    private void WeaponEquip(WeaponStateManager weapon)
    {
        //Instantiating weapon and setting weapon variables
        if (!weaponPrefab.scene.IsValid())
        {
            MachineGunObject = GameObject.Instantiate(weaponPrefab, weapon.transform.position, weapon.transform.rotation);
            MachineGunObject.transform.position = weapon.transform.position;
            MachineGunObject.transform.SetParent(weapon.transform);

            SetWeaponVariables(weapon);
        }
    }

    private void SetWeaponVariables(WeaponStateManager weapon)
    {
        //Setting Variables that can't be set until the weapon exists
        firePoint = MachineGunObject.transform.Find("Firepoint").gameObject;
        weapon.currentWeaponModel = MachineGunObject;
        animController = MachineGunObject.GetComponent<Animator>();
        weaponBasePosition = MachineGunObject.transform.localPosition;
    }



/////////////////////////////////////////////////////Firing\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    public override void PrimaryFire(WeaponStateManager weapon)
    {
        if (Time.time - lastFired > 1/fireRate && AmmoCount > 0)
        {
            //Fire and restart shooting cd
            lastFired = Time.time;
            DoShoot(weapon);

            //Adding VFX to the scene
            GameObject muzzleFlash = GameObject.Instantiate(MuzzleFlashPrefab, firePoint.transform.position, weapon.PlayerCam.transform.rotation);
            muzzleFlash.transform.SetParent(MachineGunObject.transform);
            GameObject.Destroy(muzzleFlash, 1.0f);
        } 
        else
        {
            //AmmoCount = 32;
        }
    }

    private void DoShoot(WeaponStateManager weapon)
    {
        AmmoCount -= 1;
        DoRecoil(weapon);

        //Screen shake / kickback
        CinemachineImpulseSource source = MachineGunObject.GetComponent<CinemachineImpulseSource>();
        source.GenerateImpulse();

        Vector3 ray = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));

        //Fire a raycast which functions as the "bullet"
        if (Physics.Raycast(ray, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            //Apply effects to target
            Debug.Log(hit);
            Debug.Log(weapon);
            AccessEnemyScript(weapon, hit);
        }
    }
    
    public override void AccessEnemyScript(WeaponStateManager weapon, RaycastHit hit)
    {
        //Checking if the player actually hit the damageHandler or not, if yes: hitVFX are added and if no miss vfx are added
        //This will collect the "Stats" script from the damageHandler which can be used to damage it
        if (hit.transform.tag == "Enemy")
        {
            GameObject bulletHitEnemy = GameObject.Instantiate(BulletHitPrefab, hit.point, Quaternion.identity);
            GameObject.Destroy(bulletHitEnemy, 1.0f);

            GameObject parentHit = hit.transform.parent.gameObject;
            if (parentHit)
            {
                damageHandler = parentHit.GetComponent<IDamageHandler>();

                //Check if the damageHandler script has been found and then do damage if it has
                if (damageHandler != null && hit.point != null)
                {
                    float dmg = damage + dmgBoost;
                    dmgBoost += 1f;
                    damageHandler.TakeDamage(damage, hit.collider);
                }
            }
            else
            {
                Debug.Log("Nada");
            }


        }
        else
        {
            damageHandler = null;
            GameObject bulletMissEnemy = GameObject.Instantiate(BulletMissPrefab, hit.point, Quaternion.identity);
            GameObject.Destroy(bulletMissEnemy, 1.0f);
        }
    }

//////////////////////////////////////////////////////////////////Recoil\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    void DoRecoil(WeaponStateManager weapon)
    {
        MachineGunObject.transform.localPosition += weaponRecoil;
        //animController.Play("Firing");
    }

    public void CheckGunRecoil()
    {
        //Slowly resetting the Temporary recoil system after the player finishes firing
        if (MachineGunObject.transform.localPosition != weaponBasePosition)
        {
            elapsedTime += Time.deltaTime;
            MachineGunObject.transform.localPosition = Vector3.Lerp(MachineGunObject.transform.localPosition, weaponBasePosition, elapsedTime/duration);
        }
        else
        {
            elapsedTime = 0f;
        }
    }

    /////////////////////////////////////////////////////////Reload\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    public override void Reload(WeaponStateManager weapon)
    {
        AmmoCount = magCapacity;
        dmgBoost = 0f;
    }
}
