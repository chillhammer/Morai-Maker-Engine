using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GridManager : Singleton<GridManager>
    {
        public int GridWidth { get; private set; }
        public int GridHeight { get; private set; }

        public event Action<int, int> GridSizeChanged;

        [SerializeField]
        private GridObject gridObjectPrefab;
        [SerializeField]
        private Transform gridObjectParent;

        private GridObject[,] gridFunctional;
        private GridObject[,] gridDecorative;

        // TODO Temporary
        protected void Start()
        {
            SetGridSize(100, 20);
        }

        public void SetGridSize(int x, int y)
        {
            GridWidth = x;
            GridHeight = y;

            gridFunctional = new GridObject[x, y];
            gridDecorative = new GridObject[x, y];

            GridSizeChanged(x, y);
        }

        public bool CanAddGridObject(SpriteData sprite, int x, int y)
        {
            if(x < 0 || x + sprite.Width > GridWidth)
                return false;
            else if(y < 0 || y + sprite.Height > GridHeight)
                return false;

            for(int i = x; i < x + sprite.Width; i++)
            {
                for(int j = y; j < y + sprite.Height; j++)
                {
                    if((sprite.Functional ? gridFunctional[i, j] : gridDecorative[i, j]) != null)
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
            clone.SetSprite(sprite);
            clone.SetPosition(x, y);

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

        public void RemoveGridObject(bool functional, int x, int y)
        {
            if(!ContainsGridObject(functional, x, y))
                return;

            // Automatically removes all references in grid
            Destroy((functional ? gridFunctional[x, y] : gridDecorative[x, y]).gameObject);
        }

        public bool ContainsGridObject(bool functional, int x, int y)
        {
            if(x < 0 || x >= GridWidth)
                return false;
            else if(y < 0 || y >= GridHeight)
                return false;

            return (functional ? gridFunctional[x, y] : gridDecorative[x, y]) != null;
        }
    }
}