using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onigiri : MonoBehaviour
{
    [SerializeField] private ParticleSystem getParticleEffect;
    [SerializeField] private AudioSource getAudioSource;
    private OnigiriGenerator onigirigenerator;

    void Start()
    {
        onigirigenerator = GameObject.Find("OnigiriGenerator").GetComponent<OnigiriGenerator>();
    }

    void Update()
    {
        //アイテムっぽく回転させる
        transform.Rotate(new Vector3(0, 90 * Time.deltaTime, 0));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //プレイヤーのパワーアップ
            other.GetComponent<PlayerAttack>().PowerUp();

            //エフェクト再生
            getParticleEffect.transform.parent = null;
            getParticleEffect.Play();
            getAudioSource.Play();
            Destroy(getParticleEffect.gameObject, getParticleEffect.main.duration);

            //おにぎりの破壊
            onigirigenerator.generatedOnigiriList.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
