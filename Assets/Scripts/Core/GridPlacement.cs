using Assets.Scripts.Core;
using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPlacement : Singleton<GridPlacement>
{
    public SpriteData CurrentSprite;
    
    [SerializeField]
    private GridObject gridObjectPrefab;
    [SerializeField]
    private Transform gridObjectParent;
    [SerializeField]
    private CameraController mainCamera;

    private GridObject previewObject;

    private void Start()
    {
        CurrentSprite = SpriteManager.GetSpriteData(SpriteName.Tree);

        // Create preview object
        previewObject = Instantiate(gridObjectPrefab, gridObjectParent);
        previewObject.SetSprite(CurrentSprite);
        previewObject.SetAlpha(0.5f);
        previewObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Calcuulate sprite coordinates for the current mouse position
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int x = Mathf.RoundToInt(mousePosition.x - (float)CurrentSprite.Width / 2);
        int y = Mathf.RoundToInt(mousePosition.y - (float)CurrentSprite.Height / 2);

        // Update preview object
        if(CurrentSprite.Name != previewObject.Sprite.Name)
            previewObject.SetSprite(CurrentSprite);
        previewObject.SetPosition(x, y);

        if(GridManager.Instance.CanAddGridObject(CurrentSprite, x, y))
        {
            previewObject.gameObject.SetActive(true);

            // Place new grid object
            if(Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && CurrentSprite.HoldToPlace))
                GridManager.Instance.AddGridObject(CurrentSprite, x, y);
        }
        else
        {
            previewObject.gameObject.SetActive(false);
        }
    }
}