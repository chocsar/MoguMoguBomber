using System;
using UnityEngine;

[Serializable]
public class PlayerManager
{
    public Color playerColor;
    public Transform spawnPoint;
    [HideInInspector] public int playerNumber;
    [HideInInspector] public string coloredPlayerText;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public int wins;


    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;
    private GameObject canvasGameObject;


    public void Setup()
    {
        playerMovement = instance.GetComponent<PlayerMovement>();
        playerAttack = instance.GetComponent<PlayerAttack>();
        canvasGameObject = instance.GetComponentInChildren<Canvas>().gameObject;

        playerMovement.playerNumber = playerNumber;
        playerAttack.playerNumber = playerNumber;

        coloredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(playerColor) + ">PLAYER " + playerNumber + "</color>";

        SkinnedMeshRenderer[] renderers = instance.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].color = playerColor;      //今回はSkinnedMeshRendererが一つ、その中のMaterialが複数あった！
            }
        }
    }

    public void DisableControl()
    {
        playerMovement.enabled = false;
        playerAttack.enabled = false;

        canvasGameObject.SetActive(false);
    }

    public void EnableControl()
    {
        playerMovement.enabled = true;
        playerAttack.enabled = true;

        canvasGameObject.SetActive(true);
    }

    public void Reset()
    {
        instance.transform.position = spawnPoint.position;
        instance.transform.rotation = spawnPoint.rotation;

        instance.SetActive(false);
        instance.SetActive(true);
    }
}
