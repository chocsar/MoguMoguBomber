using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public float m_Speed;


    private Rigidbody m_Rigidbody;
    private float m_MovementInputValueY;
    private float m_MovementInputValueX;
    private string m_MovementXAxisName; 
    private string m_MovementYAxisName;        

    private Animator m_Animator;
    private float currentAngle;

    private PlayerAttack m_PlayerAttack;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
        m_PlayerAttack = GetComponent<PlayerAttack>();
    }

    private void OnEnable()
    {
        m_Rigidbody.isKinematic = false;

        m_MovementInputValueX = 0f;
        m_MovementInputValueY = 0f;

    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;

    }

    // Start is called before the first frame update
    private void Start()
    {
        m_MovementXAxisName = "Horizontal" + m_PlayerNumber;
        m_MovementYAxisName = "Vertical" + m_PlayerNumber;

    }

    // Update is called once per frame
    private void Update()
    {

        //入力
        m_MovementInputValueX = Input.GetAxis(m_MovementXAxisName);
        m_MovementInputValueY = Input.GetAxis(m_MovementYAxisName);
        //Debug.Log(m_MovementInputValueX  + "," + m_MovementInputValueY);


        //後ろ方向へは移動できないようにする
        // if(m_MovementInputValueY < 0) 
        // {
        //     m_MovementInputValueY = 0; 
        // } 

        //Attackアニメーション中は移動・回転しないようにする
        //アニメーションの名前 or フラグ条件　どっちが良いか？
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        //if(m_PlayerAttack.GetFireFlag())
        {
            m_MovementInputValueX = 0;
            m_MovementInputValueY = 0;
        }

        //アニメーション
        m_Animator.SetFloat("Run", m_MovementInputValueX * m_MovementInputValueX + m_MovementInputValueY * m_MovementInputValueY);

    }

    private void FixedUpdate()
    {
        Move();

        // //後ろを振り向く
        // if(Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     float duration = 0.3f;
        //     //180度回転するQuaternionを計算 → Vector3に変換
        //     Vector3 toRotation = (m_Rigidbody.rotation * Quaternion.Euler(0,180,0)).eulerAngles;
        //     transform.DORotate(toRotation,duration);
        // }
    }

    private void Move()
    {
        //移動したい方向ベクトルを計算
        //ワールド空間でのベクトルであることに注意

        //Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        //Vector3 movement = (Vector3.right * m_MovementInputValueX + Vector3.forward * m_MovementInputValueY).normalized * m_Speed * Time.deltaTime;
        Vector3 movement = new Vector3(m_MovementInputValueX, 0, m_MovementInputValueY).normalized * m_Speed * Time.deltaTime;
        //Debug.Log(movement);

        //移動
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);

        //向きを回転させる
        //入力がない時でも回転を保存する
        if (m_MovementInputValueX * m_MovementInputValueX + m_MovementInputValueY * m_MovementInputValueY > 0.01f)
        {
            float targetAngle = Mathf.Atan2(m_MovementInputValueX, m_MovementInputValueY) * Mathf.Rad2Deg;
        
            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 10.0f);

            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
        }

    }


}
