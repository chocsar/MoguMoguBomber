using System;
using UnityEngine;

[Serializable]
public class PlayerManager
{
    public Color m_PlayerColor;                             // This is the color this tank will be tinted.
    public Transform m_SpawnPoint;                          // The position and direction the tank will have when it spawns.
    [HideInInspector] public int m_PlayerNumber;            // This specifies which player this the manager for.
    [HideInInspector] public string m_ColoredPlayerText;    // A string that represents the player with their number colored to match their tank.
    [HideInInspector] public GameObject m_Instance;         // A reference to the instance of the tank when it is created.
    [HideInInspector] public int m_Wins;                    // The number of wins this player has so far.


    private PlayerMovement m_Movement;                        // Reference to tank's movement script, used to disable and enable control.
    private PlayerAttack m_Attack;                        // Reference to tank's shooting script, used to disable and enable control.
    private GameObject m_CanvasGameObject;                  // Used to disable the world space UI during the Starting and Ending phases of each round.


    public void Setup()
    {
        // Get references to the components.
        m_Movement = m_Instance.GetComponent<PlayerMovement>();
        m_Attack = m_Instance.GetComponent<PlayerAttack>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        // Set the player numbers to be consistent across the scripts.
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Attack.m_PlayerNumber = m_PlayerNumber;

        // Create a string using the correct color that says 'PLAYER 1' etc based on the tank's color and the player's number.
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        // Get all of the renderers of the tank.
        SkinnedMeshRenderer[] renderers = m_Instance.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
           Material[] materials = renderers[i].materials;
           for(int j = 0;j < materials.Length;j++)
           {
               materials[j].color = m_PlayerColor;      //今回はSkinndeMeshRendererが一つ、その中のMaterialが複数あった！
           }
        }
    }


    // Used during the phases of the game where the player shouldn't be able to control their tank.
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Attack.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    // Used during the phases of the game where the player should be able to control their tank.
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Attack.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    // Used at the start of each round to put the tank into it's default state.
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
