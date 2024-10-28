using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmbulanceHealth : MonoBehaviour
{
    public float maxHealth;
    private float animationDuration = 0.25f;
    private float maxDamage = 15f;
    private float damageTime = 0f;
    private float currentSpeed = 0f;

    public Image healthBar;
    public float healthValue = 50f;

    public Collider ambulanceColider;
    public Rigidbody ambulanceRigidbody;

    public AudioClip collisionSound; // Som de colisão
    public AudioClip stopSound; // Som específico para a tag "Stop"
    public AudioSource audioSource; // Fonte de áudio

    private AmbulanceLights ambulanceLights;

    void Start()
    {
        ambulanceLights = GetComponent<AmbulanceLights>();
        maxHealth = healthValue;
    }

    void Update()
    {
        currentSpeed = ambulanceRigidbody.velocity.magnitude * 3.6f; // Converte para km/h
    }

    void OnTriggerEnter(Collider other)
    {
        float damageTaken;
        float newHealthValue;

        // Colisão regular (não é "Stop")
        if (!other.CompareTag("Stop"))
        {
            damageTaken = (1f * currentSpeed) > maxDamage ? maxDamage : (0.5f * currentSpeed);
            newHealthValue = Mathf.Clamp(healthValue - damageTaken, 0f, maxHealth);

            // Se passar 1 segundo desde o último dano
            if (Time.time - damageTime > 1f)
            {
                StartCoroutine(HealthChange(healthValue, newHealthValue));
                healthValue = newHealthValue;
                damageTime = Time.time;
                if(audioSource.enabled) audioSource.PlayOneShot(collisionSound);
            }
        }
        // Colisão com a tag "Stop" e sirene desligada
        else if (other.CompareTag("Stop") && !ambulanceLights.IsSirenActive())
        {
            damageTaken = 10f;
            newHealthValue = Mathf.Clamp(healthValue - damageTaken, 0f, maxHealth);

            if (Time.time - damageTime > 1f)
            {
                StartCoroutine(HealthChange(healthValue, newHealthValue));
                healthValue = newHealthValue;
                damageTime = Time.time;
                if (audioSource.enabled) audioSource.PlayOneShot(stopSound);
                
            }
        }
    }

    IEnumerator HealthChange(float oldHealth, float newHealth)
    {
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentHealth = Mathf.Lerp(oldHealth, newHealth, elapsedTime / animationDuration);
            UpdateHealthBar(currentHealth);
            yield return null;
        }
        UpdateHealthBar(newHealth);
    }

    void UpdateHealthBar(float health)
    {
        float healthAmount = (health / maxHealth) * 180.0f / 360;
        healthBar.fillAmount = healthAmount;
    }

}
