using UnityEngine;
namespace LaserHit2D
{
    public class TargetManager : MonoBehaviour
    {
        public static TargetManager Instance { get; private set; }

        private LaserTarget[] m_AllTargets;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

        }

        void Start()
        {
            // ค้นหาเป้าหมายทั้งหมดในฉาก (แบบเร็วกว่าเดิม) แล้วเก็บใส่ตัวแปร m_AllTargets
            m_AllTargets = FindObjectsByType<LaserTarget>(FindObjectsSortMode.None);

            Debug.Log($"Found {m_AllTargets.Length} targets in scene.");
        }

        public void CheckAllTargetsHit()
        {
            // Early return if targets not yet initialized
            if (m_AllTargets == null || m_AllTargets.Length == 0)
            {
                Debug.LogWarning("TargetManager: No targets initialized.");
                return;
            }

            foreach (var target in m_AllTargets)
            {
                if (!target.IsHit)
                    return; // At least one target is not hit
            }

            // All targets are hit
            Debug.Log("All targets are hit!");
            GameControl.Current.HandleWin();
        }
    }
}