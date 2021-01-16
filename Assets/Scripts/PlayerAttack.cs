using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public int m_PlayerNumber = 1;

    public Transform m_AttackTransform;
    public Rigidbody m_Bomb;
    public Slider m_AimSlider;
    public float m_MinLaunchForce = 15f; //最小でもある程度のパワーがある設定
    public float m_MaxLaunchForce = 30f;
    public float m_MaxChargeTime = 0.75f;//どれくらいの時間でmaxまで行くかを調整できる

    public AudioSource m_AttackAudio;
    public AudioClip m_ChargingClip;
    public AudioClip m_FireClip;

    public bool m_PowerUp;
    public GameObject m_PowerUpEffect;


    private string m_FireButton;
    private float m_CurrentLaunchForce;
    private float m_ChargeSpeed;//チャージ速度
    private bool m_Fired;//発射を1回だけに限定するためのフラグ

    private Animator m_Animator;

    private int m_PowerUpCount;



    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
        m_Fired = false;      

    }

    private void OnDisable()
    {
        m_PowerUpCount = 0;
        m_PowerUp = false;
        m_PowerUpEffect.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;


        //パラメータから、変化スピードを計算する(キョリ÷時間)
        //スクリプトで使うのは時間ではなく速さ
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }

    // Update is called once per frame
    void Update()
    {


        m_AimSlider.value = m_MinLaunchForce;

        //最大までチャージした時
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            m_CurrentLaunchForce = m_MaxLaunchForce;

            m_Fired = true;

            //アニメーション
            m_Animator.SetTrigger("Attack");
        }
        else if (Input.GetButtonDown(m_FireButton) && !m_Fired) //あとでGetButtonに変更する
        {
            //forceを初期値に設定
            m_CurrentLaunchForce = m_MinLaunchForce;

            //オーディオ
            m_AttackAudio.clip = m_ChargingClip;
            m_AttackAudio.Play();

        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            //ボタンを押している時
            //力を増やしていく
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
            //UIも更新する
            m_AimSlider.value = m_CurrentLaunchForce;


        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            //ボタンを離した時
            m_Fired = true;
            //アニメーション
            m_Animator.SetTrigger("Attack");
        }

    }

    public void Fire()
    {
        //3way
        //AttackTransformのローカルの位置と回転を変えてから発射する
        //位置と角度の調整はもっとうまいやり方ありそう

        if (m_PowerUp)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        m_AttackTransform.localPosition += new Vector3(-1, 0, 0);
                        m_AttackTransform.localRotation *= Quaternion.Euler(0, -15, 0);
                        break;

                    case 1:
                        m_AttackTransform.localPosition += new Vector3(2, 0, 0);
                        m_AttackTransform.localRotation *= Quaternion.Euler(0, 30, 0);
                        break;

                    case 2:
                        m_AttackTransform.localPosition += new Vector3(-1, 0, 0);
                        m_AttackTransform.localRotation *= Quaternion.Euler(0, -15, 0);
                        break;

                }

                //発射位置、角度を設定してインスタンス化 
                //同時にrigidbodyへの参照を格納している
                Rigidbody bombInstance = Instantiate(m_Bomb, m_AttackTransform.position, m_AttackTransform.rotation) as Rigidbody;

                //爆発音を鳴らすのは一個だけにする
                if (i == 2)
                {
                    bombInstance.GetComponent<BombExplosion>().ChangeSoundFlag();
                }

                //速度の設定　→　方向 * 大きさ
                bombInstance.velocity = m_AttackTransform.forward * m_CurrentLaunchForce;

            }
        }
        else
        {
            Rigidbody bombInstance = Instantiate(m_Bomb, m_AttackTransform.position, m_AttackTransform.rotation) as Rigidbody;
            bombInstance.GetComponent<BombExplosion>().ChangeSoundFlag();
            bombInstance.velocity = m_AttackTransform.forward * m_CurrentLaunchForce;
        }

        //オーディオ
        m_AttackAudio.clip = m_FireClip;
        m_AttackAudio.Play();

        //念の為forceを初期値に戻しておく
        m_CurrentLaunchForce = m_MinLaunchForce;
    }

    public void ChangeFireFlag()
    {
        m_Fired = false;
    }

    //移動のアニメーションを制御するために取得する (今は未使用)
    public bool GetFireFlag()
    {
        return m_Fired;
    }

    public void PowerUp()
    {
        StartCoroutine(PowerUpCoroutine());
    }

    IEnumerator PowerUpCoroutine()
    {
        //追加でおにぎりとった場合に対応する
        m_PowerUpCount++;
        //Debug.Log(m_PowerUpCount)
;
        m_PowerUp = true;
        m_PowerUpEffect.SetActive(true);

        yield return new WaitForSeconds(20f);

        m_PowerUpCount--;
        //Debug.Log(m_PowerUpCount);

        if(m_PowerUpCount == 0)
        {
            m_PowerUp = false;
            m_PowerUpEffect.SetActive(false);
        }

    }
}
