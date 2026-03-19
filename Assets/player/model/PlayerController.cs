using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public PathManager pathManager;

    [Header("Speed")]
    public float forwardSpeed = 5f;

    [Header("Lane")]
    public float laneDistance = 2f;
    public float laneSmooth = 8f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.6f;

    private List<Transform> points;
    private int currentPointIndex = 0;
    private float moveT = 0f;

    private int currentLane = 1;
    private float currentLaneOffset = 0f;

    private bool isJumping = false;
    private float jumpTimer = 0f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (pathManager == null)
        {
            Debug.LogError("Assign PathManager to PlayerController");
            return;
        }

        points = pathManager.pathPoints;

        if (points == null || points.Count < 2)
        {
            Debug.LogError("Need at least 2 path points");
            return;
        }

        transform.position = points[0].position;

        Vector3 dir = (points[1].position - points[0].position).normalized;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    void Update()
    {
        if (points == null || points.Count < 2) return;

        HandleInput();
        MoveOnPath();
        HandleJump();

        if (animator != null)
            animator.SetBool("IsRunning", true);
    }

    void MoveOnPath()
    {
        if (currentPointIndex >= points.Count - 1)
            return;

        Transform startPoint = points[currentPointIndex];
        Transform endPoint = points[currentPointIndex + 1];

        float segmentLength = Vector3.Distance(startPoint.position, endPoint.position);

        if (segmentLength <= 0.01f)
        {
            currentPointIndex++;
            return;
        }

        moveT += (forwardSpeed / segmentLength) * Time.deltaTime;

        if (moveT >= 1f)
        {
            moveT = 0f;
            currentPointIndex++;

            if (currentPointIndex >= points.Count - 1)
            {
                Debug.Log("END OF TRACK!");
                return;
            }

            startPoint = points[currentPointIndex];
            endPoint = points[currentPointIndex + 1];
        }

        Vector3 centerPos = Vector3.Lerp(startPoint.position, endPoint.position, moveT);

        Vector3 forwardDir = (endPoint.position - startPoint.position).normalized;
        Vector3 rightDir = Vector3.Cross(Vector3.up, forwardDir).normalized;

        float targetLaneOffset = (currentLane - 1) * laneDistance;
        currentLaneOffset = Mathf.Lerp(currentLaneOffset, targetLaneOffset, laneSmooth * Time.deltaTime);

        float jumpY = 0f;
        if (isJumping)
        {
            float p = jumpTimer / jumpDuration;
            jumpY = 4f * jumpHeight * p * (1f - p);
        }

        Vector3 finalPos = centerPos + rightDir * currentLaneOffset;
        finalPos.y += jumpY;

        transform.position = finalPos;

        if (forwardDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(forwardDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentLane > 0) currentLane--;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentLane < 2) currentLane++;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (!isJumping)
            {
                isJumping = true;
                jumpTimer = 0f;

                if (animator != null)
                    animator.SetTrigger("Jump");
            }
        }
    }

    void HandleJump()
    {
        if (!isJumping) return;

        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpDuration)
        {
            jumpTimer = 0f;
            isJumping = false;
        }
    }
}