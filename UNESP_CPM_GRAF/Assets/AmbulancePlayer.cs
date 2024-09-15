using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Importar para usar UI

public class AmbulancePlayer : MonoBehaviour
{
    public float moveSpeed = 10f;         // Velocidade de movimento do carro
    public float rotationSpeed = 50f;     // Velocidade de rotação do carro
    public float maxSpeed = 20f;          // Velocidade máxima do carro
    public float acceleration = 35f;      // Taxa de aceleração
    public float deceleration = 35f;      // Taxa de desaceleração (ajustada para ser mais realista)
    public float reverseDeceleration = 30f; // Taxa de desaceleração quando a direção é oposta (ajustada para ser mais realista)
    public float turnSmoothness = 2f;     // Suavidade da rotação
    public float minTurningSpeed = 5f;    // Velocidade mínima para começar a virar
    public float turnTransitionSpeed = 1f; // Velocidade da transição para a rotação

    public RectTransform speedometerNeedle; // Referência à agulha do velocímetro

    public AudioClip engineSound;          // Som do motor
    public AudioSource audioSource;       // Referência ao componente AudioSource

    private float currentSpeed = 0f;      // Velocidade atual do carro
    private float turnInput = 0f;         // Entrada para rotação
    private bool isMoving = false;        // Indica se o carro está se movendo

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = engineSound;
        audioSource.loop = true; // Define o som como um loop
        audioSource.Play(); // Inicia o som do motor
    }

    private void Update()
    {
        PlayerMovement();
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        UpdateMeters();
        UpdateEngineSound();
    }

    void PlayerMovement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        isMoving = moveZ != 0;

        MoveVertically(moveZ);
        MoveHorizontally(moveX);
    }

    void MoveVertically(float moveZ)
    {
        if (moveZ != 0)
        {
            if (Mathf.Sign(moveZ) != Mathf.Sign(currentSpeed) && currentSpeed != 0)
            {
                // Frear mais suavemente se a direção for oposta
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0, reverseDeceleration * Time.deltaTime);
            }
            else
            {
                // Aumenta ou diminui a velocidade com base na entrada do jogador
                currentSpeed += moveZ * acceleration * Time.deltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);
            }
        }
        else
        {
            // Desacelera quando não há entrada
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        // Atualiza o estado de movimentação
        isMoving = currentSpeed != 0;
    }

    void MoveHorizontally(float moveX)
    {
        // Ajusta o input para rotação
        if (Mathf.Abs(currentSpeed) >= minTurningSpeed || currentSpeed == 0)
        {
            // Calcula a velocidade atual proporcional para a rotação
            float turnSpeed = Mathf.Clamp01((Mathf.Abs(currentSpeed) - minTurningSpeed) / (maxSpeed - minTurningSpeed));

            if (moveX != 0)
            {
                // Atualiza a entrada de rotação
                turnInput = Mathf.Lerp(turnInput, moveX, turnSmoothness * Time.deltaTime * turnSpeed);
            }
            else
            {
                // Gradualmente retorna a rotação para zero quando não está virando
                turnInput = Mathf.Lerp(turnInput, 0, turnSmoothness * Time.deltaTime);
            }

            // Rotaciona o carro suavemente com base na velocidade
            if (currentSpeed != 0)
            {
                float rotationAmount = turnInput * rotationSpeed * Time.deltaTime * Mathf.Sign(currentSpeed);
                transform.Rotate(Vector3.up * rotationAmount);
            }
        }
        else
        {
            // Quando não está se movendo ou a velocidade está abaixo do limite de rotação, retorna a rotação para zero
            turnInput = Mathf.Lerp(turnInput, 0, turnSmoothness * Time.deltaTime);
        }
    }

    void UpdateMeters()
    {
        // Atualiza o ângulo da agulha do velocímetro com base na velocidade
        if (speedometerNeedle != null)
        {
            // Converte a velocidade em um ângulo (0 a 180 graus, ajustável)
            float needleAngle = Mathf.Lerp(0f, -270f, Mathf.InverseLerp(0f, maxSpeed, Mathf.Abs(currentSpeed)));
            speedometerNeedle.localRotation = Quaternion.Euler(0f, 0f, needleAngle);
        }
    }

    void UpdateEngineSound()
    {
        if (audioSource != null && engineSound != null)
        {
            // Ajusta o pitch do áudio com base na velocidade
            audioSource.pitch = Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, maxSpeed, Mathf.Abs(currentSpeed)));
        }
    }
}
