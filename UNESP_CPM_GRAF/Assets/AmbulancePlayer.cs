using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmbulancePlayer : MonoBehaviour
{
    //Colisores
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    //Transformadores
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    public Material brakeLights;
    //public Material headLights;
    //public Material blueLights;


    public Light leftHeadLight;
    public Light rightHeadLight;
    // Referência à agulha do velocímetro
    public RectTransform speedometerNeedle;

    //Velocidade
    public float motorForce = 12000f;
    public float maxSpeed = 50f;
    private float currentSpeed = 0f;
    private bool isReversing = false;

    //Breque
    private bool isBreaking;
    private float currentbreakForce;
    public float breakForce = 7000f;

    //Virada
    public float maxSteerAngle = 30f;
    private float currentSteerAngle = 0f;


    //Som
    public AudioClip engineSound;
    public AudioSource engineAudioSource;

    public AudioClip hornSound;
    public AudioSource hornAudioSource;



    void Start()
    {
        SetLightEmission(brakeLights, isBreaking, Color.red, Color.black);
        hornAudioSource.clip = hornSound;
        engineAudioSource.clip = engineSound;
        engineAudioSource.Play();
    }

    void Update()
    {
        PlayerMovement();
        UpdateMeters();
    }
    void PlayerMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        isBreaking = Input.GetKey(KeyCode.Space);
        isReversing = Input.GetKey(KeyCode.S);

        MoveVertically(moveZ);
        MoveHorizontally(moveX);
        UpdateWheels();
        UpdateSounds();
        ApplyBrakes();
    }

    void ApplyBrakes()
    {
        currentbreakForce = isBreaking ? breakForce : 0f;
        SetLightEmission(brakeLights, isBreaking ? isBreaking : isReversing, isBreaking ? Color.red : (isReversing ? Color.grey : Color.black), Color.black);
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    void MoveVertically(float moveZ)
    {
        currentSpeed = this.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
        
        if (moveZ != 0 && currentSpeed <= maxSpeed)
        {
            rearLeftWheelCollider.motorTorque = motorForce * moveZ;
            rearRightWheelCollider.motorTorque = motorForce * moveZ;
            rearLeftWheelCollider.brakeTorque = 0f;
            rearRightWheelCollider.brakeTorque = 0f;

        }
        else
        {
            rearLeftWheelCollider.motorTorque = 0f;
            rearRightWheelCollider.motorTorque = 0f;
            rearLeftWheelCollider.brakeTorque = motorForce;
            rearRightWheelCollider.brakeTorque = motorForce;
        }
    }

    void MoveHorizontally(float moveX)
    {
        currentSteerAngle = maxSteerAngle * moveX;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    void UpdateMeters()
    {
        if (speedometerNeedle != null)
        {
            float needleAngle = Mathf.Lerp(0f, -270f, Mathf.InverseLerp(0f, maxSpeed, Mathf.Abs(currentSpeed)));
            speedometerNeedle.localRotation = Quaternion.Euler(0f, 0f, needleAngle);
        }
    }

    void UpdateSounds()
    {
        if (engineAudioSource != null && engineSound != null)
        {
            engineAudioSource.pitch = Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, maxSpeed, Mathf.Abs(currentSpeed)));
        }
        if (Input.GetKey(KeyCode.Q))
        {
            if (hornAudioSource != null && !hornAudioSource.isPlaying)
            {
                hornAudioSource.loop = true;  // Define o som da buzina para tocar em loop
                hornAudioSource.Play();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            if (hornAudioSource != null && hornAudioSource.isPlaying)
            {
                hornAudioSource.loop = false;  // Remove o loop quando a tecla é solta
                hornAudioSource.Stop();
            }
        }
    }

    void SetLightEmission(Material materialToLight, bool shouldBeLit, Color lightOn, Color lightOff)
    {
        materialToLight.SetColor("_EmissionColor", shouldBeLit ? lightOn : lightOff);
        materialToLight.EnableKeyword("_EMISSION");
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out _, out Quaternion rot);
        wheelTransform.rotation = rot;
    }

}
