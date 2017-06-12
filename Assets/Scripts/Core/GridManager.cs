using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GridManager : Singleton<GridManager>
    {
        private TileObject[,] gridFunctional;
        private TileObject[,] gridDecorative;

        protected override void Awake()
        {
            base.Awake();
            SetGridSize(200, 14);
        }

        public void SetGridSize(int x, int y)
        {
            gridFunctional = new TileObject[x, y];
            gridDecorative = new TileObject[x, y];

            // Set main camera positioning
            Camera.main.orthographicSize = (float)y / 2;

            // Set minimap camera positioning
        }

        public void AddTileObject(TileObject tileObject, int x, int y)
        {
            // TODO
        }
    }
}