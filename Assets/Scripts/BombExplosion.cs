using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    public float maxDamage = 100f;
    public float explosionForce = 1000f;
    public float maxLifeTime = 2f;
    public float explosionRadius = 5f;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private AudioSource explosionAudio;

    private bool soundFlag = false;//爆発音を鳴らすかどうか


    private void Start()
    {
        Destroy(gameObject, maxLifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //爆発範囲内のPlayerのコライダーを取得
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, playerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            //nullの場合
            if (!targetRigidbody) continue;

            //距離に応じて力を加える
            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);


            PlayerHealth targetHealth = targetRigidbody.GetComponent<PlayerHealth>();

            //nullの場合
            if (!targetHealth) continue;

            //ダメージ計算
            float damage = CalculateDamage(targetRigidbody.position);
            targetHealth.TakeDamage(damage);
        }

        //エフェクト再生
        explosionParticle.transform.parent = null;
        explosionParticle.Play();

        //オーディオ再生
        if (soundFlag)
        {
            explosionAudio.Play();
        }

        //エフェクトの破壊(パーティクルの再生時間分を待ってから)
        Destroy(explosionParticle.gameObject, explosionParticle.main.duration);

        //自らの破壊
        Destroy(gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        //方向ベクトル計算
        Vector3 explosionToTarget = targetPosition - transform.position;
        //長さ計算
        float explosionDistance = explosionToTarget.magnitude;
        //半径に対する割合計算
        float relativeDistance = (explosionRadius - explosionDistance) / explosionRadius;
        //ダメージ計算
        float damage = relativeDistance * maxDamage;

        //エッジケース対策（負になる場合がある）
        damage = Mathf.Max(0f, damage);

        return damage;
    }

    public void ChangeSoundFlag()
    {
        soundFlag = true;
    }


}
