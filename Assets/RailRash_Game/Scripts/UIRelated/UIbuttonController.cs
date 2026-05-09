using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIbuttonController : MonoBehaviour
{
   public void playGame()
    {
        SceneManager.LoadScene("level1Scene");
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void mainMenu()
    {
        Debug.Log("Main menu Clicked");
        SceneManager.LoadScene("menuScene");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("pauseScene");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("level1Scene"); // CORRECT
    }



    public void Level2Load()
    {
        SceneManager.LoadScene("level2Scene"); // CORRECT
    }

    public void pausegame()
    {
        Debug.Log("Pause Clicked");
        Time.timeScale = 0f;
        SceneManager.LoadScene("pauseScene",LoadSceneMode.Additive);
    }


}
