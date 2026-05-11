using UnityEngine;
using UnityEngine.Splines;

public class PlayerController : MonoBehaviour
{
    [Header("Spline")]
    public SplineContainer splineContainer;

    [Header("Speed")]
    public float forwardSpeed = 15f;

    [Header("Lane")]
    public float laneDistance = 2f;
    public float laneSmooth = 8f;

    [Header("Jump")]
    public float jumpHeight = 2f;
    public float jumpDuration = 0.6f;

    [Header("Ground")]
    public LayerMask groundLayer;
    public float groundOffset = 1.2f;

    // ─────────────────────────────────────────────
    // Private Variables
    // ─────────────────────────────────────────────

    private float splineProgress = 0f;
    private float splineLength;

    private int targetLane = 1;
    private float smoothLaneOffset = 0f;

    private bool jumping = false;
    private float jumpTimer = 0f;

    private Animator anim;

    // ─────────────────────────────────────────────
    // Start
    // ─────────────────────────────────────────────

    void Start()
    {
        anim = GetComponent<Animator>();

        if (splineContainer == null)
        {
            Debug.LogError("Spline Container Missing!");
            return;
        }

        // Calculate spline length
        splineLength = splineContainer.CalculateLength();

        // Starting position
        Vector3 startPos =
            splineContainer.EvaluatePosition(0f);

        // Starting forward direction
        Vector3 startForward =
            ((Vector3)splineContainer.EvaluateTangent(0f)).normalized;

        transform.position = startPos;
        transform.rotation = Quaternion.LookRotation(startForward);
    }

    // ─────────────────────────────────────────────
    // Update
    // ─────────────────────────────────────────────

    void Update()
    {
        if (splineContainer == null) return;

        HandleInput();
        MoveOnSpline();
        HandleJump();

        if (anim != null)
        {
            anim.SetBool("IsRunning", true);
        }
    }

    // ─────────────────────────────────────────────
    // Movement
    // ─────────────────────────────────────────────

    void MoveOnSpline()
    {
        // Move forward along spline
        splineProgress +=
            (forwardSpeed / splineLength) * Time.deltaTime;

        // Clamp progress
        splineProgress = Mathf.Clamp01(splineProgress);

        // Current spline position
        Vector3 splinePos =
            splineContainer.EvaluatePosition(splineProgress);

        // Current forward direction
        Vector3 tangent =
            ((Vector3)splineContainer.EvaluateTangent(splineProgress)).normalized;

        // Right vector
        Vector3 right =
            Vector3.Cross(Vector3.up, tangent).normalized;

        // Smooth lane switching
        smoothLaneOffset = Mathf.Lerp(
            smoothLaneOffset,
            LaneTarget(),
            laneSmooth * Time.deltaTime);

        // Apply lane offset
        Vector3 pos =
            splinePos + right * smoothLaneOffset;

        // Apply ground snapping + jump
        pos.y = GroundY(pos) + groundOffset + JumpArc();

        transform.position = pos;

        // Smooth rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            SlopeRotation(tangent),
            10f * Time.deltaTime);

        // End of spline
        if (splineProgress >= 1f)
        {
            Debug.Log("END OF TRACK!");
        }
    }

    // ─────────────────────────────────────────────
    // Input
    // ─────────────────────────────────────────────

    void HandleInput()
    {
        // Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.A))
        {
            targetLane = Mathf.Max(0, targetLane - 1);
        }

        // Move Right
        if (Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.D))
        {
            targetLane = Mathf.Min(2, targetLane + 1);
        }

        // Jump
        if ((Input.GetKeyDown(KeyCode.UpArrow) ||
             Input.GetKeyDown(KeyCode.W))
             && !jumping)
        {
            jumping = true;
            jumpTimer = 0f;

            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }
        }
    }

    // ─────────────────────────────────────────────
    // Jump
    // ─────────────────────────────────────────────

    void HandleJump()
    {
        if (!jumping) return;

        jumpTimer += Time.deltaTime;

        if (jumpTimer >= jumpDuration)
        {
            jumping = false;
            jumpTimer = 0f;
        }
    }

    float JumpArc()
    {
        if (!jumping) return 0f;

        float p = jumpTimer / jumpDuration;

        return 4f * jumpHeight * p * (1f - p);
    }

    // ─────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────

    float LaneTarget()
    {
        // Left = -distance
        // Center = 0
        // Right = +distance

        return (targetLane - 1) * laneDistance;
    }

    float GroundY(Vector3 pos)
    {
        if (Physics.Raycast(
            pos + Vector3.up * 5f,
            Vector3.down,
            out RaycastHit hit,
            20f,
            groundLayer))
        {
            return hit.point.y;
        }

        return pos.y;
    }

    Quaternion SlopeRotation(Vector3 forward)
    {
        Quaternion rot =
            Quaternion.LookRotation(forward);

        if (Physics.Raycast(
            transform.position + Vector3.up * 2f,
            Vector3.down,
            out RaycastHit hit,
            5f,
            groundLayer))
        {
            rot =
                Quaternion.FromToRotation(
                    transform.up,
                    hit.normal) * rot;
        }

        return rot;
    }
}