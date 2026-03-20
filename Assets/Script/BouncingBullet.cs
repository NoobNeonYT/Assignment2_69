using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BouncingBullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 10f;
    public int maxBounces = 3;

    private Rigidbody2D rb;
    private int currentBounces = 0;
    private Vector2 currentVelocity; // เอาไว้เก็บทิศทางก่อนชน

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // บังคับยิงไปด้านหน้าของตัวกระสุนเอง
        rb.linearVelocity = transform.up * speed;
    }

    void FixedUpdate()
    {
        // บังคับความเร็วให้คงที่เสมอ (กันกระสุนหนืดหรือหยุดนิ่งเวลาไถลกำแพง)
        currentVelocity = rb.linearVelocity;
        if (currentVelocity.magnitude > 0.1f)
        {
            rb.linearVelocity = currentVelocity.normalized * speed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. ถ้าชนผู้เล่น -> ทำดาเมจและหายไป
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("โดนกระสุนชิ่ง! ผู้เล่นตาย!");
            Destroy(collision.gameObject); // ทำลายผู้เล่น
            Destroy(gameObject); // ทำลายกระสุน
            return;
        }

        // 2. ถ้าชนอย่างอื่น (เช่น กำแพง) -> คำนวณชิ่ง
        currentBounces++;
        if (currentBounces > maxBounces)
        {
            Destroy(gameObject);
        }
        else
        {
            // สูตรคำนวณการสะท้อน (Reflect)
            Vector2 surfaceNormal = collision.GetContact(0).normal;
            Vector2 reflectDirection = Vector2.Reflect(currentVelocity.normalized, surfaceNormal);

            // สั่งให้กระสุนพุ่งไปในทิศทางใหม่
            rb.linearVelocity = reflectDirection * speed;

            // หันหน้ากระสุนให้ตรงกับทิศที่พุ่งไป
            float angle = Mathf.Atan2(reflectDirection.y, reflectDirection.x) * Mathf.Rad2Deg;
            rb.rotation = angle - 0f;
        }
    }
}