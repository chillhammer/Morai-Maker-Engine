using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GridManager : Singleton<GridManager>
    {
        public event Action<int, int> GridSizeChanged;

        [SerializeField]
        private GridObject gridObjectPrefab;
        [SerializeField]
        private Transform gridObjectParent;

        private int gridWidth;
        private int gridHeight;

        private GridObject[,] gridFunctional;
        private GridObject[,] gridDecorative;

        // TODO Temporary
        protected void Start()
        {
            SetGridSize(200, 14);
        }

        // TODO Temporary
        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
                AddGridObject(SpriteManager.GetSpriteData(SpriteName.Tree), 
                    Mathf.RoundToInt(Input.mousePosition.x / Screen.width * Camera.main.orthographicSize * Camera.main.aspect * 2 + CameraController.Instance.OffsetX), 
                    Mathf.RoundToInt(Input.mousePosition.y / Screen.height * Camera.main.orthographicSize * 2));
        }

        public void SetGridSize(int x, int y)
        {
            gridWidth = x;
            gridHeight = y;

            gridFunctional = new GridObject[x, y];
            gridDecorative = new GridObject[x, y];

            GridSizeChanged(x, y);
        }

        public bool CanAddGridObject(SpriteData sprite, int x, int y)
        {
            if(x < 0 || x + sprite.Width > gridWidth)
                return false;
            if(y < 0 || y + sprite.Height > gridHeight)
                return false;

            for(int i = x; i < x + sprite.Width; i++)
            {
                for(int j = y; j < y + sprite.Height; j++)
                {
                    Debug.Log(i + " " + j + ": " + gridDecorative[i, j]);
                    if(sprite.Functional && gridFunctional[i, j] != null)
                        return false;
                    else if(!sprite.Functional && gridDecorative[i, j] != null)
                        return false;
                }
            }

            return true;
        }

        public void AddGridObject(SpriteData sprite, int x, int y)
        {
            if(!CanAddGridObject(sprite, x, y))
                return;

            // Instantiate object
            GridObject clone = Instantiate(gridObjectPrefab, gridObjectParent);
            clone.Initialize(sprite, x, y);

            // Add references to object in grid
            for(int i = x; i < x + sprite.Width; i++)
            {
                for(int j = y; j < y + sprite.Height; j++)
                {
                    if(sprite.Functional)
                        gridFunctional[i, j] = clone;
                    else
                        gridDecorative[i, j] = clone;
                }
            }
        }
    }
}