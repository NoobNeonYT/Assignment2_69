
using UnityEngine;
namespace LaserHit2D
{
    public class LaserTarget : MonoBehaviour
    {
        [SerializeField] private int m_TargetNumber;

        private bool m_IsHit = false;

        public int TargetNumber => m_TargetNumber;
        public bool IsHit => m_IsHit;

        public void Hit(int laserNumber)
        {
            if (laserNumber == m_TargetNumber)
            {
                if (!m_IsHit)
                {
                    m_IsHit = true;
                    Debug.Log($"Target {m_TargetNumber} is hit correctly!");
                    TargetManager.Instance.CheckAllTargetsHit();
                }
            }
            else
            {
                GameControl.Current.HandleLose();
                Debug.Log($"Wrong laser hit target {m_TargetNumber}");
            }
        }
    }
}