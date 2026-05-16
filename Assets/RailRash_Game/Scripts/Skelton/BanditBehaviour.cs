using UnityEngine;
using UnityEngine.AI;

namespace RPGGame
{
    public class BanditBehaviour : MonoBehaviour
    {
        // =========================
        // PLAYER REFERENCES
        // =========================

        private PlayerController m_Target;
        private playerScoreAndHealthManager playerHealth;

        // =========================
        // COMPONENTS
        // =========================

        private NavMeshAgent m_NavMeshAgent;
        private Animator animator;

        // AUDIO
        private AudioSource audioSource;

        // =========================
        // DETECTION SETTINGS
        // =========================

        [Header("Detection")]
        public float detectionRadius = 10.0f;
        public float detectionAngle = 180.0f;

        // =========================
        // ATTACK SETTINGS
        // =========================

        [Header("Attack")]
        public float attackRange = 3.0f;
        public int damage = 5;
        public float attackCooldown = 1.5f;

        private float nextAttackTime;

        private bool hasDetectedPlayer = false;

        // =========================
        // AWAKE
        // =========================

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            // GET AUDIO SOURCE
            audioSource = GetComponent<AudioSource>();
        }

        // =========================
        // UPDATE
        // =========================

        private void Update()
        {
            if (!hasDetectedPlayer)
            {
                m_Target = LookForPlayer();

                if (m_Target != null)
                {
                    hasDetectedPlayer = true;
                }
            }

            if (m_Target == null)
            {
                m_NavMeshAgent.isStopped = true;

                animator.SetBool("Run", false);
                animator.SetBool("Attack", false);

                return;
            }

            if (playerHealth == null)
            {
                playerHealth =
                    m_Target.GetComponent<playerScoreAndHealthManager>();
            }

            float distanceToPlayer =
                Vector3.Distance(
                    transform.position,
                    m_Target.transform.position
                );

            if (distanceToPlayer > detectionRadius * 2f)
            {
                m_Target = null;
                hasDetectedPlayer = false;

                animator.SetBool("Run", false);
                animator.SetBool("Attack", false);

                return;
            }

            if (distanceToPlayer > attackRange)
            {
                m_NavMeshAgent.isStopped = false;

                m_NavMeshAgent.SetDestination(
                    m_Target.transform.position
                );

                animator.SetBool("Run", true);
                animator.SetBool("Attack", false);
            }
            else
            {
                m_NavMeshAgent.isStopped = true;

                Vector3 direction =
                    m_Target.transform.position - transform.position;

                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    Quaternion rotation =
                        Quaternion.LookRotation(direction);

                    transform.rotation = Quaternion.Euler(
                        0,
                        rotation.eulerAngles.y,
                        0
                    );
                }

                animator.SetBool("Run", true);

                if (Time.time >= nextAttackTime)
                {
                    animator.SetBool("Attack", true);

                    Attack();

                    // 🔊 PLAY SOUND HERE (LOUD)
                    PlayAttackSound();

                    nextAttackTime =
                        Time.time + attackCooldown;

                    Invoke(nameof(StopAttackAnimation), 0.8f);
                }
            }
        }

        // =========================
        // ATTACK
        // =========================

        private void Attack()
        {
            if (playerHealth != null)
            {
                playerHealth.UpdateHealth(-damage);
                Debug.Log("Bandit attacked player!");
            }
        }

        // =========================
        // SOUND FUNCTION
        // =========================

        private void PlayAttackSound()
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.volume = 1.0f; // MAX LOUD
                audioSource.Play();
            }
        }

        private void StopAttackAnimation()
        {
            animator.SetBool("Attack", false);
        }

        // =========================
        // DETECTION
        // =========================

        private PlayerController LookForPlayer()
        {
            if (PlayerController.Instance == null)
                return null;

            Vector3 toPlayer =
                PlayerController.Instance.transform.position
                - transform.position;

            toPlayer.y = 0;

            if (toPlayer.magnitude <= detectionRadius)
            {
                if (
                    Vector3.Dot(
                        toPlayer.normalized,
                        transform.forward
                    )
                    >
                    Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad)
                )
                {
                    return PlayerController.Instance;
                }
            }

            return null;
        }

        // =========================
        // GIZMOS
        // =========================

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color c = new Color(0.8f, 0, 0, 0.4f);

            UnityEditor.Handles.color = c;

            Vector3 rotatedForward =
                Quaternion.Euler(
                    0,
                    -detectionAngle * 0.5f,
                    0
                ) * transform.forward;

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                detectionAngle,
                detectionRadius
            );
        }
#endif
    }
}