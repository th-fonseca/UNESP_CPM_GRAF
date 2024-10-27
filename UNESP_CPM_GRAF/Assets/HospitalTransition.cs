using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HospitalTransition : MonoBehaviour
{
    public Camera playerCamera;  // Câmera principal
    public Camera hospitalCamera;  // Câmera do hospital
    public Image fadeImage;  // Imagem usada para o efeito de fade
    public GameObject playerUI;
    public GameObject ambulancePlayer;

    private AmbulancePlayer ambulanceControls;
    private AmbulanceLights ambulanceLights;
    private AmbulanceHealth ambulanceHealth;
    private AudioSource ambulanceAudioSource;
    public AudioSource[] aditionalAudioSource;
    private DisplayQuickTimeEvent ambulanceDisplayQuickTimeEvent;
    private Timer timer;

    void Start()
    {
        ambulanceControls = ambulancePlayer.GetComponent<AmbulancePlayer>();
        ambulanceLights = ambulancePlayer.GetComponent<AmbulanceLights>();
        ambulanceHealth = ambulancePlayer.GetComponent<AmbulanceHealth>();
        ambulanceAudioSource = ambulancePlayer.GetComponent<AudioSource>();
        ambulanceDisplayQuickTimeEvent = GetComponentInParent<DisplayQuickTimeEvent>();
        timer = playerUI.GetComponentInChildren<Timer>();
        hospitalCamera.enabled = false;  // Desativa a câmera do hospital no início
        fadeImage.color = new Color(0, 0, 0, 0);  // Começa com a imagem invisível
    }

    public void SwitchToHospitalCamera()
    {
        StartCoroutine(FadeTransition(hospitalCamera, playerCamera));
        playerUI.SetActive(false);
        ToggleAmbulanceControls();
        ambulanceDisplayQuickTimeEvent.StartQTE();
    }

    public void SwitchToPlayerCamera()
    {
        StartCoroutine(FadeTransition(playerCamera, hospitalCamera));
        playerUI.SetActive(true);
        ToggleAmbulanceControls();
    }

    public void ToggleAmbulanceControls()
    {
        ambulanceControls.enabled = !ambulanceControls.enabled;
        ambulanceLights.enabled = !ambulanceLights.enabled;
        ambulanceHealth.enabled = !ambulanceHealth.enabled;
        ambulanceAudioSource.enabled = !ambulanceAudioSource.enabled;
        timer.StopTimer();
        foreach (var audioSource in aditionalAudioSource)
        {
            audioSource.enabled = !audioSource.enabled;
        }
    }

    private IEnumerator FadeTransition(Camera targetCamera, Camera currentCamera)
    {
        yield return StartCoroutine(Fade(1f, 0.5f));

        currentCamera.enabled = false;
        targetCamera.enabled = true;

        yield return StartCoroutine(Fade(0f, 0.5f));
    }

    private IEnumerator Fade(float targetAlpha, float duration)
    {
        Color color = fadeImage.color;
        float startAlpha = color.a;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, normalizedTime);
            fadeImage.color = color;
            yield return null;
        }

        // Garante que o alpha está exatamente no valor alvo no final da transição
        color.a = targetAlpha;
        fadeImage.color = color;
    }
}
