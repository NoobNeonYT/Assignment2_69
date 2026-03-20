using UnityEngine;
namespace LaserHit2D
{
    public class LaserShooter : MonoBehaviour
    {
        [SerializeField] private GameObject m_LaserPrefab;
        [SerializeField] private int m_LaserNumber;
        public Color m_LaserColor = Color.white;

        private LaserReflector m_Reflector;

        void Start()
        {
            GameObject laser = Instantiate(m_LaserPrefab, transform.position, Quaternion.identity);
            m_Reflector = laser.GetComponent<LaserReflector>();
            m_Reflector.SetLaserNumber(m_LaserNumber);
            laser.GetComponent<LineRenderer>().startColor = m_LaserColor;
            laser.GetComponent<LineRenderer>().endColor = m_LaserColor;
        }

        void Update()
        {
            if (m_Reflector != null)
            {
                m_Reflector.CastLaser(transform.position + new Vector3(0, 0, 5), transform.right);
            }
        }
    }
}