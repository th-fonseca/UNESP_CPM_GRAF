using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliverHospital : MonoBehaviour
{
    public GameObject player;
    public GameObject hospital;
    public GameObject hospitalIndoors;

    public GameObject deliveryElement;
    public TextMeshProUGUI deliveryText;

    public float deliveryDistance = 15f;
    private bool patientDelivered = false;
    private GrabPatient grabPatientScript;
    private HospitalTransition cameraSwitch;
    private void Start()
    {
        grabPatientScript = player.GetComponent<GrabPatient>();
        cameraSwitch = hospitalIndoors.GetComponent<HospitalTransition>();
        if (grabPatientScript == null)
        {
            Debug.LogError("O script GrabPatient não foi encontrado no jogador.");
        }
    }

    private void Update()
    {
        if (patientDelivered || grabPatientScript == null || !grabPatientScript.hasPickedUpPatient)
            return;

        float distance = Vector3.Distance(player.transform.position, hospital.transform.position);
        if (distance <= deliveryDistance)
        {
            deliveryText.text = "Entregar paciente";
            deliveryElement.SetActive(true);
        }
        else deliveryElement.SetActive(false);

        if (distance <= deliveryDistance && Input.GetKeyDown(KeyCode.E)) DeliverPatient();
    }

    void DeliverPatient()
    {
        patientDelivered = true;
        deliveryElement.SetActive(false);
        cameraSwitch.SwitchToHospitalCamera();
    }
}
