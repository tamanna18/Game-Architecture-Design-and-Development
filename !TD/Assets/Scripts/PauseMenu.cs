using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject menuPanel;
    public GameObject pauseMenuUI;
    public GameObject infosPanelUI;
    public GameObject gameInfosUI;
    public GameObject rewardsInfosUI;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButtonClicked();
        }
    }

    public void PauseButtonClicked()
    {
        if (GameIsPaused)
        {
            menuPanel.SetActive(false);
            Resume();
        }
        else
        {
            menuPanel.SetActive(true);
            Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("In resume");
        pauseMenuUI.SetActive(false);
        menuPanel.SetActive(false);
        gameInfosUI.SetActive(false);
        rewardsInfosUI.SetActive(false);
        infosPanelUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Restart()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        pauseMenuUI.transform.GetChild(5).gameObject.SetActive(false); //todo: remove/finish options
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

}

