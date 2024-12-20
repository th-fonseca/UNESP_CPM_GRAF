using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HospitalTransition : MonoBehaviour
{
    public Camera playerCamera;  // C�mera principal
    public Camera hospitalCamera;  // C�mera do hospital
    public Camera cutsceneCamera;  // C�mera do hospital
    public Image fadeImage;  // Imagem usada para o efeito de fade
    public GameObject playerUI;
    public GameObject ambulancePlayer;
    public GameObject patientCutscene;

    private AmbulancePlayer ambulanceControls;
    private AmbulanceLights ambulanceLights;
    private AmbulanceHealth ambulanceHealth;
    private AudioSource ambulanceAudioSource;
    public AudioSource[] aditionalAudioSource;
    public AudioClip victoryClip;
    private AudioSource victorySource;
    public PlayableDirector playableDirector;
    private DisplayQuickTimeEvent ambulanceDisplayQuickTimeEvent;
    private ComputeResult computeScore;
    private Timer timer;
    private MusicManager manager;

    void Start()
    {
        ambulanceControls = ambulancePlayer.GetComponent<AmbulancePlayer>();
        ambulanceLights = ambulancePlayer.GetComponent<AmbulanceLights>();
        ambulanceHealth = ambulancePlayer.GetComponent<AmbulanceHealth>();
        ambulanceAudioSource = ambulancePlayer.GetComponent<AudioSource>();
        ambulanceDisplayQuickTimeEvent = GetComponentInParent<DisplayQuickTimeEvent>();
        timer = playerUI.GetComponentInChildren<Timer>();
        computeScore = playerUI.GetComponentInParent<Canvas>().GetComponentInChildren<ComputeResult>();
        victorySource = GetComponent<AudioSource>();
        patientCutscene.SetActive(false);
        hospitalCamera.enabled = false;  // Desativa a c�mera do hospital no in�cio
        cutsceneCamera.enabled = false;
        fadeImage.color = new Color(0, 0, 0, 0);  // Come�a com a imagem invis�vel
        manager = MusicManager.Instance;
        manager.PlayMusicFromStart();
    }

    public void SwitchToHospitalCamera()
    {
        manager.ToggleMusicWithFade();
        computeScore.SaveVehicleScore(ambulanceHealth.healthValue, ambulanceHealth.maxHealth, timer.timeRemaining, timer.startTimeInSeconds);
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

    public void SwitchToCutsceneCamera()
    {
        manager.StopMusicWithFade();
        patientCutscene.SetActive(true);
        playableDirector.Play();
        StartCoroutine(FadeTransitionWithScoreDisplay(cutsceneCamera, hospitalCamera));
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

    private IEnumerator FadeTransitionWithScoreDisplay(Camera targetCamera, Camera currentCamera)
    {
        yield return StartCoroutine(Fade(1f, 0.5f));

        currentCamera.enabled = false;
        targetCamera.enabled = true;

        yield return StartCoroutine(Fade(0f, 0.5f));

        // Espera antes de exibir a pontua��o
        yield return new WaitForSeconds(5f);  // Tempo de delay desejado
        victorySource.PlayOneShot(victoryClip);
        computeScore.DisplayScore();

        yield return new WaitForSeconds(5f);
        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return StartCoroutine(Fade(1f, 0.5f));  // Fade out para o menu principal
        SceneManager.LoadScene("MenuScene");  // Nome da cena do menu principal
    }

    private IEnumerator FadeTransition(Camera targetCamera, Camera currentCamera)
    {
        yield return StartCoroutine(Fade(1f, 0.5f));

        currentCamera.enabled = false;
        targetCamera.enabled = true;

        yield return StartCoroutine(Fade(0f, 0.5f));
    }

    public IEnumerator Fade(float targetAlpha, float duration)
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

        // Garante que o alpha est� exatamente no valor alvo no final da transi��o
        color.a = targetAlpha;
        fadeImage.color = color;
    }
}
