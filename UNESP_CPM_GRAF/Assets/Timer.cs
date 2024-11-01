using System.Collections;
using UnityEngine;
using TMPro;  // Certifique-se de incluir o namespace do TextMeshPro

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText; 
    public float timeRemaining; 
    public float startTimeInSeconds = 300f;
    private bool timerRunning = false;
    public FailMission triggerFail;

    void Start()
    {

        SetTime(startTimeInSeconds);
        StartTimer(); 
    }

    void Update()
    {
        if (timerRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; 
            UpdateTimerDisplay(); 

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                triggerFail.Fail();
                StopTimer();  
                UpdateTimerDisplay();
            }
        }
    }

    public void SetTime(float newTimeInSeconds)
    {
        timeRemaining = Mathf.Clamp(newTimeInSeconds, 0f, Mathf.Infinity); 
        UpdateTimerDisplay();  
    }

    public void StartTimer()
    {
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        if(timerText) timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);  // Atualiza o texto com o tempo restante
    }
}
