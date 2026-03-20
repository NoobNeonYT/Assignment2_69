using UnityEngine;
using System.Collections;

public class LaserGun : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float lifeTime = 5f;

    [Header("Blink Settings (ระบบเปิด-ปิด)")]
    public GameObject laserBeam; // ลากตัวลูก (Beam) มาใส่ช่องนี้
    public float onDuration = 0.5f; // ระยะเวลาที่เลเซอร์ยิง (เปิด)
    public float offDuration = 0.5f; // ระยะเวลาที่เลเซอร์ดับ (ปิด)

    void Start()
    {
        // สั่งทำลายตัวเองเมื่อหมดอายุขัย
        Destroy(gameObject, lifeTime);

        // เริ่มระบบยิงแบบเปิด-ปิด
        if (laserBeam != null)
        {
            StartCoroutine(BlinkLaser());
        }
        else
        {
            Debug.LogWarning("อย่าลืมลากลูก Beam มาใส่ในช่อง Laser Beam นะครับ!");
        }
    }

    void Update()
    {
        // ปืนเลื่อนลงมาเรื่อยๆ
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
    }

    IEnumerator BlinkLaser()
    {
        while (true) // ลูปสลับเปิด-ปิดไปเรื่อยๆ จนกว่าปืนจะถูกทำลาย
        {
            // 1. เปิดเลเซอร์ (สร้างดาเมจ + เห็นภาพ)
            laserBeam.SetActive(true);
            yield return new WaitForSeconds(onDuration);

            // 2. ปิดเลเซอร์ (เดินทะลุได้ + ภาพหาย)
            laserBeam.SetActive(false);
            yield return new WaitForSeconds(offDuration);
        }
    }
}