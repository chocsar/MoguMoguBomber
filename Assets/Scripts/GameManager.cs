using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int numRoundsToWin = 5;
    public float startDelay = 3f;
    public float endDelay = 3f;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PlayerManager[] playerManagers;
    [SerializeField] private OnigiriGenerator onigiriGenerator;
    [SerializeField] private CameraControl cameraControl;
    [SerializeField] private Text messageText;

    private int roundNumber;
    private WaitForSeconds startWait;//ラウンドスタート時の待機時間
    private WaitForSeconds endWait;//ラウンド終了時の待機時間
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

        //ゲームの終了
        if (gameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        //ラウンドの終了
        else
        {
            //次のラウンドへ
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllPlayers();
        DisablePlayerControl();

        cameraControl.SetStartPositionAndSize();

        roundNumber++;

        //メッセージテキストを表示
        if (roundNumber == 1)
        {
            messageText.text = "MOGUMOGU BOMBER!\n";
            messageText.text += "ROUND " + roundNumber;
        }
        else
        {
            messageText.text = "ROUND " + roundNumber;
        }

        //おにぎり生成
        onigiriGenerator.GenerateOnigiri();

        //一定時間待機
        yield return startWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnablePlayerControl();

        messageText.text = string.Empty;

        //勝敗が決まるまで待機
        while (!OnePlayerLeft())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisablePlayerControl();

        //ラウンド勝利者の取得
        roundWinner = null;
        roundWinner = GetRoundWinner();
        if (roundWinner != null) roundWinner.wins++;

        //ゲーム勝利者の取得
        gameWinner = GetGameWinner();

        //メッセージテキストの表示
        string message = EndMessage();
        messageText.text = message;

        //おにぎりの破壊
        onigiriGenerator.DestroyAllOnigiri();

        //一定時間待機
        yield return endWait;
    }

    /// <summary>
    /// プレイヤーが一人以下になったかどうか調べる
    /// </summary>
    /// <returns>プレイヤーが一人以下になったかどうか</returns>
    private bool OnePlayerLeft()
    {
        int numPlayersLeft = 0;

        //アクティブなプレイヤーをカウントする
        for (int i = 0; i < playerManagers.Length; i++)
        {
            if (playerManagers[i].instance.activeSelf) numPlayersLeft++;
        }

        return numPlayersLeft <= 1;
    }

    /// <summary>
    /// ラウンドの勝利者を取得する
    /// </summary>
    /// <returns>ラウンド勝利者のPlayerManager</returns>
    private PlayerManager GetRoundWinner()
    {
        //アクティブなプレイヤーを検索する
        for (int i = 0; i < playerManagers.Length; i++)
        {
            if (playerManagers[i].instance.activeSelf)
            {
                return playerManagers[i];
            }
        }

        return null;
    }

    /// <summary>
    /// ゲーム勝利者を取得する
    /// </summary>
    /// <returns>ゲーム勝利者のPlayerManger</returns>
    private PlayerManager GetGameWinner()
    {
        for (int i = 0; i < playerManagers.Length; i++)
        {
            if (playerManagers[i].wins == numRoundsToWin)
                return playerManagers[i];
        }

        return null;
    }

    /// <summary>
    /// ラウンド終了時に表示する文字列を作成する
    /// </summary>
    /// <returns>ラウンド終了時に表示する文字列</returns>
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
