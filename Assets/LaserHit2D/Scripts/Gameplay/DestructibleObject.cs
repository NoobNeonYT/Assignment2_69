using UnityEngine;
namespace LaserHit2D
{
    public class DestructibleObject : MonoBehaviour
    {
        [SerializeField] private int m_Health = 10;
        [SerializeField] private GameObject m_DestroyEffect;

        public void ApplyLaserDamage(float damageAmount)
        {
            int damage = Mathf.CeilToInt(damageAmount);
            if (m_Health <= 0) return;

            m_Health -= damage;
            if (m_Health <= 0)
            {
                if (m_DestroyEffect != null)
                    Instantiate(m_DestroyEffect, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }
        }

    }
}