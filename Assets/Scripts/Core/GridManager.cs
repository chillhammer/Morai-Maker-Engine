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

        public string FormatToJava()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("package dk.itu.mario.level;");
            builder.AppendLine("");
            builder.AppendLine("import java.util.Random;");
            builder.AppendLine("");
            builder.AppendLine("import dk.itu.mario.MarioInterface.Constraints;");
            builder.AppendLine("import dk.itu.mario.MarioInterface.GamePlay;");
            builder.AppendLine("import dk.itu.mario.MarioInterface.LevelInterface;");
            builder.AppendLine("import dk.itu.mario.engine.sprites.SpriteTemplate;");
            builder.AppendLine("import dk.itu.mario.engine.sprites.Enemy;");
            builder.AppendLine("");
            builder.AppendLine("public class MyLevel extends Level");
            builder.AppendLine("{");
            builder.AppendLine("    private static long lastSeed;");
            builder.AppendLine("    private static Random levelSeedRandom;");
            builder.AppendLine("");
            builder.AppendLine("    private int difficulty;");
            builder.AppendLine("    private int type;");
            builder.AppendLine("");
            builder.AppendLine("    public MyLevel(int width, int height)");
            builder.AppendLine("    {");
            builder.AppendLine("        super(width, height);");
            builder.AppendLine("    }");
            builder.AppendLine("");
            builder.AppendLine("    public MyLevel(int width, int height, long seed, int difficulty, int type, GamePlay playerMetrics)");
            builder.AppendLine("    {");
            builder.AppendLine("        this(width, height);");
            builder.AppendLine("        this.difficulty = difficulty;");
            builder.AppendLine("        this.type = type;");
            builder.AppendLine("        lastSeed = seed;");
            builder.AppendLine("        levelSeedRandom = new Random(seed);");
            builder.AppendLine("");

            // Add objects and calculate exit location
            int[] exit = new int[] { 0, 0 };
            foreach(GridObject gridObject in gridObjects)
            {
                string mapping = null;
                bool enemy = false;

                int width = gridObject.Data.Width;
                int height = gridObject.Data.Height;

                switch(gridObject.Data.Mapping)
                {
                    case SimulatorObject.Ground:
                        mapping = "GROUND";
                        if(gridObject.X > exit[0] || (gridObject.X == exit[0] && gridObject.Y > exit[1]))
                            exit = new int[] { gridObject.X + gridObject.Data.Width - 2, gridObject.Y + gridObject.Data.Height };
                        break;
                    case SimulatorObject.Block:
                        mapping = "BLOCK_EMPTY";
                        if(gridObject.X > exit[0] || (gridObject.X == exit[0] && gridObject.Y > exit[1]))
                            exit = new int[] { gridObject.X + gridObject.Data.Width - 2, gridObject.Y + gridObject.Data.Height };
                        break;
                    case SimulatorObject.Coin:
                        mapping = "COIN";
                        break;
                    case SimulatorObject.Goomba:
                        mapping = "ENEMY_GOOMBA";
                        enemy = true;
                        width = 1;
                        height = 1;
                        break;
                    case SimulatorObject.Koopa:
                        mapping = "ENEMY_GREEN_KOOPA";
                        enemy = true;
                        width = 1;
                        height = 1;
                        break;
                }

                if(mapping != null)
                {
                    for(int i = 0; i < width; i++)
                    {
                        for(int j = 0; j < height; j++)
                        {
                            StringBuilder subBuilder = new StringBuilder();
                            subBuilder.Append("        ");
                            subBuilder.Append(enemy ? "setSpriteTemplate(" : "setBlock(");
                            subBuilder.Append((gridObject.X + i) + ", " + (GridHeight - gridObject.Y - j - 1) + ", ");
                            subBuilder.Append(enemy ? "new SpriteTemplate(Enemy." : "");
                            subBuilder.Append(mapping);
                            subBuilder.Append(enemy ? ", false)" : "");
                            subBuilder.Append(");");
                            builder.AppendLine(subBuilder.ToString());
                        }
                    }
                }
            }

            // Add hill tops
            for(int i = 0; i < GridWidth; i++)
            {
                bool prevGround = false;
                for(int j = GridHeight - 1; j >= 0; j--)
                {
                    GridObject gridObject = gridFunctional[i, j];
                    if(gridObject != null && gridObject.Data.Mapping == SimulatorObject.Ground)
                    {
                        if(!prevGround)
                        {
                            StringBuilder subBuilder = new StringBuilder();
                            subBuilder.Append("        setBlock(");
                            subBuilder.Append(i + ", " + (GridHeight - j - 1) + ", ");
                            subBuilder.Append("HILL_TOP);");
                            builder.AppendLine(subBuilder.ToString());

                            prevGround = true;
                        }
                    }
                    else
                    {
                        prevGround = false;
                    }
                }
            }

            // Add exit location
            builder.AppendLine("");
            builder.AppendLine("        xExit = " + exit[0] + ";");
            builder.AppendLine("        yExit = " + (GridHeight - exit[1]) + ";");

            builder.AppendLine("    }");
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}