using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CameraOverview : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer outline;

        private new Camera camera;
        private CameraScroll windowScroll;
        private float outlineHalfWidth;

        private void Awake()
        {
            windowScroll = Camera.main.GetComponent<CameraScroll>();
            camera = GetComponent<Camera>();
            outlineHalfWidth = outline.widthMultiplier / 2;
            GridManager.Instance.GridSizeChanged += OnGridSizeChanged;
        }

        private void Update()
        {
            // Scroll to overview location
            if(Input.GetMouseButton(0) && camera.rect.Contains(new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height)))
                windowScroll.ScrollImmediate(camera.ScreenToWorldPoint(Input.mousePosition).x);
        }

        private void OnGridSizeChanged(int x, int y)
        {
            camera.orthographicSize = x / camera.aspect / 2;
            transform.position = new Vector3(camera.orthographicSize * camera.aspect, (float)y / 2, transform.position.z);

            // Update current window outline
            float windowWidth = Camera.main.aspect * y / 2;
            float windowHeight = (float)y / 2;
            outline.SetPositions(new Vector3[] {
                new Vector3(-windowWidth + outlineHalfWidth, -windowHeight + outlineHalfWidth),
                new Vector3(windowWidth - outlineHalfWidth, -windowHeight + outlineHalfWidth),
                new Vector3(windowWidth - outlineHalfWidth, windowHeight - outlineHalfWidth),
                new Vector3(-windowWidth + outlineHalfWidth, windowHeight - outlineHalfWidth)});
        }
    }
}