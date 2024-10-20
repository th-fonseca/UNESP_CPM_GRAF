using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Import necessário para manipular a UI

public class AmbulanceLights : MonoBehaviour
{
    public Material headLights;
    public Material blueLights;

    public Light leftHeadLight;
    public Light rightHeadLight;

    public AudioClip sirenSound;
    public AudioSource sirenAudioSource;

    public Image cooldownImage; // Referência à imagem de cooldown

    private bool isSirenOn = false;
    private bool isHeadlightOn = false;
    private Coroutine sirenCoroutine;
    private float lastLightToggleTime = 0f;

    // Parâmetros de duração e cooldown
    private float sirenDuration = 5f; // Duração da sirene em segundos
    private float sirenCooldown = 7f; // Tempo de cooldown em segundos
    private bool isCooldownActive = false;
    private float cooldownTimer = 0f; // Para controlar o tempo restante de cooldown

    void Start()
    {
        SetLightEmission(headLights, isHeadlightOn, Color.white, Color.black);
        SetLightEmission(blueLights, isSirenOn, Color.cyan, Color.black);
        leftHeadLight.enabled = false;
        rightHeadLight.enabled = false;

        sirenAudioSource.clip = sirenSound;

        // Inicializa a imagem de cooldown como cheia (pronto para uso)
        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 1f;
        }
    }

    void Update()
    {
        HandleLightToggle();
        HandleSirenToggle();

        // Atualiza o cooldown visualmente se estiver ativo
        if (isCooldownActive && cooldownImage != null)
        {
            cooldownTimer += Time.deltaTime;
            cooldownImage.fillAmount = cooldownTimer / sirenCooldown; // Barra enche à medida que o cooldown avança
        }
    }

    // Alterna as luzes
    void HandleLightToggle()
    {
        if (Input.GetKey(KeyCode.F) && Time.time - lastLightToggleTime > 0.5f)
        {
            isHeadlightOn = !isHeadlightOn;
            SetLightEmission(headLights, isHeadlightOn, Color.white, Color.black);
            lastLightToggleTime = Time.time;
            leftHeadLight.enabled = isHeadlightOn;
            rightHeadLight.enabled = isHeadlightOn;
        }
    }

    // Alterna a sirene com cooldown e duração
    void HandleSirenToggle()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isCooldownActive)
        {
            if (!isSirenOn)
            {
                // Ativa a sirene e inicia a contagem de duração
                isSirenOn = true;
                sirenCoroutine = StartCoroutine(BlinkSirenLights());
                sirenAudioSource.Play();
                StartCoroutine(SirenDurationRoutine());
            }
            else
            {
                // Desliga a sirene manualmente e inicia o cooldown
                TurnOffSiren();
            }
        }
    }

    // Rotina que controla a duração da sirene
    IEnumerator SirenDurationRoutine()
    {
        yield return new WaitForSeconds(sirenDuration); // Espera a duração da sirene

        // Desliga a sirene após o tempo definido
        TurnOffSiren();
    }

    // Função para desligar a sirene
    void TurnOffSiren()
    {
        if (isSirenOn)
        {
            isSirenOn = false;
            if (sirenCoroutine != null) StopCoroutine(sirenCoroutine);
            SetLightEmission(blueLights, false, Color.cyan, Color.black); // Apaga as luzes da sirene
            sirenAudioSource.Stop();

            // Inicia o cooldown
            StartCoroutine(SirenCooldownRoutine());
        }
    }

    // Rotina que controla o cooldown da sirene
    IEnumerator SirenCooldownRoutine()
    {
        isCooldownActive = true;
        cooldownTimer = 0f; // Reinicia o temporizador de cooldown

        if (cooldownImage != null)
        {
            cooldownImage.fillAmount = 0f; // A barra começa vazia quando o cooldown começa
        }

        // Atualiza a barra de cooldown até encher novamente
        while (cooldownTimer < sirenCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownImage != null)
            {
                cooldownImage.fillAmount = cooldownTimer / sirenCooldown;
            }
            yield return null;
        }

        // Quando o cooldown acabar, a barra está cheia
        isCooldownActive = false;
        cooldownImage.fillAmount = 1f; // A barra volta a ficar cheia
    }

    // Função de piscar as luzes da sirene
    IEnumerator BlinkSirenLights()
    {
        while (isSirenOn) // Certifica-se de que só pisque enquanto a sirene está ligada
        {
            blueLights.SetColor("_EmissionColor", Color.cyan);
            yield return new WaitForSeconds(0.1f); // Tempo de piscar

            blueLights.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(0.1f); // Tempo de piscar
        }
    }

    // Função auxiliar para definir a emissão de luz
    void SetLightEmission(Material materialToLight, bool shouldBeLit, Color lightOn, Color lightOff)
    {
        materialToLight.SetColor("_EmissionColor", shouldBeLit ? lightOn : lightOff);
        materialToLight.EnableKeyword("_EMISSION");
    }

    public bool IsSirenActive() { return isSirenOn; }
}
