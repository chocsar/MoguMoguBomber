using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    public int playerNumber = 1;

    public float minLaunchForce = 15f;//最小でもある程度のパワーがある設定
    public float maxLaunchForce = 30f;
    public float maxChargeTime = 0.75f;//どれくらいの時間でmaxまで行くか

    [SerializeField] private Transform attackTransform;//爆弾の発射位置
    [SerializeField] private Rigidbody bomb;
    [SerializeField] private Slider aimSlider;
    [SerializeField] private AudioSource attackAudio;
    [SerializeField] private AudioClip chargingClip;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private GameObject powerUpEffect;
    private Animator animator;

    private string fireButton;//発射ボタンを示す文字列
    private float currentLaunchForce;
    private float chargeSpeed;//チャージ速度
    private bool fired;//発射を1回だけに限定するためのフラグ
    private bool powerUp;
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

        //パラメータから変化スピードを計算する(キョリ÷時間)
        //スクリプトで使うのは時間ではなく速さ
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
    }

    void Update()
    {
        aimSlider.value = minLaunchForce;


        //ボタンを押した時
        if (Input.GetButtonDown(fireButton) && !fired) //あとでGetButtonに変更する
        {
            //forceを初期値に設定
            currentLaunchForce = minLaunchForce;

            //オーディオ
            attackAudio.clip = chargingClip;
            attackAudio.Play();

        }
        //ボタンを押している間
        else if (Input.GetButton(fireButton) && !fired)
        {
            //力を増やしていく
            currentLaunchForce += chargeSpeed * Time.deltaTime;

            //UIを更新する
            aimSlider.value = currentLaunchForce;

            //最大までチャージした時
            if (currentLaunchForce >= maxLaunchForce)
            {
                currentLaunchForce = maxLaunchForce;

                fired = true;

                //アニメーション
                animator.SetTrigger("Attack");
            }
        }
        //ボタンを離した時
        else if (Input.GetButtonUp(fireButton) && !fired)
        {
            fired = true;

            //アニメーション
            animator.SetTrigger("Attack");
        }

    }

    public void Fire()//アニメーションイベントで呼び出す
    {
        //3way
        //AttackTransformのローカルの位置と回転を変えてから発射する
        //位置と角度の調整はもっとうまいやり方ありそう
        if (powerUp)
        {
            for (int i = 0; i < 3; i++)
            {
                //発射位置、角度の計算
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

                //爆弾を生成
                Rigidbody bombInstance = Instantiate(bomb, attackTransform.position, attackTransform.rotation) as Rigidbody;

                //爆発音を鳴らすのは一個だけにする
                if (i == 2)
                {
                    bombInstance.GetComponent<BombExplosion>().ChangeSoundFlag();
                }

                //速度の設定 → 方向 * 大きさ
                bombInstance.velocity = attackTransform.forward * currentLaunchForce;

            }
        }
        else
        {
            Rigidbody bombInstance = Instantiate(bomb, attackTransform.position, attackTransform.rotation) as Rigidbody;
            bombInstance.GetComponent<BombExplosion>().ChangeSoundFlag();
            bombInstance.velocity = attackTransform.forward * currentLaunchForce;
        }

        //攻撃のサウンド
        attackAudio.clip = fireClip;
        attackAudio.Play();

        //念の為forceを初期値に戻しておく
        currentLaunchForce = minLaunchForce;
    }


    public void ChangeFireFlag()//アニメーションイベントで呼び出す
    {
        fired = false;
    }

    public void PowerUp()
    {
        StartCoroutine(PowerUpCoroutine());
    }

    IEnumerator PowerUpCoroutine()
    {
        //追加でおにぎりとった場合に対応する
        powerUpCount++;

        powerUp = true;
        powerUpEffect.SetActive(true);

        yield return new WaitForSeconds(20f);

        powerUpCount--;

        if (powerUpCount == 0)
        {
            powerUp = false;
            powerUpEffect.SetActive(false);
        }

    }
}
