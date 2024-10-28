using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputeResult : MonoBehaviour
{
    // Referências aos componentes de cima
    public Image topBackground;
    public TextMeshProUGUI[] topDescriptions;
    public TextMeshProUGUI[] topScores;

    // Referências aos componentes de baixo
    public Image bottomBackground;
    public TextMeshProUGUI[] bottomDescriptions;
    public TextMeshProUGUI[] bottomScores;

    private float healthValue;
    private float timeValue;
    private float vehicleScore;
    private float qteValue;
    private float totalScore;

    void Start()
    {
        SetActiveAll(false);
    }

    public void SaveVehicleScore(float healthValue, float maxHealthValue, float timeRemaining, float maxTime)
    {
        float healthScore = (healthValue / maxHealthValue) * 3000;
        float timeScore = (timeRemaining / maxTime) * 1500;
        vehicleScore = healthScore + timeScore;
        timeValue = timeRemaining;
    }

    public void QTEScore(float score)
    {
        qteValue = score;
    }

    public void DisplayScore()
    {
        totalScore = vehicleScore + qteValue;

        // Preenchendo valores nas posições corretas
        topScores[0].text = vehicleScore.ToString("F0");  // Pontuação do veículo

        // Converte o timeValue para o formato de minutos e segundos
        int minutes = Mathf.FloorToInt(timeValue / 60);
        int seconds = Mathf.FloorToInt(timeValue % 60);
        topScores[1].text = string.Format("{0:D2}:{1:D2}", minutes, seconds);  // Tempo

        bottomScores[0].text = qteValue.ToString("F0");  // Pontuação de QTE
        bottomScores[1].text = totalScore.ToString("F0");  // Pontuação total

        StartCoroutine(DisplaySequence());
    }

    private void SetActiveAll(bool isActive)
    {
        topBackground.gameObject.SetActive(isActive);
        foreach (var text in topDescriptions) text.gameObject.SetActive(isActive);
        foreach (var score in topScores) score.gameObject.SetActive(isActive);
        bottomBackground.gameObject.SetActive(isActive);
        foreach (var text in bottomDescriptions) text.gameObject.SetActive(isActive);
        foreach (var score in bottomScores) score.gameObject.SetActive(isActive);
    }

    private IEnumerator DisplaySequence()
    {
        // Mostra a imagem do topo com efeito pop-in
        yield return PopInEffect(topBackground.gameObject);
        yield return new WaitForSeconds(0.3f);

        // Mostra descrições do topo
        foreach (var text in topDescriptions)
        {
            text.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(0.3f);

        // Mostra pontuações do topo com efeito pop-in
        foreach (var score in topScores)
        {
            yield return PopInEffect(score.gameObject);
        }

        yield return new WaitForSeconds(0.5f);

        // Mostra a imagem do fundo com efeito pop-in
        yield return PopInEffect(bottomBackground.gameObject);
        yield return new WaitForSeconds(0.3f);

        // Mostra descrições do fundo
        foreach (var text in bottomDescriptions)
        {
            text.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(0.3f);

        // Mostra pontuações do fundo com efeito pop-in
        foreach (var score in bottomScores)
        {
            yield return PopInEffect(score.gameObject);
        }
    }

    private IEnumerator PopInEffect(GameObject obj)
    {
        obj.SetActive(true);
        obj.transform.localScale = Vector3.zero;
        float elapsedTime = 0f;
        float duration = 0.3f;

        while (elapsedTime < duration)
        {
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.transform.localScale = Vector3.one;
    }
}
