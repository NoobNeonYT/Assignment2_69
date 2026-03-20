using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 movement;

    // ตัวแปรสำหรับจัดการการกระเด็น
    private float knockbackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector2 inputVec = new Vector2(moveX, moveY);

        if (inputVec.magnitude < 0.2f)
        {
            movement = Vector2.zero;
        }
        else
        {
            movement = Vector2.ClampMagnitude(inputVec, 1f);
        }
    }

    void FixedUpdate()
    {
        // ถ้ากำลังกระเด็นอยู่ ให้ปล่อยให้ตัวลอยไปตามแรงฟิสิกส์
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;
        }
        // ถ้าไม่กระเด็น ก็บังคับเดินตามจอยปกติ
        else
        {
            rb.linearVelocity = movement * moveSpeed;
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกจากตัวบอสตอนที่บอสผลักเรา
    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        knockbackTimer = duration;
        rb.linearVelocity = direction * force;
    }
    // ใส่ไว้ใน PlayerController2D.cs นะครับ
    public void Die()
    {
        Debug.Log("Player ตายแล้ว!");
        // เดี๋ยวค่อยมาเพิ่มโค้ดเรียก UI Game Over หรือโหลดฉากใหม่ตรงนี้

        Destroy(gameObject);
    }
}