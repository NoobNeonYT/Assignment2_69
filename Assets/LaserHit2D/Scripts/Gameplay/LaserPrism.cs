using UnityEngine;
namespace LaserHit2D
{
    public class LaserPrism : MonoBehaviour
    {
        [SerializeField] private int m_BranchCount = 3;
        [SerializeField] private float m_SpreadAngle = 45f;
        [SerializeField] private float m_ExitOffset = 20f;

        public void RefractLaser(Vector2 hitPoint, Vector2 incomingDirection, int laserNumber, int depth, LaserReflector reflector)
        {
            Debug.Log("Prism hit — branching laser");

            Vector2 exitPoint = hitPoint + incomingDirection.normalized * m_ExitOffset;

            float baseAngle = 0f; // Centered around Vector2.up
            float angleStep = m_SpreadAngle / (m_BranchCount - 1);

            for (int i = 0; i < m_BranchCount; i++)
            {
                float angleOffset = -m_SpreadAngle / 2f + i * angleStep;
                float finalAngle = baseAngle + angleOffset;
                Vector2 newDirection = Quaternion.Euler(0, 0, finalAngle) * Vector2.up;

                Vector2 spawnPoint = exitPoint + newDirection * 0.05f;

                reflector.CastLaserRecursive(spawnPoint, newDirection, reflector.BranchPoints, depth);
            }
        }
    }
}
