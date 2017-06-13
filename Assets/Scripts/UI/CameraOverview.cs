using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class CameraOverview : MonoBehaviour
    {
        [SerializeField]
        private CameraScroll mainCamera;
        [SerializeField]
        private LineRenderer outline;

        private new Camera camera;

        private void Awake()
        {
            camera = GetComponent<Camera>();
            GridManager.Instance.GridSizeChanged += OnGridSizeChanged;
        }

        private void Update()
        {
            // Scroll to overview location
            if(Input.GetMouseButton(0) && camera.rect.Contains(new Vector2(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height)))
            {
                mainCamera.ScrollImmediate(camera.ScreenToWorldPoint(Input.mousePosition).x);
            }
        }

        private void OnGridSizeChanged(int x, int y)
        {
            camera.orthographicSize = x / camera.aspect / 2;
            transform.position = new Vector3(camera.orthographicSize * camera.aspect, (float)y / 2, transform.position.z);

            // Update current window outline
            float windowWidth = Camera.main.aspect * y / 2;
            float windowHeight = (float)y / 2;
            outline.SetPositions(new Vector3[] {
                new Vector3(-windowWidth, -windowHeight),
                new Vector3(windowWidth, -windowHeight),
                new Vector3(windowWidth, windowHeight),
                new Vector3(-windowWidth, windowHeight)});
        }
    }
}