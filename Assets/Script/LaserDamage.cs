using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ถ้าสิ่งที่มาโดนเลเซอร์คือ Player
        if (collision.CompareTag("Player"))
        {
            Debug.Log("โดนเลเซอร์! ผู้เล่นตาย!");

            // ตรงนี้ถ้าคุณมีฟังก์ชันตาย (เช่น PlayerMovement.Die() เหมือนที่เคยทำในลูกน้อง) 
            // ก็เอามาเรียกตรงนี้ได้เลยครับ ชั่วคราวผมใส่ Destroy ให้ตัวผู้เล่นหายไปก่อน
            Destroy(collision.gameObject);
        }
    }
}