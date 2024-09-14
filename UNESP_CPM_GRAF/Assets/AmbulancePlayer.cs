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
    public float deceleration = 20f;      // Taxa de desaceleração (ajustada para ser mais realista)
    public float reverseDeceleration = 30f; // Taxa de desaceleração quando a direção é oposta (ajustada para ser mais realista)
    public float turnSmoothness = 2f;     // Suavidade da rotação

    public Slider speedSlider;            // Referência ao Slider de velocidade
    public RectTransform speedometerNeedle; // Referência à agulha do velocímetro

    private float currentSpeed = 0f;      // Velocidade atual do carro
    private float turnInput = 0f;         // Entrada para rotação

    // Update é chamado uma vez por frame
    void Update()
    {
        // Input para o eixo vertical (W e S) para controlar a aceleração
        float moveZ = Input.GetAxis("Vertical");

        // Se a entrada do jogador estiver na direção oposta ao movimento atual, frear mais rapidamente
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

        // Movimenta o carro para frente e para trás
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

        // Input para o eixo horizontal (A e D) para controlar a rotação
        float moveX = Input.GetAxis("Horizontal");

        // Atualiza a entrada de rotação
        if (moveX != 0 && currentSpeed != 0)
        {
            // Lerp para suavizar a rotação
            turnInput = Mathf.Lerp(turnInput, moveX, turnSmoothness * Time.deltaTime);
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

        // Atualiza o valor do Slider com a velocidade atual
        if (speedSlider != null)
        {
            speedSlider.value = Mathf.Abs(currentSpeed); // Valor absoluto para mostrar velocidade positiva
        }

        // Atualiza o ângulo da agulha do velocímetro com base na velocidade
        if (speedometerNeedle != null)
        {
            // Converte a velocidade em um ângulo (0 a 180 graus, ajustável)
            float needleAngle = Mathf.Lerp(0f, -270f, Mathf.InverseLerp(0f, maxSpeed, Mathf.Abs(currentSpeed)));
            speedometerNeedle.localRotation = Quaternion.Euler(0f, 0f, needleAngle);
        }
    }
}
