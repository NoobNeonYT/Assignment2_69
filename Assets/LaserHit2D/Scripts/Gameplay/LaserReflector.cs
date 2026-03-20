using System.Collections.Generic;
using UnityEngine;
namespace LaserHit2D
{
    public class LaserReflector : MonoBehaviour
    {
        public List<Vector3> BranchPoints => m_BranchPoints;

        [SerializeField] private LineRenderer m_LineRenderer;
        [SerializeField] private float m_MaxDistance = 100f;
        [SerializeField] private float m_DamagePerSecond = 5f;
        private const int MAX_DEPTH = 5;

        private HashSet<DestructibleObject> m_CurrentHits = new HashSet<DestructibleObject>();
        private List<Vector3> m_BranchPoints = new List<Vector3>();

        private int m_LaserNumber;


        public void SetLaserNumber(int number)
        {
            m_LaserNumber = number;
        }

        public void CastLaser(Vector2 origin, Vector2 direction)
        {
            m_CurrentHits.Clear();
            m_BranchPoints.Clear();

            List<Vector3> points = new List<Vector3>();
            CastLaserRecursive(origin, direction, points, 0);

            points.AddRange(m_BranchPoints);

            m_LineRenderer.positionCount = points.Count;
            m_LineRenderer.SetPositions(points.ToArray());

            m_LineRenderer.startWidth = 5;
            m_LineRenderer.endWidth = 5;

            float damageThisFrame = m_DamagePerSecond * Time.deltaTime;
            foreach (var destructible in m_CurrentHits)
            {
                destructible.ApplyLaserDamage(damageThisFrame);
            }
        }

        public void CastLaserRecursive(Vector2 origin, Vector2 direction, List<Vector3> points, int depth)
        {
            if (depth > MAX_DEPTH) return;

            points.Add(origin);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, m_MaxDistance);

            if (hit.collider != null)
            {
                Vector2 hitPoint = hit.point;
                points.Add(hitPoint);

                if (hit.collider.CompareTag("Mirror"))
                {
                    Vector2 reflectedDir = Vector2.Reflect(direction, hit.normal);
                    CastLaserRecursive(hitPoint + reflectedDir * 0.01f, reflectedDir, points, depth + 1);
                }
                else if (hit.collider.CompareTag("LaserTarget"))
                {
                    HandleLaserTarget(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Prism"))
                {
                    points.Add(hitPoint);
                    if (hit.collider.TryGetComponent<LaserPrism>(out var prism))
                    {
                        prism.RefractLaser(hitPoint, direction, m_LaserNumber, depth + 1, this);
                    }
                    return;
                }
                else if (hit.collider.TryGetComponent<DestructibleObject>(out var destructible))
                {
                    m_CurrentHits.Add(destructible);
                }
            }
            else
            {
                points.Add(origin + direction * m_MaxDistance);
            }
        }

        private void HandleLaserTarget(GameObject targetObject)
        {
            if (targetObject.TryGetComponent<LaserTarget>(out var target))
            {
                target.Hit(m_LaserNumber);
            }
        }
    }
}