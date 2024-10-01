using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tryMove : MonoBehaviour
{
    public GameObject wheel_frontRight;
    public GameObject wheel_frontLeft;
    public GameObject wheel_backRight;
    public GameObject wheel_backLeft;

    public WheelCollider W_FR;
    public WheelCollider W_FL;
    public WheelCollider W_BR;
    public WheelCollider W_BL;

    public float speed = 1000f;
    public float lowestSpeed = 20f;
    public float lowestAngle = 70f;
    public float highestAngle = 40f;
    public float steerSmoothness = 10f;  // Taxa de suavização da direção
    public float steerMultiplier = 2f;  // Multiplicador para a resposta do volante
    public float accelerationFactor = 0.5f;  // Fator de aceleração ao virar
    public float rotationSpeed = 100f;   // Velocidade de rotação adicional para curvas

    private float currentSteerAngle = 0f;  // Armazena o ângulo de direção atual

    void Start()
    {

    }

    void FixedUpdate()
    {
        carMove();
    }

    void carMove()
    {
        // Aplica torque nas rodas traseiras para o movimento em linha reta
        W_BL.motorTorque = speed * Input.GetAxis("Vertical");
        W_BR.motorTorque = speed * Input.GetAxis("Vertical");

        // Calcula o fator de velocidade para o ângulo de direção
        float speedFactor = this.GetComponent<Rigidbody>().velocity.magnitude / lowestSpeed;

        // Calcula o ângulo de direção alvo considerando a aceleração
        float targetAngle = Mathf.Lerp(lowestAngle, highestAngle, speedFactor) * Input.GetAxis("Horizontal") * steerMultiplier;

        // Reduz a influência do ângulo alvo quando acelerando
        if (Input.GetAxis("Vertical") > 0)
        {
            targetAngle *= accelerationFactor;  // Diminui a resposta do ângulo ao acelerar
        }

        // Suaviza a transição entre o ângulo atual e o ângulo alvo
        currentSteerAngle = Mathf.LerpAngle(currentSteerAngle, targetAngle, steerSmoothness * Time.deltaTime);

        // Aplica o ângulo suavizado nas rodas dianteiras
        W_FL.steerAngle = currentSteerAngle;
        W_FR.steerAngle = currentSteerAngle;

        // Adiciona uma rotação suave ao veículo nas curvas (opcional)
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {
            // Gira o veículo diretamente quando está virando, baseado no input horizontal
            transform.Rotate(Vector3.up, Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime);
        }
    }
}
