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
    public float avoidForce = 12f; // ปรับลดลงนิดนึงจะได้เลี้ยวสมูทขึ้น
    public LayerMask obstacleLayer;

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

        // --- 2. ระบบ Obstacle Avoidance (แถลบออกข้าง) ---
        Vector2 forwardDir = rb.linearVelocity.sqrMagnitude > 0.1f ? rb.linearVelocity.normalized : pursueVelocity.normalized;

        // สร้างเวกเตอร์ทิศ "ขวา" และ "ซ้าย" ของตัวละคร เพื่อใช้ผลักออกด้านข้าง
        Vector2 rightDir = new Vector2(forwardDir.y, -forwardDir.x);
        Vector2 leftDir = new Vector2(-forwardDir.y, forwardDir.x);

        Vector2 leftRayDir = Quaternion.Euler(0, 0, rayAngle) * forwardDir;
        Vector2 rightRayDir = Quaternion.Euler(0, 0, -rayAngle) * forwardDir;

        RaycastHit2D hitLeft = Physics2D.Raycast(rb.position, leftRayDir, lookAhead, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(rb.position, rightRayDir, lookAhead, obstacleLayer);

        Vector2 avoidanceForce = Vector2.zero;

        Debug.DrawRay(rb.position, leftRayDir * lookAhead, Color.yellow);
        Debug.DrawRay(rb.position, rightRayDir * lookAhead, Color.yellow);

        // เปลี่ยนจากดันสะท้อนกลับ เป็นดันออกด้านข้างแทน
        if (hitLeft.collider != null)
        {
            // เรดาร์ซ้ายชน -> ผลักตัวหลบไปทางขวา
            avoidanceForce += rightDir * avoidForce;
            Debug.DrawRay(hitLeft.point, rightDir, Color.red);
        }
        else if (hitRight.collider != null)
        {
            // เรดาร์ขวาชน -> ผลักตัวหลบไปทางซ้าย
            avoidanceForce += leftDir * avoidForce;
            Debug.DrawRay(hitRight.point, leftDir, Color.red);
        }

        // --- 3. รวมพลัง ---
        Vector2 finalVelocity = pursueVelocity + avoidanceForce;

        if (finalVelocity.magnitude > maxSpeed)
        {
            finalVelocity = finalVelocity.normalized * maxSpeed;
        }

        rb.linearVelocity = finalVelocity;

        // --- 4. หันหน้า ---
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