using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public float startHealth = 100f;
    public Color fullHealthColor = Color.green;
    public Color zeroHealthColor = Color.red;

    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject explosionPrefab;
    private AudioSource explosionAudio;
    private ParticleSystem explosionParticles;

    private float currentHealth;
    private bool dead;


    private void Awake()
    {
        explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
        explosionAudio = explosionParticles.GetComponent<AudioSource>();
        explosionParticles.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        currentHealth = startHealth;
        dead = false;

        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        SetHealthUI();

        if (currentHealth <= 0f && !dead)
        {
            OnDeath();
        }
    }

    private void SetHealthUI()
    {
        slider.value = currentHealth;

        //体力バーの色を変更する
        fillImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, currentHealth / startHealth);
    }

    private void OnDeath()
    {
        dead = true;

        //爆発エフェクト再生
        explosionParticles.transform.position = transform.position;
        explosionParticles.gameObject.SetActive(true);
        explosionParticles.Play();

        //爆発サウンド再生
        explosionAudio.Play();

        //プレイヤーを非アクティブにする
        gameObject.SetActive(false);
    }
}
