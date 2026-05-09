using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerScoreAndHealthManager : MonoBehaviour
{
    public Stats stats;
    public canvaManager canva;
    public AudioSource AudioSource;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        stats = GetComponent<Stats>();
        canva.updateHealth(stats.health);
        canva.updateScore(stats.score);
        AudioSource=GetComponent<AudioSource>();
    }



    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            stats.score += 1;
            canva.updateScore(stats.score);

            AudioSource coinAudio = other.GetComponent<AudioSource>();
            coinAudio.PlayOneShot(coinAudio.clip, 2f); // 🔊 louder

            Destroy(other.gameObject, 0.2f); // wait so sound plays
        }

        if (other.CompareTag("Obstacle")) //place in punch
        {
            UpdateHealth(-5);
            AudioSource coinAudio = other.GetComponent<AudioSource>();
            coinAudio.PlayOneShot(coinAudio.clip, 2f); // 🔊 louder
            Destroy(other.gameObject, 3f);


        }

        if (other.CompareTag("Gift"))
        {
            stats.score = stats.score + 5;
            canva.updateScore(stats.score);
            AudioSource coinAudio = other.GetComponent<AudioSource>();
            coinAudio.PlayOneShot(coinAudio.clip, 2f); // 🔊 louder
            Destroy(other.gameObject, 1f);

        }

        if (other.CompareTag("Health"))
        {
            UpdateHealth(+5);
            AudioSource coinAudio = other.GetComponent<AudioSource>();
            coinAudio.PlayOneShot(coinAudio.clip, 2f); // 🔊 louder
           Destroy(other.gameObject,1f);
        }

        if (other.CompareTag("Animal")) //for ice also
        {
            UpdateHealth(-2);
            AudioSource animalAudio = other.GetComponent<AudioSource>();
            animalAudio.PlayOneShot(animalAudio.clip, 2f); // 🔊 louder
            Destroy(other.gameObject);
           
        }



        if (other.CompareTag("Killer"))
        {
            UpdateHealth(-999); // force death
            AudioSource coinAudio = other.GetComponent<AudioSource>();
            coinAudio.PlayOneShot(coinAudio.clip, 2f); // 🔊 louder
           
            Destroy(gameObject, 0.5f); // wait so sound plays

        }


        if (other.CompareTag("finisher"))
        {
            AudioSource coinAudio = other.GetComponent<AudioSource>();
            coinAudio.PlayOneShot(coinAudio.clip, 1f); // 🔊 louder
            Destroy(gameObject, 0.5f); // wait so sound plays
            Level1Complete();
            Debug.Log("Level 1 completed");
           // level 1 complete seen load here ..............................................
        }

    }



    void UpdateHealth(int amount)
    {
        stats.health += amount;

        // If health goes below or equal 0
        if (stats.health <= 0)
        {
            stats.health = 0;
            canva.updateHealth(stats.health);

            GameOver(); // 🔥 MAIN FIX
            return;
        }

        canva.updateHealth(stats.health);
    }


    void GameOver()
    {
        Debug.Log("Game Over");

        // Stop game
        Time.timeScale = 0f;
        canva.ShowGameOver(stats.score);
        // Optional: destroy player
        Destroy(gameObject);
    }



    void Level1Complete()
    {
        Debug.Log("level complete ");
        // Stop game
        Time.timeScale = 0f;
        canva.ShowLevelComplete(stats.score);
        // Optional: destroy player
        Destroy(gameObject);
    }
}
