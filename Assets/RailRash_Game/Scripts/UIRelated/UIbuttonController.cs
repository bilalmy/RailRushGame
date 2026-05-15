using UnityEngine;
using UnityEngine.SceneManagement;

public class UIbuttonController : MonoBehaviour
{
    public void playGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("level1Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void mainMenu()
    {
        Time.timeScale = 1f;

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

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Level2Load()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("level2Scene");
    }

    public void pausegame()
    {
        Debug.Log("Pause Clicked");

        Time.timeScale = 0f;

        SceneManager.LoadScene("pauseScene", LoadSceneMode.Additive);
    }
}