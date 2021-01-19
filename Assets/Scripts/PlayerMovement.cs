using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    public int playerNumber = 1;
    public float speed = 5;


    private Rigidbody playerRigidbody;
    private float movementInputValueY;
    private float movementInputValueX;
    private string movementXAxisName;
    private string movementYAxisName;

    private Animator animator;
    private float currentAngle;

    private PlayerAttack playerAttack;


    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void OnEnable()
    {
        playerRigidbody.isKinematic = false;

        movementInputValueX = 0f;
        movementInputValueY = 0f;

    }

    private void OnDisable()
    {
        playerRigidbody.isKinematic = true;
    }

    private void Start()
    {
        movementXAxisName = "Horizontal" + playerNumber;
        movementYAxisName = "Vertical" + playerNumber;

    }
    private void Update()
    {

        //入力
        movementInputValueX = Input.GetAxis(movementXAxisName);
        movementInputValueY = Input.GetAxis(movementYAxisName);

        //後ろ方向へは移動できないようにする
        // if(m_MovementInputValueY < 0) 
        // {
        //     m_MovementInputValueY = 0; 
        // } 

        //Attackアニメーション中は移動・回転しないようにする
        //アニメーションの名前 or フラグ条件　どっちが良いか？
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        //if(m_PlayerAttack.GetFireFlag())
        {
            movementInputValueX = 0;
            movementInputValueY = 0;
        }

        //アニメーション
        animator.SetFloat("Run", movementInputValueX * movementInputValueX + movementInputValueY * movementInputValueY);

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
        Vector3 movement = new Vector3(movementInputValueX, 0, movementInputValueY).normalized * speed * Time.deltaTime;

        //移動
        playerRigidbody.MovePosition(playerRigidbody.position + movement);

        //向きを回転させる
        //入力がない時でも回転を保存する
        if (movementInputValueX * movementInputValueX + movementInputValueY * movementInputValueY > 0.01f)
        {
            float targetAngle = Mathf.Atan2(movementInputValueX, movementInputValueY) * Mathf.Rad2Deg;

            currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * 10.0f);

            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
        }

    }


}
