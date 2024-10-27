using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQuickTimeEvent : MonoBehaviour
{
    public enum QTEType { Sequence, RapidPress, SingleQuickPress }
    public QTEType currentQTEType;

    public GameObject QTEInterface;
    public List<KeyCode> possibleKeys;
    public GameObject keyImagePrefab;
    public Transform keyImageParent;
    public Slider timerSlider;

    public int sequenceLength = 5;
    public float sequenceDelay = 10f;
    public int totalQTEs = 3;
    public float fadeDuration = 0.5f; // Duração do fade
    private float startDelay = 1f;

    private List<KeyCode> generatedSequence;
    private List<GameObject> displayedKeys;
    private int currentSequenceIndex;
    private float sequenceTimer;
    private bool isQTEActive = false;
    private int completedQTECount = 0;

    void Start()
    {
        QTEInterface.SetActive(false);
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
            HandleSequenceQTE();
            UpdateTimerUI();
        }
    }

    public void StartQTE()
    {
        StartCoroutine(DelayedStartQTE(startDelay));
    }

    // Corrotina para iniciar o QTE após um delay
    private IEnumerator DelayedStartQTE(float delay)
    {
        yield return new WaitForSeconds(delay); // Espera pelo tempo de delay

        StartCoroutine(FadeQTEInterface(true)); // Inicia o fade-in da interface QTE
        GenerateRandomSequence();
        DisplaySequenceUI();
        ResetQTE();
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

    private void DisplaySequenceUI()
    {
        ClearSequenceUI();
        foreach (KeyCode key in generatedSequence)
        {
            GameObject keyImageObj = Instantiate(keyImagePrefab, keyImageParent);

            // Define a cor branca como padrão para a imagem
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
            // Inicia o fade-out da tecla pressionada corretamente
            StartCoroutine(FadeOutKey(displayedKeys[currentSequenceIndex]));

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

    private void CompleteQTE(bool success)
    {
        Debug.Log(success ? "QTE Completed" : "QTE Failed");

        if (success)
        {
            completedQTECount++; // Incrementa o contador de QTEs completados

            if (completedQTECount >= totalQTEs)
            {
                Debug.Log("All QTEs Completed! You win!");
                StopQTE(); // Finaliza o QTE após completar o número total de sequências
            }
            else
            {
                GenerateRandomSequence(); // Gera uma nova sequência
                DisplaySequenceUI();      // Exibe a nova sequência na interface
                ResetQTE();               // Reinicia a sequência
            }
        }
        else
        {
            Debug.Log("QTE Failed! Try again.");
            completedQTECount = 0;       // Reseta o contador em caso de falha
            StopQTE();                   // Interrompe o QTE se falhar
        }
    }


    private void ResetQTE()
    {
        currentSequenceIndex = 0;
        sequenceTimer = sequenceDelay;
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerSlider != null)
        {
            timerSlider.value = sequenceTimer / sequenceDelay;
        }
    }

    // Corrotina para fade-in e fade-out da interface QTE
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

    // Corrotina para fade-out das teclas pressionadas corretamente
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
