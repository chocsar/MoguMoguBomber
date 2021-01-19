using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public int playerNumber = 1;

    public Transform attackTransform;
    public Rigidbody bomb;
    public Slider aimSlider;
    public float minLaunchForce = 15f; //最小でもある程度のパワーがある設定
    public float maxLaunchForce = 30f;
    public float maxChargeTime = 0.75f;//どれくらいの時間でmaxまで行くかを調整できる

    public AudioSource attackAudio;
    public AudioClip chargingClip;
    public AudioClip fireClip;

    public bool powerUp;
    public GameObject powerUpEffect;


    private string fireButton;
    private float currentLaunchForce;
    private float chargeSpeed;//チャージ速度
    private bool fired;//発射を1回だけに限定するためのフラグ

    private Animator animator;

    private int powerUpCount;



    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        currentLaunchForce = minLaunchForce;
        aimSlider.value = minLaunchForce;
        fired = false;

    }

    private void OnDisable()
    {
        powerUpCount = 0;
        powerUp = false;
        powerUpEffect.SetActive(false);
    }

    void Start()
    {
        fireButton = "Fire" + playerNumber;

        //パラメータから、変化スピードを計算する(キョリ÷時間)
        //スクリプトで使うのは時間ではなく速さ
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }

    void Update()
    {
        aimSlider.value = minLaunchForce;

        //最大までチャージした時
        if (currentLaunchForce >= maxLaunchForce && !fired)
        {
            currentLaunchForce = maxLaunchForce;

            fired = true;

            //アニメーション
            animator.SetTrigger("Attack");
        }
        else if (Input.GetButtonDown(fireButton) && !fired) //あとでGetButtonに変更する
        {
            //forceを初期値に設定
            currentLaunchForce = minLaunchForce;

            //オーディオ
            attackAudio.clip = chargingClip;
            attackAudio.Play();

        }
        else if (Input.GetButton(fireButton) && !fired)
        {
            //ボタンを押している時
            //力を増やしていく
            currentLaunchForce += chargeSpeed * Time.deltaTime;
            //UIも更新する
            aimSlider.value = currentLaunchForce;


        }
        else if (Input.GetButtonUp(fireButton) && !fired)
        {
            //ボタンを離した時
            fired = true;
            //アニメーション
            animator.SetTrigger("Attack");
        }

    }

    public void Fire()
    {
        //3way
        //AttackTransformのローカルの位置と回転を変えてから発射する
        //位置と角度の調整はもっとうまいやり方ありそう

        if (powerUp)
        {
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        attackTransform.localPosition += new Vector3(-1, 0, 0);
                        attackTransform.localRotation *= Quaternion.Euler(0, -15, 0);
                        break;

                    case 1:
                        attackTransform.localPosition += new Vector3(2, 0, 0);
                        attackTransform.localRotation *= Quaternion.Euler(0, 30, 0);
                        break;

                    case 2:
                        attackTransform.localPosition += new Vector3(-1, 0, 0);
                        attackTransform.localRotation *= Quaternion.Euler(0, -15, 0);
                        break;

                }

                //発射位置、角度を設定してインスタンス化 
                //同時にrigidbodyへの参照を格納している
                Rigidbody bombInstance = Instantiate(bomb, attackTransform.position, attackTransform.rotation) as Rigidbody;

                //爆発音を鳴らすのは一個だけにする
                if (i == 2)
                {
                    bombInstance.GetComponent<BombExplosion>().ChangeSoundFlag();
                }

                //速度の設定　→　方向 * 大きさ
                bombInstance.velocity = attackTransform.forward * currentLaunchForce;

            }
        }
        else
        {
            Rigidbody bombInstance = Instantiate(bomb, attackTransform.position, attackTransform.rotation) as Rigidbody;
            bombInstance.GetComponent<BombExplosion>().ChangeSoundFlag();
            bombInstance.velocity = attackTransform.forward * currentLaunchForce;
        }

        //オーディオ
        attackAudio.clip = fireClip;
        attackAudio.Play();

        //念の為forceを初期値に戻しておく
        currentLaunchForce = minLaunchForce;
    }

    public void ChangeFireFlag()
    {
        fired = false;
    }

    //移動のアニメーションを制御するために取得する (今は未使用)
    public bool GetFireFlag()
    {
        return fired;
    }

    public void PowerUp()
    {
        StartCoroutine(PowerUpCoroutine());
    }

    IEnumerator PowerUpCoroutine()
    {
        //追加でおにぎりとった場合に対応する
        powerUpCount++;
        //Debug.Log(m_PowerUpCount)
        ;
        powerUp = true;
        powerUpEffect.SetActive(true);

        yield return new WaitForSeconds(20f);

        powerUpCount--;
        //Debug.Log(m_PowerUpCount);

        if (powerUpCount == 0)
        {
            powerUp = false;
            powerUpEffect.SetActive(false);
        }

    }
}
