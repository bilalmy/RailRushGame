using UnityEngine;
using UnityEngine.UI;
public class canvaManager : MonoBehaviour
{
    public Text health;
    public Text Score;


    public void updateHealth(float newhealth)
    {
        health.text = "Health :"+newhealth;
    }


    public void updateScore(float newscore)
    {
        Score.text = "Score :" + newscore;
    }
}

