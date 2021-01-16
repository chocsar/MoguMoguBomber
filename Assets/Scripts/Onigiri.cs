using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onigiri : MonoBehaviour
{
    public ParticleSystem m_GetParticleEffect;
    public AudioSource m_GetAudioSource;

    private OnigiriGenerator m_Onigirigenerator;

    // Start is called before the first frame update
    void Start()
    {
        m_Onigirigenerator = GameObject.Find("OnigiriGenerator").GetComponent<OnigiriGenerator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0,90 * Time.deltaTime,0));
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //プレイヤーのパワーアップ
            other.GetComponent<PlayerAttack>().PowerUp();

            //エフェクト
            m_GetParticleEffect.transform.parent = null;
            m_GetParticleEffect.Play();
            m_GetAudioSource.Play();
            Destroy(m_GetParticleEffect.gameObject,m_GetParticleEffect.main.duration);

            //おにぎりの破壊
            m_Onigirigenerator.m_GeneratedOnigiriList.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
