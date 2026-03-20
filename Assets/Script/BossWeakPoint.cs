using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossWeakPoint : MonoBehaviour
{
    [Header("References")]
    public BossController boss;
    public Transform player;

    [Header("Settings")]
    public float fleeSpeed = 8f;
    public ParticleSystem explosionVFX;
    public LayerMask wallLayer; // เลือก Layer ที่เป็นกำแพงใน Inspector

    private Rigidbody2D rb;
    private bool isFleeing = false;
    private bool hasBeenHit = false; // กันบั๊กชนซ้ำ

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        // ป้องกันการทะลุกำแพง (สำคัญมาก)
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void FixedUpdate()
    {
        if (isFleeing && player != null)
        {
            Vector2 fleeDirection = (transform.position - player.position).normalized;

            // เช็คกำแพงข้างหน้าในระยะ 1 หน่วย
            RaycastHit2D hit = Physics2D.Raycast(transform.position, fleeDirection, 1f, wallLayer);

            if (hit.collider != null)
            {
                // ถ้าเจอกำแพง ให้หยุดวิ่ง หรือไถลไปตามกำแพงแทน
                rb.linearVelocity = Vector2.zero;
            }
            else
            {
                rb.linearVelocity = fleeDirection * fleeSpeed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasBeenHit) return; // ถ้ากำลังระเบิดหรือตายอยู่ ไม่ต้องทำอะไร

        if (collision.CompareTag("Player"))
        {
            if (!isFleeing)
            {
                // ชนครั้งแรก -> เข้าเฟส 2
                hasBeenHit = true;
                if (boss != null) boss.OnGemHitFirstTime(this);
            }
            else
            {
                // ชนตอนกำลังหนี -> บอสตาย
                hasBeenHit = true;
                rb.simulated = false; // หยุดฟิสิกส์ทันทีกันกระเด็นมั่ว
                if (boss != null) boss.OnPhase2GemDestroyed();

                // สร้างระเบิดส่งท้ายก่อนหายไป
                ExplodeAtBoom();
                Destroy(gameObject);
            }
        }
    }

    public void ExplodeAndDie()
    {
        ExplodeAtBoom();
        Destroy(gameObject);
    }

    private void ExplodeAtBoom()
    {
        if (explosionVFX != null)
        {
            // หาจุด Boom ที่เป็นลูกของบอส
            Vector3 spawnPos = transform.position;
            if (boss != null)
            {
                Transform boomPoint = boss.transform.Find("Boom");
                if (boomPoint != null) spawnPos = boomPoint.position;
            }

            ParticleSystem vfx = Instantiate(explosionVFX, spawnPos, Quaternion.identity);
            vfx.Play();
            Destroy(vfx.gameObject, 2f);
        }
    }

    public void DetachAndFlee()
    {
        transform.SetParent(null);
        isFleeing = true;
    }
}