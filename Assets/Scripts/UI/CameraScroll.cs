using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CameraScroll : MonoBehaviour
    {
        public bool MouseScroll;
        public float OffsetX { get { return transform.position.x - minX; } }

        private new Camera camera;

        private float speed;
        private float? scrollTarget;

        private float minX;
        private float maxX;

        private readonly float ACCELERATION = 0.3f;
        private readonly float DRAG = 0.9f;

        private void Awake()
        {
            camera = GetComponent<Camera>();

            GridManager.Instance.GridSizeChanged += OnGridSizeChanged;
        }

        private void FixedUpdate()
        {
            if(scrollTarget.HasValue)
            {
                // Scroll the view based on scroll target position
                ScrollImmediate(Mathf.Lerp(transform.position.x, scrollTarget.Value, 0.1f));
            }
            else if(MouseScroll)
            {
                // Calculating new camera speed
                float border = Screen.width / 6;
                if(Input.mousePosition.x > Screen.width - border)
                {
                    float ratio = Mathf.InverseLerp(Screen.width - border, Screen.width, Input.mousePosition.x);
                    speed += ratio * ratio * ACCELERATION;
                }
                else if(Input.mousePosition.x < border)
                {
                    float ratio = Mathf.InverseLerp(border, 0, Input.mousePosition.x);
                    speed -= ratio * ratio * ACCELERATION;
                }

                speed *= DRAG;

                // Scroll the view based on camera speed
                ScrollImmediate(transform.position.x + speed);
            }
        }

        /// <summary>
        /// Scrolls the camera instantly and clamps it within the map bounds.
        /// </summary>
        /// <param name="x">Position to scroll the camera to</param>
        public void ScrollImmediate(float x)
        {
            if(x > maxX)
            {
                x = maxX;
                speed = 0;
            }
            else if(x < minX)
            {
                x = minX;
                speed = 0;
            }

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Scrolls the camera to a target position over time, blocking manual scrolling as it does so.
        /// </summary>
        /// <param name="x">Position to scroll the camera to</param>
        public void ScrollOverTime(float x)
        {
            speed = 0;
            scrollTarget = x;
        }

        /// <summary>
        /// Resets the camera's scroll target position and enables manual scrolling again.
        /// </summary>
        public void StopScrolling()
        {
            scrollTarget = null;
        }

        private void OnGridSizeChanged(int x, int y)
        {
            camera.orthographicSize = (float)y / 2;

            minX = camera.orthographicSize * camera.aspect;
            maxX = x - minX;

            transform.position = new Vector3(minX, camera.orthographicSize, transform.position.z);
        }
    }
}