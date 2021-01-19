using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int numRoundsToWin = 5;
    public float startDelay = 3f;
    public float endDelay = 3f;
    public CameraControl cameraControl;
    public Text messageText;
    public GameObject playerPrefab;
    public PlayerManager[] playerManagers;

    public OnigiriGenerator onigiriGenerator;

    private int roundNumber;
    private WaitForSeconds startWait;
    private WaitForSeconds endWait;
    private PlayerManager roundWinner;
    private PlayerManager gameWinner;

    private void Start()
    {
        startWait = new WaitForSeconds(startDelay);
        endWait = new WaitForSeconds(endDelay);

        SpawnAllPlayers();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void SpawnAllPlayers()
    {
        for (int i = 0; i < playerManagers.Length; i++)
        {
            playerManagers[i].instance =
                Instantiate(playerPrefab, playerManagers[i].spawnPoint.position, playerManagers[i].spawnPoint.rotation) as GameObject;
            playerManagers[i].playerNumber = i + 1;
            playerManagers[i].Setup();
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[playerManagers.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = playerManagers[i].instance.transform;
        }

        cameraControl.Players = targets;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());

        yield return StartCoroutine(RoundPlaying());

        yield return StartCoroutine(RoundEnding());

        if (gameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllPlayers();
        DisablePlayerControl();

        cameraControl.SetStartPositionAndSize();

        roundNumber++;
        if (roundNumber == 1)
        {
            messageText.text = "MOGUMOGU BOMBER!\n";
            messageText.text += "ROUND " + roundNumber;
        }
        else
        {
            messageText.text = "ROUND " + roundNumber;
        }

        onigiriGenerator.GenerateOnigiri();

        yield return startWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnablePlayerControl();

        messageText.text = string.Empty;

        while (!OnePlayerLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisablePlayerControl();

        roundWinner = null;

        roundWinner = GetRoundWinner();

        if (roundWinner != null)
            roundWinner.wins++;

        gameWinner = GetGameWinner();

        string message = EndMessage();
        messageText.text = message;

        onigiriGenerator.DestroyAllOnigiri();

        yield return endWait;
    }


    private bool OnePlayerLeft()
    {
        int numPlayersLeft = 0;

        for (int i = 0; i < playerManagers.Length; i++)
        {
            if (playerManagers[i].instance.activeSelf)
                numPlayersLeft++;
        }

        return numPlayersLeft <= 1;
    }

    private PlayerManager GetRoundWinner()
    {
        for (int i = 0; i < playerManagers.Length; i++)
        {
            if (playerManagers[i].instance.activeSelf)
                return playerManagers[i];
        }

        return null;
    }


    private PlayerManager GetGameWinner()
    {
        for (int i = 0; i < playerManagers.Length; i++)
        {
            if (playerManagers[i].wins == numRoundsToWin)
                return playerManagers[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (roundWinner != null)
            message = roundWinner.coloredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < playerManagers.Length; i++)
        {
            message += playerManagers[i].coloredPlayerText + ": " + playerManagers[i].wins + " WINS\n";
        }

        if (gameWinner != null)
            message = gameWinner.coloredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllPlayers()
    {
        for (int i = 0; i < playerManagers.Length; i++)
        {
            playerManagers[i].Reset();
        }
    }


    private void EnablePlayerControl()
    {
        for (int i = 0; i < playerManagers.Length; i++)
        {
            playerManagers[i].EnableControl();
        }
    }


    private void DisablePlayerControl()
    {
        for (int i = 0; i < playerManagers.Length; i++)
        {
            playerManagers[i].DisableControl();
        }
    }
}
