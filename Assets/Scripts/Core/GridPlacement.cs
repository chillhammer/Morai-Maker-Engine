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
            int spriteX = 0, spriteY = 0;

            // Interpolate between previous and current mouse position
            for(float i = 0.25f; i <= 1; i += 0.25f)
            {
                spriteX = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.x, mousePosition.x, i) - (float)CurrentSprite.Width / 2);
                spriteY = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.y, mousePosition.y, i) - (float)CurrentSprite.Height / 2);

                // Place new grid object
                if(GridManager.Instance.CanAddGridObject(CurrentSprite, spriteX, spriteY))
                {
                    previewObject.gameObject.SetActive(true);

                    if((Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && CurrentSprite.HoldToPlace)) && !Input.GetMouseButton(1))
                        GridManager.Instance.AddGridObject(CurrentSprite, spriteX, spriteY);
                }
                else
                {
                    previewObject.gameObject.SetActive(false);
                }

                if(Input.GetMouseButton(1))
                {
                    int mouseX = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.x, mousePosition.x, i) - 0.5f);
                    int mouseY = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.y, mousePosition.y, i) - 0.5f);

                    // Set deletion layer if not set, prioritizing the functional layer
                    if(deletionLayer == null)
                    {
                        if(GridManager.Instance.ContainsGridObject(true, mouseX, mouseY))
                            deletionLayer = true;
                        else if(GridManager.Instance.ContainsGridObject(false, mouseX, mouseY))
                            deletionLayer = false;
                    }

                    // Remove existing grid object based on deletion layer
                    if(deletionLayer != null)
                        if(GridManager.Instance.ContainsGridObject(deletionLayer.Value, mouseX, mouseY))
                            GridManager.Instance.RemoveGridObject(deletionLayer.Value, mouseX, mouseY);
                }
            }

            // Update preview object
            if(CurrentSprite.Name != previewObject.Sprite.Name)
                previewObject.SetSprite(CurrentSprite);
            previewObject.SetPosition(spriteX, spriteY);

            // Remove deletion layer
            if(Input.GetMouseButtonUp(1))
                deletionLayer = null;

            // Store mouse position
            previousMousePosition = mousePosition;
        }
    }
}