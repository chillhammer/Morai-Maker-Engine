using Assets.Scripts.Core;
using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CameraScroll : Lockable
    {
        [SerializeField]
        private DialogueMenu dialogueMenu;

        [SerializeField]
        private Rect scrollLeft;
        [SerializeField]
        private Rect scrollRight;
        [SerializeField]
        private float acceleration;
        [SerializeField]
        private float drag;

        private new Camera camera;

        private float speed;
        private float? scrollTarget;

        private float minX;
        private float maxX;

        protected override void Awake()
        {
            base.Awake();

            camera = GetComponent<Camera>();
            if(camera == Camera.main)
                GridManager.Instance.GridSizeChanged += OnGridSizeChanged;

            dialogueMenu.DialogueOpened += () => AddLock(dialogueMenu);
            dialogueMenu.DialogueClosed += () => RemoveLock(dialogueMenu);
        }

        private void FixedUpdate()
        {
            if(IsLocked)
                return;

            if(scrollTarget.HasValue)
            {
                // Scroll the view based on scroll target position
                ScrollImmediate(Mathf.Lerp(transform.position.x, scrollTarget.Value, 0.06f));
            }
            else
            {
                // Calculating new camera speed
                Vector2 mousePosition = new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
                if(scrollLeft.Contains(mousePosition))
                {
                    float ratio = Mathf.InverseLerp(scrollLeft.xMax, scrollLeft.xMin, mousePosition.x);
                    speed -= ratio * ratio * acceleration;
                }
                else if(scrollRight.Contains(mousePosition))
                {
                    float ratio = Mathf.InverseLerp(scrollRight.xMin, scrollRight.xMax, mousePosition.x);
                    speed += ratio * ratio * acceleration;
                }

                speed *= drag;

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

            if(x < minX)
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

        /// <summary>
        /// Sets the horizontal bounds for the camera based on the camera's orthographic size and aspect ratio.
        /// </summary>
        /// <param name="minX">Minimum camera x position</param>
        /// <param name="maxX">Maximum camera x position</param>
        public void SetBounds(float minX, float maxX)
        {
            // Set new bounds
            float offsetX = camera.orthographicSize * camera.aspect;
            this.minX = minX + offsetX;
            this.maxX = maxX - offsetX;

            // Clamp to new bounds
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, this.minX, this.maxX), transform.position.y, transform.position.z);
        }

        private void OnGridSizeChanged(int x, int y)
        {
            speed = 0;
            camera.orthographicSize = (float)y / 2;
            transform.position = new Vector3(0, camera.orthographicSize, transform.position.z);
            SetBounds(0, x);
        }
    }
}