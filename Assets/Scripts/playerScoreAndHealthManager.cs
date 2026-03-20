using System;
using Unity.VisualScripting;
using UnityEngine;

public class playerScoreAndHealthManager : MonoBehaviour
{
    public Stats stats;
    public canvaManager canva;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        stats = GetComponent<Stats>();
        canva.updateHealth(stats.health);
        canva.updateScore(stats.score);
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            stats.score = stats.score + 1;
            canva.updateScore(stats.score);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Gift"))
        {
            stats.score = stats.score + 5;
            canva.updateScore(stats.score);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Health"))
        {
            stats.health = stats.health + 5;
            canva.updateHealth(stats.health);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Animal"))
        {
            stats.health = stats.health - 2;
            canva.updateHealth(stats.health);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Killer"))
        {
            stats.health = 0;
            canva.updateHealth(stats.health);

            Destroy(other.gameObject);
        }

    }
}
