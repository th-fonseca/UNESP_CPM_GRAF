using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class AmbulanceHealth : MonoBehaviour
{
    public Image healthBar;
    public float healthValue = 0f;
    public Collider ambulanceColider;

    void Start()
    {
        
    }

    void Update()
    {
    }


    void OnTriggerEnter(Collider other)
    {
            healthValue -= 10f; 
            HealthChange(healthValue);
    }

    void HealthChange(float healthValue)
    {
        float healthAmount = (healthValue / 100.0f) * 180.0f / 360;
        healthBar.fillAmount = healthAmount;

    }
}
