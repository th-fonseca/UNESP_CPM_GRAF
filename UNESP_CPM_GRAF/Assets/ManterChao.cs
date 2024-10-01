using UnityEngine;

public class KeepCarOnGround : MonoBehaviour
{
    private Rigidbody rb;
    public float groundDistance = 0.5f; // Distância para verificar a colisão com o chão
    public LayerMask groundLayer; // Layer que representa o chão

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Verifica a distância até o chão
        if (!IsGrounded())
        {
            // Ajusta a posição do carro se não estiver no chão
            AdjustCarPosition();
        }
    }

    private bool IsGrounded()
    {
        // Verifica se o carro está tocando o chão
        return Physics.Raycast(transform.position, Vector3.down, groundDistance, groundLayer);
    }

    private void AdjustCarPosition()
    {
        // Move o carro para baixo na direção do chão
        Vector3 newPosition = transform.position;
        newPosition.y = GetGroundHeight();
        transform.position = newPosition;
    }

    private float GetGroundHeight()
    {
        // Faz um Raycast para obter a altura do chão abaixo do carro
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDistance, groundLayer))
        {
            return hit.point.y;
        }
        return transform.position.y; // Retorna a posição atual se não encontrar chão
    }
}
