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
        SceneManager.LoadScene("menuScene");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        SceneManager.UnloadSceneAsync("pauseScene");
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // IMPORTANT
        SceneManager.UnloadSceneAsync("pauseScene");

        string current = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(current); // CORRECT
    }
    
    


    public void pausegame()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene("pauseScene",LoadSceneMode.Additive);
    }


}
