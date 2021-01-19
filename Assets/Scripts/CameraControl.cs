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
    private Vector3 desiredPosition;   //目的Position           


    private void Awake()
    {
        mainCamera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate() //タンクの移動がFixedUpdateなので
    {
        Move();
        Zoom();
    }


    private void Move()
    {
        FindAveragePosition();

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
    }


    private void FindAveragePosition()
    {
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

        for (int i = 0; i < Players.Length; i++)
        {
            if (!Players[i].gameObject.activeSelf)//activeなタンクのみ計算
                continue;

            averagePos += Players[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;//yの値は変えない

        desiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, requiredSize, ref zoomSpeed, dampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(desiredPosition);

        float size = 0f;

        for (int i = 0; i < Players.Length; i++)
        {
            if (!Players[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(Players[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / mainCamera.aspect);//アスペクトで割ることでxとyを同等に扱えるようになる
        }

        size += screenEdgeBuffer;

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