using UnityEngine;

public class StopOnCollision : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // M�todo chamado quando uma colis�o � detectada
    void OnCollisionEnter(Collision collision)
    {
        // Para parar o ve�culo ao colidir com qualquer objeto
        Debug.Log("Colis�o detectada com: " + collision.gameObject.name);
        Debug.Log("Ambulancia: " + rb.gameObject.name);

        // Zera a velocidade do objeto
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Aplica uma for�a de desacelera��o usando ForceMode.Impulse
        Vector3 stopForce = -rb.velocity * rb.mass* 10000000; // Calcula a for�a necess�ria para parar o ve�culo
        rb.AddForce(stopForce, ForceMode.Impulse); // Aplica a for�a instantaneamente
    }
}
