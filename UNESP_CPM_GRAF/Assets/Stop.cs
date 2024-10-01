using UnityEngine;

public class StopOnCollision : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Método chamado quando uma colisão é detectada
    void OnCollisionEnter(Collision collision)
    {
        // Para parar o veículo ao colidir com qualquer objeto
        Debug.Log("Colisão detectada com: " + collision.gameObject.name);
        Debug.Log("Ambulancia: " + rb.gameObject.name);

        // Zera a velocidade do objeto
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Aplica uma força de desaceleração usando ForceMode.Impulse
        Vector3 stopForce = -rb.velocity * rb.mass* 10000000; // Calcula a força necessária para parar o veículo
        rb.AddForce(stopForce, ForceMode.Impulse); // Aplica a força instantaneamente
    }
}
