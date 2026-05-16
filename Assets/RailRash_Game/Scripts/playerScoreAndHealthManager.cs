using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerScoreAndHealthManager : MonoBehaviour
{
    public Stats stats;
    public canvaManager canva;
    public AudioSource AudioSource;

    // Animator Reference
    private Animator animator;

    // Player Controller Reference
    private PlayerController playerController;

    // Original Running Speed
    private float originalSpeed;

    // Only for Animal collision
    private bool isAnimalHit = false;

    private void Awake()
    {
        stats = GetComponent<Stats>();

        canva.updateHealth(stats.health);
        canva.updateScore(stats.score);

        AudioSource = GetComponent<AudioSource>();

        // Get Animator
        animator = GetComponent<Animator>();

        // Get Player Controller
        playerController = GetComponent<PlayerController>();

        // Store original speed
        originalSpeed = playerController.forwardSpeed;
    }

    private void Update()
    {
        // ONLY FOR ANIMAL HIT
        if (isAnimalHit && Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Start jump animation
            animator.SetBool("isJumping", true);

            // Remove hit animation
            animator.SetBool("isHit", false);

            // Restore speed
            playerController.forwardSpeed = originalSpeed;

            // Reset animal hit
            isAnimalHit = false;
        }

        // Stop jump animation when key released
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            animator.SetBool("isJumping", false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // ================= COIN =================
        if (other.CompareTag("Coin"))
        {
            stats.score += 1;

            canva.updateScore(stats.score);

            AudioSource coinAudio = other.GetComponent<AudioSource>();

            coinAudio.PlayOneShot(coinAudio.clip, 2f);

            Destroy(other.gameObject, 0.2f);
        }




        // ================= OBSTACLE =================
        if (other.CompareTag("Obstacle"))
        {
            UpdateHealth(-5);

            AudioSource obstacleAudio = other.GetComponent<AudioSource>();

            obstacleAudio.PlayOneShot(obstacleAudio.clip, 2f);

            Destroy(other.gameObject, 0.5f);
        }

        // ================= GIFT =================
        if (other.CompareTag("Gift"))
        {
            stats.score += 5;

            canva.updateScore(stats.score);

            AudioSource giftAudio = other.GetComponent<AudioSource>();

            giftAudio.PlayOneShot(giftAudio.clip, 2f);

            Destroy(other.gameObject, 1f);
        }

        // ================= HEALTH =================
        if (other.CompareTag("Health"))
        {
            UpdateHealth(+5);

            AudioSource healthAudio = other.GetComponent<AudioSource>();

            healthAudio.PlayOneShot(healthAudio.clip, 2f);

            Destroy(other.gameObject, 1f);
        }

        // ================= ANIMAL =================
        if (other.CompareTag("Animal"))
        {
            // Stop player
            playerController.forwardSpeed = 0f;

            // Stay in hit / idle animation
            animator.SetBool("isHit", true);

            // Enable jump recovery system
            isAnimalHit = true;

            UpdateHealth(-2);

            AudioSource animalAudio = other.GetComponent<AudioSource>();

            animalAudio.PlayOneShot(animalAudio.clip, 2f);
        }

        // ================= KILLER =================
        if (other.CompareTag("Killer"))
        {
            animator.SetBool("isHit", true);

            UpdateHealth(-999);

            AudioSource killerAudio = other.GetComponent<AudioSource>();

            killerAudio.PlayOneShot(killerAudio.clip, 2f);

            Destroy(gameObject, 0.5f);
        }

        // ================= FINISHER =================
        if (other.CompareTag("finisher"))
        {
            AudioSource finishAudio = other.GetComponent<AudioSource>();

            finishAudio.PlayOneShot(finishAudio.clip, 1f);

            Destroy(gameObject, 0.5f);

            Level1Complete();

            Debug.Log("Level 1 completed");
        }
    }

    // ================= UPDATE HEALTH =================
    public void UpdateHealth(int amount)
    {
        stats.health += amount;

        if (stats.health <= 0)
        {
            stats.health = 0;

            canva.updateHealth(stats.health);

            GameOver();

            return;
        }

        canva.updateHealth(stats.health);
    }

    // ================= GAME OVER =================
    void GameOver()
    {
        Debug.Log("Game Over");

        Time.timeScale = 0f;

        canva.ShowGameOver(stats.score);

        Destroy(gameObject);
    }

    // ================= LEVEL COMPLETE =================
    void Level1Complete()
    {
        Debug.Log("Level Complete");

        Time.timeScale = 0f;

        canva.ShowLevelComplete(stats.score);

        Destroy(gameObject);
    }


}