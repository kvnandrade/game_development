using UnityEngine;

namespace ValeDosCristais
{
    public sealed class CameraFollow : MonoBehaviour
    {
        public Transform Target { get; set; }
        public Vector2 LevelBounds { get; set; }

        private Camera attachedCamera;

        private void Awake()
        {
            attachedCamera = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            if (Target == null || attachedCamera == null)
            {
                return;
            }

            float verticalSize = attachedCamera.orthographicSize;
            float horizontalSize = verticalSize * attachedCamera.aspect;
            float targetX = Mathf.Clamp(Target.position.x, horizontalSize, Mathf.Max(horizontalSize, LevelBounds.x - horizontalSize));
            float targetY = Mathf.Clamp(Target.position.y + 1.1f, verticalSize, Mathf.Max(verticalSize, LevelBounds.y - verticalSize));

            transform.position = new Vector3(targetX, targetY, -10f);
        }
    }
}
