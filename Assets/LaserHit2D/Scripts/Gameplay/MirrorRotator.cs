using UnityEngine;
namespace LaserHit2D
{
    public class MirrorRotator : MonoBehaviour
    {
        [SerializeField] private float m_RotationAngle = 45f;
        [SerializeField] private string m_CameraName = "Main Camera";
        private Camera m_GameplayCamera;

        private void Awake()
        {
            m_GameplayCamera = GameObject.Find(m_CameraName)?.GetComponent<Camera>();

            if (m_GameplayCamera == null)
            {
                foreach (Camera cam in Camera.allCameras)
                {
                    if (cam.tag != "MainCamera")
                    {
                        m_GameplayCamera = cam;
                        break;
                    }
                }
            }


        }

        private void Update()
        {
            if (m_GameplayCamera == null) return;

            HandleMouseInput();
            HandleTouchInput();
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0)) // Left click
            {
                Vector2 mousePos = m_GameplayCamera.ScreenToWorldPoint(Input.mousePosition);
                Collider2D hit = Physics2D.OverlapPoint(mousePos);

                if (hit != null && hit.gameObject == gameObject)
                {
                    RotateMirror();
                }
            }
        }

        private void HandleTouchInput()
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Vector2 touchPos = m_GameplayCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
                Collider2D hit = Physics2D.OverlapPoint(touchPos);

                if (hit != null && hit.gameObject == gameObject)
                {
                    RotateMirror();
                }
            }
        }

        private void RotateMirror()
        {
            transform.Rotate(0f, 0f, m_RotationAngle);
        }
    }
}