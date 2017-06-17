using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using System.Text;
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
        private Transform gridObjectParentFunctional;
        [SerializeField]
        private Transform gridObjectParentDecorative;

        [SerializeField]
        private Vector2 initialGridSize;

        private GridObject[,] gridFunctional;
        private GridObject[,] gridDecorative;

        private List<GridObject> gridObjects;
        
        private void Start()
        {
            gridObjects = new List<GridObject>();
            SetGridSize(Mathf.RoundToInt(initialGridSize.x), Mathf.RoundToInt(initialGridSize.y));
        }

        public void SetGridSize(int x, int y)
        {
            ClearGrid();

            GridWidth = x;
            GridHeight = y;

            gridFunctional = new GridObject[x, y];
            gridDecorative = new GridObject[x, y];

            GridSizeChanged(x, y);
        }

        public void ClearGrid()
        {
            foreach(GridObject gridObject in gridObjects)
                Destroy(gridObject.gameObject);
            gridObjects.Clear();
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
            GridObject clone = Instantiate(gridObjectPrefab, sprite.Functional ? gridObjectParentFunctional : gridObjectParentDecorative);
            clone.SetSprite(sprite);
            clone.SetPosition(x, y);
            gridObjects.Add(clone);

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

            // Remove the grid object
            GridObject gridObject = (functional ? gridFunctional[x, y] : gridDecorative[x, y]);
            gridObjects.Remove(gridObject);
            // - Automatically removes all references in grid
            Destroy(gridObject.gameObject);
        }

        public bool ContainsGridObject(bool functional, int x, int y)
        {
            if(x < 0 || x >= GridWidth)
                return false;
            else if(y < 0 || y >= GridHeight)
                return false;

            return (functional ? gridFunctional[x, y] : gridDecorative[x, y]) != null;
        }

        public string FormatToCSV()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(GridWidth + "," + GridHeight);
            foreach(GridObject gridObject in gridObjects)
                builder.AppendLine(gridObject.Data.Name + "," + gridObject.X + "," + gridObject.Y);
            return builder.ToString();
        }
    }
}