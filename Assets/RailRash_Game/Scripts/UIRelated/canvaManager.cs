using UnityEngine;
using UnityEngine.UI;
public class canvaManager : MonoBehaviour
{
    public Text health;
    public Text Score;
    public GameObject gameoverPanel;
    public GameObject levelCompletePanel;
    public Text finalScoreTextOver;
    public Text finalScoreTextLevel;


    public void updateHealth(float newhealth)
    {
        health.text = "Health :"+newhealth;
    }


    public void updateScore(float newscore)
    {
        Score.text = "Score :" + newscore;
    }



    public void ShowGameOver(float score)
    {
        gameoverPanel.SetActive(true);
        finalScoreTextOver.text = "Your Score: " + score;
        Time.timeScale = 0f;
    }

    public void ShowLevelComplete(float score)
    {
        levelCompletePanel.SetActive(true);
        finalScoreTextLevel.text = "Your Score: " + score;
        Time.timeScale = 0f;
    }


}

