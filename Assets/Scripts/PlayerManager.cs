using System;
using UnityEngine;

[Serializable]
public class PlayerManager
{
    public Transform spawnPoint;
    [HideInInspector] public int playerNumber;
    [HideInInspector] public string coloredPlayerText;//Playerごとの色付きの文字列
    [HideInInspector] public GameObject instance;//Playerのインスタンス
    [HideInInspector] public int wins;//勝利数

    [SerializeField] private Color playerColor;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private GameObject canvasGameObject;//Playerが持つCanvasを保持


    public void Setup()
    {
        playerMovement = instance.GetComponent<PlayerMovement>();
        playerAttack = instance.GetComponent<PlayerAttack>();
        canvasGameObject = instance.GetComponentInChildren<Canvas>().gameObject;

        playerMovement.playerNumber = playerNumber;
        playerAttack.playerNumber = playerNumber;

        coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER " + playerNumber + "</color>";

        //Playerの色を設定
        SkinnedMeshRenderer[] renderers = instance.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].color = playerColor;      //今回はSkinnedMeshRendererが一つ、その中のMaterialが複数あった
            }
        }
    }

    public void EnableControl()
    {
        playerMovement.enabled = true;
        playerAttack.enabled = true;

        canvasGameObject.SetActive(true);
    }

    public void DisableControl()
    {
        playerMovement.enabled = false;
        playerAttack.enabled = false;

        canvasGameObject.SetActive(false);
    }

    public void Reset()
    {
        //初期位置に戻す
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;

        instance.SetActive(false);//OnDisableを呼ぶ
        instance.SetActive(true);//OnEnableを呼ぶ
    }
}
