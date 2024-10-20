using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GrabPatient : MonoBehaviour
{
    // O alvo/paciente que o jogador deve pegar
    public GameObject patient;
    public GameObject player;

    // UI elements (Imagem e Texto)
    public GameObject pickupElement;
    public TextMeshProUGUI pickupText;

    // Distância mínima para mostrar a UI
    public float displayDistance = 5f;

    private void Start()
    {
        pickupElement.SetActive(false);
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, patient.transform.position);
        if (distance <= displayDistance)
        {
            pickupElement.SetActive(true);
            pickupText.text = "Pegar paciente";
        }
        else
        {
            pickupElement.SetActive(false);
        }
        if (distance <= displayDistance && Input.GetKeyDown(KeyCode.E))
        {
            PickupPatient();
        }
    }
    void PickupPatient()
    {
        Debug.Log("Paciente pego!");
        patient.SetActive(false);
        pickupElement.SetActive(false);
    }
}
