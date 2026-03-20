using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Boss Settings")]
    public int currentPhase = 1;
    public Transform player;
    public float pushBackDistance = 12f;

    [Header("Gems System")]
    public BossWeakPoint[] gems;

    [Header("Phase 1: Minion Settings")]
    public GameObject minionPrefab;
    public float minionLifespan = 5f;

    [Header("Phase 2: Laser Settings")]
    public GameObject laserPrefab;
    public float laserGap = 6f;
    public float laserSpawnY = 8f;

    [Header("SFX References")]
    public AudioSource bossAudioSource;
    public AudioClip phaseChangeSFX;
    public AudioClip bossDefeatedSFX;
    public AudioClip laserWarningSFX;

    [Header("Phase 2: Bouncing Bullet")]
    public GameObject bouncingBulletPrefab;
    public int bulletsPerAttack = 5; // ยิงออกมากี่นัดต่อ 1 ชุด


    void Start()
    {
        // เริ่มเกมมา โจมตีด้วยลูปเฟส 1
        StartCoroutine(Phase1AttackLoop());
    }

    // ---------------- ลูปโจมตี เฟส 1 ----------------
    IEnumerator Phase1AttackLoop()
    {
        while (currentPhase == 1)
        {
            yield return StartCoroutine(Attack_Minions());
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator Attack_Minions()
    {
        if (minionPrefab == null) yield break;
        for (int wave = 0; wave < 3; wave++)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 3f;
                GameObject minion = Instantiate(minionPrefab, spawnPos, Quaternion.identity);
                // ถ้ามีสคริปต์ MinionAI อย่าลืมตั้ง LifeSpan ผ่านโค้ดตรงนี้ (ถ้าต้องการ)
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // ---------------- ลูปโจมตี เฟส 2 ----------------
    IEnumerator Phase2AttackLoop()
    {
        yield return new WaitForSeconds(2f);

        while (currentPhase == 2)
        {
            // สุ่ม 2 หรือ 3
            int attackType = Random.Range(2, 4);

            switch (attackType)
            {
                case 2:
                    yield return StartCoroutine(Attack_LaserSweep());
                    break;
                case 3:
                    yield return StartCoroutine(Attack_BouncingBullets());
                    break;
            }

            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator Attack_LaserSweep()
    {
        Debug.Log("Attack 2: เลเซอร์เฟส 2 เริ่มทำงาน! (ยิง 7 ชุด)");

        for (int i = 0; i < 7; i++)
        {
            if (bossAudioSource && laserWarningSFX)
                bossAudioSource.PlayOneShot(laserWarningSFX);

            float randomX = Random.Range(-6f, 6f);

            // เปลี่ยนจาก 12f ที่โดนล็อคไว้ มาใช้ตัวแปรที่เราเพิ่งสร้างครับ
            float spawnY = transform.position.y + laserSpawnY;

            Vector3 leftPos = new Vector3(randomX - (laserGap / 2) - 10f, spawnY, 0);
            Vector3 rightPos = new Vector3(randomX + (laserGap / 2) + 10f, spawnY, 0);

            if (laserPrefab != null)
            {
                Instantiate(laserPrefab, leftPos, Quaternion.identity);
                Instantiate(laserPrefab, rightPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(1.2f);
        }
    }

    // ---------------- ระบบเปลี่ยนเฟส & จบเกม ----------------
    public void OnGemHitFirstTime(BossWeakPoint hitGem)
    {
        if (currentPhase == 1)
        {
            Debug.Log("Hit Phase 1: บอสเข้าเฟส 2!");
            currentPhase = 2;

            // หยุดการเสกลูกน้องในเฟส 1 ทั้งหมดทันที
            StopAllCoroutines();

            if (bossAudioSource && phaseChangeSFX) bossAudioSource.PlayOneShot(phaseChangeSFX);
            PushPlayerBack();
            ChangeToPhase2Appearance();

            // เริ่มโจมตีด้วยลูปของเฟส 2
            StartCoroutine(Phase2AttackLoop());

            // สั่ง Gem ทำงาน
            foreach (BossWeakPoint gem in gems)
            {
                if (gem != null)
                {
                    if (gem == hitGem) gem.ExplodeAndDie();
                    else gem.DetachAndFlee();
                }
            }
        }
    }

    public void OnPhase2GemDestroyed()
    {
        currentPhase = 3; // เซ็ตให้ทะลุเฟส 2 ลูปจะได้หยุด
        StopAllCoroutines();

        Debug.Log("Hit Phase 2: บอสตาย!");
        if (bossAudioSource && bossDefeatedSFX) bossAudioSource.PlayOneShot(bossDefeatedSFX);
        Destroy(gameObject);
    }

    void PushPlayerBack()
    {
        if (player != null)
        {
            Vector2 pushDirection = (player.position - transform.position).normalized;

            // เปลี่ยนตรงนี้กลับเป็น PlayerController2D ครับ
            PlayerController2D playerScript = player.GetComponent<PlayerController2D>();

            if (playerScript != null)
                playerScript.ApplyKnockback(pushDirection, pushBackDistance * 2f, 0.5f);
        }
    }

    void ChangeToPhase2Appearance()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.red;
    }
    IEnumerator Attack_BouncingBullets()
    {
        Debug.Log("Attack 3: ยิงกระสุนชิ่งกำแพง!");

        for (int i = 0; i < bulletsPerAttack; i++)
        {
            // เล็งไปหาผู้เล่น
            Vector2 targetDir;
            if (player != null)
            {
                // ใส่ความคลาดเคลื่อนนิดหน่อย (Random -15 ถึง 15 องศา) จะได้ไม่เล็งเป้าตรงเกินไป
                Vector2 baseDir = (player.position - transform.position).normalized;
                float spread = Random.Range(-15f, 15f);
                targetDir = Quaternion.Euler(0, 0, spread) * baseDir;
            }
            else
            {
                targetDir = Random.insideUnitCircle.normalized;
            }

            // คำนวณองศาหมุนกระสุนก่อนเสก
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            Quaternion spawnRotation = Quaternion.Euler(0, 0, angle - 90f);

            // เสกกระสุน
            if (bouncingBulletPrefab != null)
            {
                Instantiate(bouncingBulletPrefab, transform.position, spawnRotation);
            }

            // เว้นจังหวะยิงทีละนัด
            yield return new WaitForSeconds(0.4f);
        }
    }
}