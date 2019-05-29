using Assets.Scripts.UI;
using Assets.Scripts.Util;
using UnityEngine;
using Assets.PlayabilityCheck.PathFinding;
using System.Collections.Generic;

namespace Assets.Scripts.Core
{
    public class GridPlacement : Lockable
    {
        [HideInInspector]
        public SpriteData CurrentSprite; // Initialized by the sprite menu

        [SerializeField]
        private GridObject previewObject;
        [SerializeField]
        private DialogueMenu dialogueMenu;
        
        private Vector2? previousMousePosition;
        private bool? deletionLayer; // Functional if true, decorative if false

		public enum PlacementMode { Level, Pathfinding }
		[SerializeField] private PlacementMode mode;

		//Pathfinding Variables
		private PathFinding pathFinding;
		[SerializeField] private int pathEndPoints = 0;
		private Vector2 pathStart;
		[SerializeField]
		private PlayabilityCheck.PathVisualization pathVis;

		protected override void Awake()
        {
            base.Awake();

            dialogueMenu.DialogueOpened += () => AddLock(dialogueMenu);
            dialogueMenu.DialogueOpened += () => previewObject.gameObject.SetActive(false);
            dialogueMenu.DialogueClosed += () => RemoveLock(dialogueMenu);
			mode = PlacementMode.Level;
			pathFinding = new PathFinding();
            Map.pathForBots = null;
		}

		private void Update()
		{

			// Calculate sprite coordinates for the current mouse position
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (previousMousePosition == null)
				previousMousePosition = mousePosition;

			#region Toggle Pathfinding Mode
			if (Input.GetKeyDown(KeyCode.P))
			{
				if (mode != PlacementMode.Pathfinding)
				{
					Debug.Log("Enabling Pathfinding Mode");
					mode = PlacementMode.Pathfinding;
				} else
				{
					Debug.Log("Disabling Pathfinding Mode");
					mode = PlacementMode.Level;
				}
				pathEndPoints = 0;
				pathVis.Clear();
			}
			#endregion

			if (!IsLocked)
			{
				// Interpolate between previous and current mouse position
				int spriteX = 0, spriteY = 0;
				for (float i = 0.25f; i <= 1; i += 0.25f)
				{
					spriteX = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.x, mousePosition.x, i) - (float)CurrentSprite.Width / 2);
					spriteY = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.y, mousePosition.y, i) - (float)CurrentSprite.Height / 2);

					if (mode == PlacementMode.Level)
					{
						if (Input.GetMouseButton(1))
						{
							int mouseX = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.x, mousePosition.x, i) - 0.5f);
							int mouseY = Mathf.RoundToInt(Mathf.Lerp(previousMousePosition.Value.y, mousePosition.y, i) - 0.5f);

							// Set deletion layer if not set, prioritizing the functional layer
							if (deletionLayer == null)
							{
								if (GridManager.Instance.ContainsGridObject(true, mouseX, mouseY))
									deletionLayer = true;
								else if (GridManager.Instance.ContainsGridObject(false, mouseX, mouseY))
									deletionLayer = false;
							}

							// Remove existing grid object based on deletion layer
							if (deletionLayer != null)
								if (GridManager.Instance.ContainsGridObject(deletionLayer.Value, mouseX, mouseY))
									GridManager.Instance.RemoveGridObject(deletionLayer.Value, mouseX, mouseY);
						}
						else if (Input.GetMouseButton(0) && CurrentSprite.HoldToPlace && GridManager.Instance.CanAddGridObject(CurrentSprite, spriteX, spriteY))
						{
							pathVis.Clear();
							// Place new grid object (if hold-to-place)
							GridManager.Instance.AddGridObject(CurrentSprite, spriteX, spriteY, true);
						}

					}
				}
				//Pathfinding
				if (mode == PlacementMode.Pathfinding)
				{
					//Simple Line
					if (pathEndPoints == 1)
						pathVis.SetDrawLine(pathStart, mousePosition);

					//Clicking
					if (Input.GetMouseButton(0) && pathEndPoints == 0)
					{
						pathStart = new Vector2(spriteX, spriteY);
						pathEndPoints = 1;
					}
					else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0) && pathEndPoints == 1)
					{
						Vector2 pathEnd = new Vector2(spriteX, spriteY); ;
						if (pathStart != pathEnd)
						{
							// Actually calculates path!
							List<Vector2> path = pathFinding.FindPath(pathStart, pathEnd);
							pathEndPoints = 2;
							pathVis.SetDrawLine(path);
							Map.pathForBots = path;
                            Map.pathForBots.Reverse();
						}
					}
					//Exit Condition
					if (Input.GetMouseButtonUp(0) && pathEndPoints == 2)
					{
						pathEndPoints = 0;
						mode = PlacementMode.Level;
					}
				}

				else if (mode == PlacementMode.Level)
				{
					// Remove deletion layer
					if (Input.GetMouseButtonUp(1))
						deletionLayer = null;
				}
				// Update preview object
				if (CurrentSprite.Sprite != previewObject.Data.Sprite)
					previewObject.SetSprite(CurrentSprite);
				previewObject.SetPosition(spriteX, spriteY);
				previewObject.gameObject.SetActive(GridManager.Instance.CanAddGridObject(CurrentSprite, spriteX, spriteY));
			}

			// Store mouse position
			previousMousePosition = mousePosition;
		}

		private void OnApplicationFocus(bool hasFocus)
        {
            previewObject.gameObject.SetActive(false);
            previousMousePosition = null;
        }
    }
}