using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameIdentifier { MainGame, MiniGameLeft, MiniGameRight }
public enum MiniGame { GravityBalls, WhackAMole }
public class GameManager : MonoBehaviour
{
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }
    [Header("UI Elements")]
    public TextMeshProUGUI timerBox;
    public float selectGameTimer = 10f;
    public TextMeshProUGUI timerBoxLeft;
    public TextMeshProUGUI timerBoxRight;
    public GameObject lockRight;
    public GameObject lockLeft;
    public GameObject menuPanel;
    public GameObject endGameScreen;
    public Camera cameraMainGame, cameraMiniGameLeft, cameraMiniGameRight;
    public SpriteRenderer mainGameBackground, miniGameLeftBackground, miniGameRightBackground;
    [Header("Global Game Variables")]
    public float timer = 60;
    bool gameHasEnded = false;
    public float restartDelay = 1f;
    public int miniGamesPlayed;
    public int miniGamesWon;
    public int miniGamesLost;
    public float minigameTimer = 30f;
    public float bonusLimit;
    public bool minigameEnded = false;
    public bool minigameSelected = false;
    bool isBonusPossible = false;
    public bool minigameLost = false;
    public bool isBonus = false;

    void Awake()
    {
        cameraMainGame.transform.position = new Vector3(mainGameBackground.transform.position.x, mainGameBackground.transform.position.y, cameraMainGame.transform.position.z);
        cameraMainGame.orthographicSize = mainGameBackground.bounds.size.y * 0.5f;

        cameraMiniGameLeft.transform.position = new Vector3(miniGameLeftBackground.transform.position.x, miniGameLeftBackground.transform.position.y, cameraMiniGameLeft.transform.position.z);
        cameraMiniGameLeft.orthographicSize = miniGameLeftBackground.bounds.size.y * 0.5f;

        cameraMiniGameRight.transform.position = new Vector3(miniGameRightBackground.transform.position.x, miniGameRightBackground.transform.position.y, cameraMiniGameRight.transform.position.z);
        cameraMiniGameRight.orthographicSize = miniGameRightBackground.bounds.size.y * 0.5f;

    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            Time.timeScale = 0f;
            PauseMenu.GameIsPaused = true;
        }
        lockLeft.SetActive(false);
        lockRight.SetActive(false);
        StartMiniGames();
        StartCoroutine(GameTimer());
        StartCoroutine(SelectMinigameTimer());

    }

    IEnumerator GameTimer()
    {
        while (timer >= 0)
        {
            string minutes = Mathf.Floor(timer / 60).ToString("00");
            string seconds = Mathf.RoundToInt(timer % 60).ToString("00");
            timerBox.text = minutes + ":" + seconds;
            if (timer == 0) EndGame(false);
            yield return new WaitForSeconds(1);
            timer--;
        }
    }


    IEnumerator SelectMinigameTimer()
    {
        string minutes = Mathf.Floor(selectGameTimer / 60).ToString("00");
        string seconds = Mathf.RoundToInt(selectGameTimer % 60).ToString("00");
        timerBoxLeft.text = minutes + ":" + seconds;
        timerBoxRight.text = minutes + ":" + seconds;
        while (selectGameTimer >= 0 && !minigameSelected && !minigameLost)
        {
            if (minigameEnded)
            {
                minigameEnded = false;
            }
            minutes = Mathf.Floor(selectGameTimer / 60).ToString("00");
            seconds = Mathf.RoundToInt(selectGameTimer % 60).ToString("00");
            timerBoxLeft.text = minutes + ":" + seconds;
            timerBoxRight.text = minutes + ":" + seconds;
            if (selectGameTimer == 0)
            {
                lockLeft.SetActive(true);
                lockRight.SetActive(true);
                RestartMinigames();
                StartCoroutine(RewardSystem.Instance.PickTimer());
            }
            yield return new WaitForSeconds(1);
            selectGameTimer--;
        }
    }

    public void startMinigameTimer(GameIdentifier gameIdentifier)
    {
        StartCoroutine(ShowMinigameTimer(gameIdentifier));
    }
    IEnumerator ShowMinigameTimer(GameIdentifier gameIdentifier)
    {

        if (minigameTimer % 2 == 1)
        {
            bonusLimit = (minigameTimer - 1) / 2;
        }
        else
        {
            bonusLimit = minigameTimer / 2;
        }
        while (minigameTimer >= 0 && minigameSelected && !minigameLost)
        {
            if (minigameTimer == 0 || minigameEnded)
            {
                minigameSelected = false;
                RestartMinigames();
                StartCoroutine(SelectMinigameTimer());
                break;
            }
            string minutes = Mathf.Floor(minigameTimer / 60).ToString("00");
            float seconds = Mathf.RoundToInt(minigameTimer % 60);
            timerBoxLeft.text = minutes + ":" + seconds.ToString("00");
            timerBoxRight.text = minutes + ":" + seconds.ToString("00");
            if (gameIdentifier == GameIdentifier.MiniGameLeft)
            {
                isBonusPossible = RewardSystem.Instance.isBonusPossibleLeft;
            }
            else
            {
                isBonusPossible = RewardSystem.Instance.isBonusPossibleRight;
            }

            if (bonusLimit >= 0 && isBonusPossible)
            {
                timerBoxLeft.text = timerBoxLeft.text + System.Environment.NewLine + "<color=green>" + (minutes + ":" + Mathf.RoundToInt(bonusLimit % 60).ToString("00")).ToString() + "</color>";
                timerBoxRight.text = timerBoxRight.text + System.Environment.NewLine + "<color=green>" + (minutes + ":" + Mathf.RoundToInt(bonusLimit % 60).ToString("00")).ToString() + "</color>";
                isBonus = true;
            }
            else
            {
                isBonus = false;
            }

            yield return new WaitForSeconds(1);
            minigameTimer--;
            bonusLimit--;
        }
        if (minigameLost)
        {
            lockLeft.SetActive(true);
            lockRight.SetActive(true);

            if (gameIdentifier == GameIdentifier.MiniGameLeft)
            {
                Instance.lockLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Loss";
                Instance.lockLeft.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else
            {
                Instance.lockRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Loss";
                Instance.lockRight.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
            }

            minigameSelected = false;
            RestartMinigames();
            StartCoroutine(RewardSystem.Instance.PickTimer());
            StartCoroutine(SelectMinigameTimer());
        }
    }
    public void EndGame(bool isWin)
    {
        if (!gameHasEnded)
        {
            Debug.Log("GAME WON");
            gameHasEnded = true;
            menuPanel.SetActive(true);
            endGameScreen.SetActive(true);
            if (isWin)
            {
                endGameScreen.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "YOU WON!";
                endGameScreen.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
                if (SceneManager.GetActiveScene().buildIndex == 10)
                {
                    endGameScreen.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "YOU WON THE GAME!";
                    endGameScreen.transform.GetChild(0).GetChild(1).GetChild(4).GetChild(1).gameObject.SetActive(false);
                }
            }
            else
            {
                endGameScreen.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "YOU LOST!";
                endGameScreen.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;
                endGameScreen.transform.GetChild(0).GetChild(1).GetChild(4).GetChild(1).gameObject.SetActive(false);
            }
            string minutes = Mathf.Floor(timer / 60).ToString("00");
            string seconds = Mathf.RoundToInt(timer % 60).ToString("00");
            endGameScreen.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "TIME LEFT: " + minutes + ":" + seconds;
            endGameScreen.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = "MINIGAMES PLAYED: " + miniGamesPlayed;
            endGameScreen.transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = "MINIGAMES WON: " + miniGamesWon;
            endGameScreen.transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text = "MINIGAMES LOST: " + miniGamesLost;
        }
    }
    void RestartMinigames()
    {
        isBonus = false;
        minigameLost = false;
        moleClickHandler.rightTimerSet = false;
        BallScript.leftTimerSet = false;
        selectGameTimer = 11f;
        minigameTimer = 30f;
        minigameEnded = false;
    }

    void StartMiniGames()
    {
        Debug.Log("START MINIGAMES: ");
        QuestionSystem.Instance.InitializeQuestions(MiniGame.GravityBalls, MiniGame.WhackAMole);
        BallsSpawner.Instance.StartBallsSpawner(GameIdentifier.MiniGameLeft);
        MoleSpawner.Instance.startMoleSpawning(GameIdentifier.MiniGameRight);
        RewardSystem.Instance.InitializeRewards();
    }

    public string GetObjectTagForMinigame(MiniGame minigame)
    {
        switch (minigame)
        {
            case MiniGame.GravityBalls:
                return "Ball";
            case MiniGame.WhackAMole:
                return "Mole";
            default:
                return "Ball";
        }
    }

    public void StartMinigame(GameIdentifier gameIdentifier)
    {
        if (gameIdentifier == GameIdentifier.MiniGameLeft && !GameManager.Instance.lockRight.activeSelf)
        {
            GameManager.Instance.lockRight.SetActive(true);
            GameManager.Instance.miniGamesPlayed++;
        }
        else if (gameIdentifier == GameIdentifier.MiniGameRight && !GameManager.Instance.lockLeft.activeSelf)
        {
            GameManager.Instance.lockLeft.SetActive(true);
            GameManager.Instance.miniGamesPlayed++;
        }
    }
}
