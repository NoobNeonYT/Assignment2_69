using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MinionAI : MonoBehaviour
{
    [Header("Target & Pursue")]
    public Transform target;
    public Rigidbody2D targetRb;
    public float maxSpeed = 6f;
    public float maxPrediction = 1f;
    public float lifeSpan = 4f;

    [Header("Obstacle Avoidance")]
    public float lookAhead = 1.5f;
    public float rayAngle = 20f;
    public float avoidForce = 12f;
    public LayerMask obstacleLayer;

    [Header("Separation (เดินไม่ซ้อนทับกัน)")]
    public float separationRadius = 1.5f; // รัศมีที่จะเริ่มผลักเพื่อนออก
    public float separationForce = 10f; // แรงผลัก (ยิ่งเยอะยิ่งดีดแรง)

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                targetRb = playerObj.GetComponent<Rigidbody2D>();
            }
        }
        Destroy(gameObject, lifeSpan);
    }

    void FixedUpdate()
    {
        if (target == null || targetRb == null) return;

        // --- 1. ระบบ Pursue ---
        Vector2 direction = (Vector2)target.position - rb.position;
        float distance = direction.magnitude;
        float speed = rb.linearVelocity.magnitude;
        float prediction = (speed <= distance / maxPrediction) ? maxPrediction : distance / speed;

        Vector2 futurePosition = (Vector2)target.position + (targetRb.linearVelocity * prediction);
        Vector2 pursueVelocity = (futurePosition - rb.position).normalized * maxSpeed;

        // --- 2. ระบบ Obstacle Avoidance (หลบกำแพง) ---
        Vector2 forwardDir = rb.linearVelocity.sqrMagnitude > 0.1f ? rb.linearVelocity.normalized : pursueVelocity.normalized;
        Vector2 rightDir = new Vector2(forwardDir.y, -forwardDir.x);
        Vector2 leftDir = new Vector2(-forwardDir.y, forwardDir.x);

        Vector2 leftRayDir = Quaternion.Euler(0, 0, rayAngle) * forwardDir;
        Vector2 rightRayDir = Quaternion.Euler(0, 0, -rayAngle) * forwardDir;

        RaycastHit2D hitLeft = Physics2D.Raycast(rb.position, leftRayDir, lookAhead, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rb.position, rightRayDir, lookAhead, obstacleLayer);

        Vector2 avoidanceForce = Vector2.zero;

        if (hitLeft.collider != null)
        {
            avoidanceForce += rightDir * avoidForce;
        }
        else if (hitRight.collider != null)
        {
            avoidanceForce += leftDir * avoidForce;
        }

        // --- 3. ระบบ Separation (หลบเพื่อนไม่ให้เดินทับกัน) ---
        Vector2 separationVector = Vector2.zero;
        int minionCount = 0;

        // ตรวจหา Collider ทั้งหมดในรัศมี
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(rb.position, separationRadius);
        foreach (Collider2D neighbor in neighbors)
        {
            // เช็คว่าไม่ใช่ตัวเอง และมีสคริปต์ MinionAI (แปลว่าเป็นเพื่อนมินเนี่ยน)
            if (neighbor.gameObject != gameObject && neighbor.GetComponent<MinionAI>() != null)
            {
                Vector2 pushAwayDir = rb.position - (Vector2)neighbor.transform.position;

                // ยิ่งอยู่ใกล้กันมาก แรงผลักยิ่งมหาศาล (แปรผกผันกับระยะทาง)
                float neighborDistance = pushAwayDir.magnitude;
                if (neighborDistance > 0) // กันค่าเป็น 0 แล้ว Error
                {
                    separationVector += (pushAwayDir.normalized / neighborDistance);
                    minionCount++;
                }
            }
        }

        if (minionCount > 0)
        {
            separationVector *= separationForce;
        }

        // --- 4. รวมพลัง (Pursue + Avoidance กำแพง + Separation หลบเพื่อน) ---
        Vector2 finalVelocity = pursueVelocity + avoidanceForce + separationVector;

        // คุมความเร็วไม่ให้ดีดแรงเกินไปเวลาโดนหลายๆ แรงผลักพร้อมกัน
        if (finalVelocity.magnitude > maxSpeed)
        {
            finalVelocity = finalVelocity.normalized * maxSpeed;
        }

        rb.linearVelocity = finalVelocity;

        // --- 5. หันหน้า ---
        if (finalVelocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(finalVelocity.y, finalVelocity.x) * Mathf.Rad2Deg;
            rb.MoveRotation(angle - 90f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) { HandlePlayerCollision(collision.gameObject); }
    private void OnCollisionEnter2D(Collision2D collision) { HandlePlayerCollision(collision.gameObject); }

    private void HandlePlayerCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController2D>();
            if (player != null) player.Die();
            Destroy(gameObject);
        }
    }
}