using UnityEngine;

public class AntiRoll : MonoBehaviour
{
    public Rigidbody rb;           // O Rigidbody do veículo
    public float antiRollForce = 5000f;  // A força anti-tombamento (ajustável)
    public float rollThreshold = 0.3f;   // O limite de inclinação para ativar o anti-roll

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        ApplyAntiRoll();
    }

    void ApplyAntiRoll()
    {
        // Verificar a inclinação em torno do eixo Z (rotação lateral)
        float rollAngle = Mathf.Abs(transform.localEulerAngles.z);

        // Verificar se o carro está inclinado além do limite
        if (rollAngle > rollThreshold && rollAngle < 360 - rollThreshold)
        {
            // Aplica uma força corretiva para baixo quando o carro inclinar
            rb.AddForce(-transform.up * antiRollForce * Time.fixedDeltaTime);
        }
    }
}
