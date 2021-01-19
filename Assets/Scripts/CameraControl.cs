using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float dampTime = 0.2f;           //移動に時間を持たせる      
    public float screenEdgeBuffer = 4f;           //画面端のバッファ
    public float minSize = 6.5f;                  //どこまでも小さくならないように最小サイズを設定
    [HideInInspector] public Transform[] Players; //プレイヤーの配列 (GameManagerから設定)

    private Camera mainCamera;
    private float zoomSpeed;
    private Vector3 moveVelocity;
    private Vector3 desiredPosition;//目的とするPosition           


    private void Awake()
    {
        mainCamera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate() //Playerの移動がFixedUpdateなので
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        //平均の位置を計算
        FindAveragePosition();

        //スムーズに移動する
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < Players.Length; i++)
        {
            //activeなPlayerのみ計算
            if (!Players[i].gameObject.activeSelf) continue;

            averagePos += Players[i].position;
            numTargets++;
        }

        //平均の位置を計算
        if (numTargets > 0) averagePos /= numTargets;

        //yの値は変えない
        averagePos.y = transform.position.y;

        desiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();

        //スムーズにズームする
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, requiredSize, ref zoomSpeed, dampTime);
    }


    private float FindRequiredSize()
    {
        //カメラからみた目的位置
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

        float size = 0f;

        for (int i = 0; i < Players.Length; i++)
        {
            //activeなPlayerのみ計算
            if (!Players[i].gameObject.activeSelf) continue;

            //カメラから見たプレイヤーの位置
            Vector3 targetLocalPos = transform.InverseTransformPoint(Players[i].position);
            //方向ベクトルを計算
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            //最大のサイズを求める
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / mainCamera.aspect);//アスペクトで割ることでxとyを同等に扱えるようになる
        }

        //サイズにバッファを加える
        size += screenEdgeBuffer;
        //最小サイズを確認
        size = Mathf.Max(size, minSize);

        return size;
    }


    public void SetStartPositionAndSize() //ゲーム開始時はSmoothDampを使わず、瞬時に移動させる (外部から呼び出す用)
    {
        FindAveragePosition();
        transform.position = desiredPosition;
        mainCamera.orthographicSize = FindRequiredSize();
    }
}