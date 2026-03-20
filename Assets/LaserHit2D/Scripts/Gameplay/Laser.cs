using UnityEngine;
namespace LaserHit2D
{
    public class Laser : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_Renderer;
        [SerializeField] private int m_LaserNumber;

        public void Initialize(int number)
        {
            m_LaserNumber = number;
        }

        public int GetLaserNumber()
        {
            return m_LaserNumber;
        }
    }
}