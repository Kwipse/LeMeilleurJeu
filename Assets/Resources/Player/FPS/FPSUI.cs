using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using classes;
using systems;


public class FPSUI : MonoBehaviour
{
    //FPS Player info
    FPSPlayer player;
    WeaponSystem WS;
    GameObject currentWeapon;

    //UI
    TMP_Text AmmoTxt;


    void Start()
    {
        SetupPlayerInfo();
        SetupUIInfo();
    }

    void Update()
    {
        UpdateAmmo();

    }

    void SetupPlayerInfo()
    {
        player = GetComponent<FPSPlayer>();
        WS = GetComponent<WeaponSystem>();
        currentWeapon = WS.currentWeapon;
    }

    void SetupUIInfo()
    {
        foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>())
        {
            switch (text.name) {

                case "AmmoCount":
                    AmmoTxt = text;
                    break;

                default:
                    break;
            }
        }
    }

    void UpdateAmmo()
    {
        AmmoTxt.text = "Sauce";

    }
}
