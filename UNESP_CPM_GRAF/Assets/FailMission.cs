using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailMission : MonoBehaviour
{
    public GameObject failScreen;
    public GameObject hospital;
    public AudioClip failSound;
    public AudioSource failAudio;

    private HospitalTransition deactivateControls;
    private bool hasFailed = false;
    void Start()
    {
        failScreen.SetActive(false);
        deactivateControls = hospital.GetComponent<HospitalTransition>();
    }


    public void Fail()
    {
        if (hasFailed) return;  // Retorna se a função já foi chamada
        hasFailed = true;  // Marca a função como chamada

        StartCoroutine(deactivateControls.Fade(1f, 0.5f));
        failAudio.PlayOneShot(failSound);
        deactivateControls.ToggleAmbulanceControls();
        deactivateControls.playerUI.SetActive(false);
        failScreen.SetActive(true);
    }

    public void RetryMission()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
