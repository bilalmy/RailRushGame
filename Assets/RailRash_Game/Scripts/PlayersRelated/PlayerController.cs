using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

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

    [Header("Ground Fix")]

    //groundLayer → which objects count as ground
    //groundOffset → how high player sits above ground
    public LayerMask groundLayer;
    public float groundOffset = 1.2f;

    // ── Private state ──────────────────────────────────────────────────────────
    private List<Transform> points;
    private int segIndex = 0;
    private float segT = 0f;

    private int targetLane = 1;            // 0 = left, 1 = centre, 2 = right
    private float smoothLaneOffset = 0f;

    private bool jumping = false;
    private float jumpTimer = 0f;

    private Animator anim;

    // ── Helpers ────────────────────────────────────────────────────────────────
    private bool PathReady => points != null && points.Count >= 2;
    private bool AtEnd => segIndex >= points.Count - 1;
    private float LaneTarget => (targetLane - 1) * laneDistance;   // –d, 0, +d

    // ── Unity lifecycle ────────────────────────────────────────────────────────
    void Start()
    {
        anim = GetComponent<Animator>();

        if (pathManager == null) { Debug.LogError("Assign PathManager"); return; }

        points = pathManager.pathPoints;

        if (!PathReady) { Debug.LogError("Need at least 2 path points"); return; }

        transform.position = points[0].position;
        transform.rotation = Quaternion.LookRotation((points[1].position - points[0].position).normalized);
    }

    void Update()
    {
        if (!PathReady) return;

        HandleInput();
        MoveOnPath();
        HandleJump();

        anim?.SetBool("IsRunning", true);
    }

    // ── Movement ───────────────────────────────────────────────────────────────
    void MoveOnPath()
    {
        if (AtEnd) return;
        //Move forward along path   ( decide next point avaiable ... ) 
        AdvanceSegment();

        Vector3 start = points[segIndex].position;
        Vector3 end = points[segIndex + 1].position;

        //Direction of movement
        Vector3 forward = (end - start).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        // Smooth transition between lanes
        smoothLaneOffset = Mathf.Lerp(smoothLaneOffset, LaneTarget, laneSmooth * Time.deltaTime);

        // Base position on path + lane
        Vector3 pos = Vector3.Lerp(start, end, segT) + right * smoothLaneOffset;

        // Snap to ground
        pos.y = GroundY(pos) + groundOffset + JumpArc();

        transform.position = pos;
        transform.rotation = Quaternion.Slerp(transform.rotation, SlopeRotation(forward), 10f * Time.deltaTime);
    }

    // Advances segT / segIndex each frame
    void AdvanceSegment()
    {
        float length = Vector3.Distance(points[segIndex].position, points[segIndex + 1].position);

        segT += (forwardSpeed / length) * Time.deltaTime;

        if (segT >= 1f)
        {
            segT = 0f;
            segIndex++;
            if (AtEnd) Debug.Log("END OF TRACK!");
        }
    }

    // ── Input ──────────────────────────────────────────────────────────────────
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            targetLane = Mathf.Max(0, targetLane - 1);

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            targetLane = Mathf.Min(2, targetLane + 1);

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !jumping)
        {
            jumping = true;
            jumpTimer = 0f;
            anim?.SetTrigger("Jump");
        }
    }

    // ── Jump ───────────────────────────────────────────────────────────────────
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
        return 4f * jumpHeight * p * (1f - p);   // parabola: 0 → peak → 0
    }

    // ── Ground & rotation helpers ──────────────────────────────────────────────
    float GroundY(Vector3 pos)
    {
        return Physics.Raycast(pos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 20f, groundLayer)
            ? hit.point.y
            : pos.y;
    }

    Quaternion SlopeRotation(Vector3 forward)
    {
        Quaternion rot = Quaternion.LookRotation(forward);
        if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f, groundLayer))
            rot = Quaternion.FromToRotation(transform.up, hit.normal) * rot;
        return rot;
    }
}