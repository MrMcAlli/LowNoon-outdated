using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounterScript : MonoBehaviour
{
    Text ammoText;
    HandCannonHandler weapon;
    WeaponMachineGunState MGState;
    int ammoCount;
    float focusVal;
    public Scrollbar focusMeter;

    void Awake()
    {
        weapon = GameObject.Find("PlayerWeapon").GetComponent<HandCannonHandler>();
        ammoText = gameObject.GetComponent<Text>();
    }

    void Update()
    {
        focusVal = weapon.LastFocusValue;
        ammoText.text = weapon.currentAmmo.ToString();
        if (focusVal > 0)
        {
            focusMeter.size = focusVal;
        } 
        else
        {
            focusMeter.size = 0;
        }

    }
}