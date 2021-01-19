using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public LayerMask playerMask;
    public ParticleSystem explosionParticles;
    public float maxDamage = 100f;
    public float explosionForce = 1000f;
    public float maxLifeTime = 2f;
    public float explosionRadius = 5f;

    public AudioSource explosionAudio;

    private bool soundFlag = false;


    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, playerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            PlayerHealth targetHealth = targetRigidbody.GetComponent<PlayerHealth>();

            if (!targetHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.TakeDamage(damage);
        }

        //エフェクト→まずは親子関係を外す
        explosionParticles.transform.parent = null;

        //パーティクルの再生
        explosionParticles.Play();

        //オーディオ
        if (soundFlag)
        {
            explosionAudio.Play();
        }

        //エフェクトの破壊(パーティクルの再生時間分を待ってから)
        Destroy(explosionParticles.gameObject, explosionParticles.main.duration);


        // Destroy the shell.
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;

        float damage = relativeDistance * maxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }

    public void ChangeSoundFlag()
    {
        soundFlag = true;
    }


}
