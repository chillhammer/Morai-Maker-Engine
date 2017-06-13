using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GridPlacement : Singleton<GridPlacement>
    {
        public SpriteData CurrentSprite;

        [SerializeField]
        private GridObject previewObject;

        private Vector2 previousMousePosition;
        private bool? deletionLayer; // Functional if true, decorative if false

        private void Start()
        {
            CurrentSprite = SpriteManager.Instance.GetSpriteData(SpriteName.Ground);
            previewObject.SetSprite(CurrentSprite);

            previousMousePosition = Input.mousePosition;
            deletionLayer = null;
        }

        private void Update()
        {
            // Calculate sprite coordinates for the current mouse position
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = 0, y = 0;

            // Interpolate between previous and current mouse position
            for(float i = 0.25f; i <= 1; i += 0.25f)
            {
                x = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.x, mousePosition.x, i) - (float)CurrentSprite.Width / 2);
                y = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.y, mousePosition.y, i) - (float)CurrentSprite.Height / 2);

                // Place new grid object
                if(GridManager.Instance.CanAddGridObject(CurrentSprite, x, y))
                {
                    previewObject.gameObject.SetActive(true);

                    if(Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && CurrentSprite.HoldToPlace))
                        GridManager.Instance.AddGridObject(CurrentSprite, x, y);
                }
                else
                {
                    previewObject.gameObject.SetActive(false);
                }

                if(Input.GetMouseButton(1))
                {
                    // Set deletion layer if not set, prioritizing the functional layer
                    if(deletionLayer == null)
                    {
                        if(GridManager.Instance.ContainsGridObject(true, x, y))
                            deletionLayer = true;
                        else if(GridManager.Instance.ContainsGridObject(false, x, y))
                            deletionLayer = false;
                    }

                    // Remove existing grid object based on deletion layer
                    if(deletionLayer != null)
                        if(GridManager.Instance.ContainsGridObject(deletionLayer.Value, x, y))
                            GridManager.Instance.RemoveGridObject(deletionLayer.Value, x, y);
                }
            }

            // Update preview object
            if(CurrentSprite.Name != previewObject.Sprite.Name)
                previewObject.SetSprite(CurrentSprite);
            previewObject.SetPosition(x, y);

            // Remove deletion layer
            if(Input.GetMouseButtonUp(1))
                deletionLayer = null;

            // Store mouse position
            previousMousePosition = mousePosition;
        }
    }
}