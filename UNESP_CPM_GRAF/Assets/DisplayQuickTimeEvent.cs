using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQuickTimeEvent : MonoBehaviour
{
    public enum QTEType { Sequence, RapidPress }
    public QTEType currentQTEType;

    public GameObject QTEInterface;
    public List<KeyCode> possibleKeys;
    public GameObject keyImagePrefab;
    public Transform keyImageParent;
    public Slider timerSlider;
    public TextMeshProUGUI rapidPressCounterText;

    public AudioClip successClip;
    public AudioClip failureClip;
    public AudioClip clickClip;

    public AudioSource qteSource;

    public int sequenceLength = 5;
    public float sequenceDelay = 10f;
    public int totalQTEs = 3;
    public int rapidPressTarget = 5;     // Número de pressões para completar o RapidPress
    public float rapidPressTimeLimit = 3f; // Tempo para completar o RapidPress
    public float fadeDuration = 0.5f;
    private float startDelay = 1f;

    private List<KeyCode> generatedSequence;
    private List<GameObject> displayedKeys;
    private int currentSequenceIndex;
    private float sequenceTimer;
    private bool isQTEActive = false;
    private int completedQTECount = 0;

    private KeyCode rapidPressKey;
    private int rapidPressCount;
    private float rapidPressTimer;

    private HospitalTransition playCutscene;


    void Start()
    {
        playCutscene =  GetComponentInParent<HospitalTransition>();
        QTEInterface.SetActive(false);
        rapidPressCounterText.enabled = false;
    }

    void Awake()
    {
        generatedSequence = new List<KeyCode>();
        displayedKeys = new List<GameObject>();
    }

    void Update()
    {
        if (isQTEActive)
        {
            if (currentQTEType == QTEType.Sequence)
            {
                HandleSequenceQTE();
            }
            else if (currentQTEType == QTEType.RapidPress)
            {
                HandleRapidPressQTE();
            }
            UpdateTimerUI();
        }
    }

    public void StartQTE()
    {
        // Inicia o QTE apenas uma vez com o delay
        if (completedQTECount == 0)
        {
            StartCoroutine(DelayedStartQTE(startDelay));
        }
        else
        {
            StartNewQTE();  // Caso já esteja em progresso, inicia direto
        }
    }

    private IEnumerator DelayedStartQTE(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera pelo tempo de delay inicial
        StartNewQTE();  // Chama a função de iniciar um QTE
    }

    private void StartNewQTE()
    {
        QTEInterface.SetActive(true);
        currentQTEType = (Random.value > 0.5f) ? QTEType.Sequence : QTEType.RapidPress;
        StartCoroutine(FadeQTEInterface(true));

        if (currentQTEType == QTEType.Sequence)
        {
            GenerateRandomSequence();
            DisplaySequenceUI();
        }
        else if (currentQTEType == QTEType.RapidPress)
        {
            rapidPressCounterText.enabled = true;
            GenerateRapidPress();
            DisplayRapidPressUI();
        }

        ResetQTE();  // Reseta os parâmetros para o novo QTE
        isQTEActive = true;
    }

    public void StopQTE()
    {
        isQTEActive = false;
        ClearSequenceUI();
        StartCoroutine(FadeQTEInterface(false));
    }

    private void GenerateRandomSequence()
    {
        generatedSequence.Clear();
        for (int i = 0; i < sequenceLength; i++)
        {
            int randomIndex = Random.Range(0, possibleKeys.Count);
            generatedSequence.Add(possibleKeys[randomIndex]);
        }
    }

    private void GenerateRapidPress()
    {
        int randomIndex = Random.Range(0, possibleKeys.Count);
        rapidPressKey = possibleKeys[randomIndex];
        rapidPressCount = 0;
        rapidPressTimer = rapidPressTimeLimit;
    }

    private void DisplaySequenceUI()
    {
        ClearSequenceUI();
        foreach (KeyCode key in generatedSequence)
        {
            GameObject keyImageObj = Instantiate(keyImagePrefab, keyImageParent);
            Image keyImage = keyImageObj.GetComponent<Image>();
            if (keyImage != null)
            {
                keyImage.color = Color.white;
            }

            TextMeshProUGUI keyText = keyImageObj.GetComponentInChildren<TextMeshProUGUI>();
            if (keyText != null)
            {
                keyText.text = key.ToString();
            }

            displayedKeys.Add(keyImageObj);
        }
    }

    private void DisplayRapidPressUI()
    {
        ClearSequenceUI();
        GameObject keyImageObj = Instantiate(keyImagePrefab, keyImageParent);
        Image keyImage = keyImageObj.GetComponent<Image>();
        if (keyImage != null)
        {
            keyImage.color = Color.white;
        }

        TextMeshProUGUI keyText = keyImageObj.GetComponentInChildren<TextMeshProUGUI>();
        if (keyText != null)
        {
            keyText.text = rapidPressKey.ToString();
        }

        rapidPressCounterText.text = "x" + rapidPressTarget.ToString(); // Mostra o contador inicial
        displayedKeys.Add(keyImageObj);
    }

    private void ClearSequenceUI()
    {
        foreach (GameObject keyImage in displayedKeys)
        {
            Destroy(keyImage);
        }
        displayedKeys.Clear();
    }

    private void HandleSequenceQTE()
    {
        sequenceTimer -= Time.deltaTime;

        if (currentSequenceIndex >= generatedSequence.Count)
        {
            CompleteQTE(true);
            return;
        }

        if (Input.GetKeyDown(generatedSequence[currentSequenceIndex]))
        {
            StartCoroutine(FadeOutKey(displayedKeys[currentSequenceIndex]));
            qteSource.PlayOneShot(clickClip);
            currentSequenceIndex++;
            sequenceTimer = sequenceDelay;
        }
        else if (Input.anyKeyDown && !Input.GetKeyDown(generatedSequence[currentSequenceIndex]))
        {
            CompleteQTE(false);
        }

        if (sequenceTimer <= 0)
        {
            CompleteQTE(false);
        }
    }

    private void HandleRapidPressQTE()
    {
        rapidPressTimer -= Time.deltaTime;

        if (Input.GetKeyDown(rapidPressKey))
        {
            rapidPressCount++;
            rapidPressCounterText.text = "x" + (rapidPressTarget - rapidPressCount).ToString(); // Atualiza o contador

            if (rapidPressCount >= rapidPressTarget)
            {
                rapidPressCounterText.enabled = false;
                CompleteQTE(true);
                return;
            }
        }
        else if (Input.anyKeyDown && !Input.GetKeyDown(rapidPressKey))
        {
            rapidPressCounterText.enabled = false;
            CompleteQTE(false);
        }

        if (rapidPressTimer <= 0)
        {
            rapidPressCounterText.enabled = false;
            CompleteQTE(false);
        }
    }

    private void CompleteQTE(bool success)
    {
        Debug.Log(success ? "QTE Completed" : "QTE Failed");

        if (success)
        {
            qteSource.PlayOneShot(successClip);
            completedQTECount++;
            if (completedQTECount >= totalQTEs)
            {
                playCutscene.SwitchToCutsceneCamera();
                StopQTE();
            }
            else
            {
                StartCoroutine(FadeQTEInterface(false));
                isQTEActive = false; // Desativa para evitar contagens incorretas
                StartCoroutine(DelayedStartNewQTE(2f)); // Inicia próxima sequência com delay
            }
        }
        else
        {
            qteSource.PlayOneShot(failureClip);
            Debug.Log("QTE Failed! Try again.");
            completedQTECount = 0;  // Reinicia a contagem de QTEs completados em caso de falha
            StopQTE();
        }
    }
    private IEnumerator DelayedStartNewQTE(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartNewQTE();
    }

    private void ResetQTE()
    {
        currentSequenceIndex = 0;
        sequenceTimer = sequenceDelay;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        float timeRemaining = currentQTEType == QTEType.RapidPress ? rapidPressTimer : sequenceTimer;
        timerSlider.value = timeRemaining / sequenceDelay;
    }

    private IEnumerator FadeQTEInterface(bool fadeIn)
    {
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;
        CanvasGroup canvasGroup = QTEInterface.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = QTEInterface.AddComponent<CanvasGroup>();
        }

        QTEInterface.SetActive(true);
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;

        if (!fadeIn)
        {
            QTEInterface.SetActive(false);
        }
    }

    private IEnumerator FadeOutKey(GameObject keyObj)
    {
        Image keyImage = keyObj.GetComponent<Image>();
        TextMeshProUGUI keyText = keyObj.GetComponentInChildren<TextMeshProUGUI>();

        float startAlpha = 1;
        float endAlpha = 0;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeDuration;
            if (keyImage != null)
            {
                keyImage.color = new Color(keyImage.color.r, keyImage.color.g, keyImage.color.b, Mathf.Lerp(startAlpha, endAlpha, normalizedTime));
            }
            if (keyText != null)
            {
                keyText.color = new Color(keyText.color.r, keyText.color.g, keyText.color.b, Mathf.Lerp(startAlpha, endAlpha, normalizedTime));
            }
            yield return null;
        }

        if (keyImage != null)
        {
            keyImage.color = new Color(keyImage.color.r, keyImage.color.g, keyImage.color.b, endAlpha);
        }
        if (keyText != null)
        {
            keyText.color = new Color(keyText.color.r, keyText.color.g, keyText.color.b, endAlpha);
        }
    }
}