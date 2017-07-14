using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        public bool GridView { get; private set; }
        public bool HoverScroll { get; private set; }

        [SerializeField]
        private FileMenu fileMenu;
        [SerializeField]
        private GameObject gridLines;

        [SerializeField]
        private Image gridViewIcon;
        [SerializeField]
        private Image hoverScrollIcon;
        [SerializeField]
        private InputField levelNameInput;
        [SerializeField]
        private InputField levelWidthInput;
        [SerializeField]
        private InputField levelHeightInput;

        [SerializeField]
        private Sprite iconCheck;
        [SerializeField]
        private Sprite iconCross;

        private bool placeholderGridView;
        private bool placeholderHoverScroll;

        public void Awake()
        {
            // Initialize options to defaults
            GridView = true;
            HoverScroll = true;
            ResetPlaceholderOptions();
        }

        public void ToggleGridView()
        {
            placeholderGridView = !placeholderGridView;
            gridViewIcon.sprite = placeholderGridView ? iconCheck : iconCross;
        }

        public void ToggleHoverScroll()
        {
            placeholderHoverScroll = !placeholderHoverScroll;
            hoverScrollIcon.sprite = placeholderHoverScroll ? iconCheck : iconCross;
        }

        public void SavePlaceholderOptions()
        {
            // Apply new grid size
            int levelWidth = int.Parse(levelWidthInput.text);
            int levelHeight = int.Parse(levelHeightInput.text);
            if(levelWidth != GridManager.Instance.GridWidth || levelHeight != GridManager.Instance.GridHeight)
                GridManager.Instance.SetGridSize(levelWidth, levelHeight, true);

            // Apply grid view
            gridLines.SetActive(placeholderGridView);

            // Apply hover scroll
            if(HoverScroll && !placeholderHoverScroll)
                Camera.main.GetComponent<CameraScroll>().AddLock(this);
            else if(!HoverScroll && placeholderHoverScroll)
                Camera.main.GetComponent<CameraScroll>().RemoveLock(this);

            // Save the options
            GridView = placeholderGridView;
            HoverScroll = placeholderHoverScroll;
            fileMenu.LevelName = levelNameInput.text;
        }

        public void ResetPlaceholderOptions()
        {
            placeholderGridView = GridView;
            placeholderHoverScroll = HoverScroll;

            gridViewIcon.sprite = GridView ? iconCheck : iconCross;
            hoverScrollIcon.sprite = HoverScroll ? iconCheck : iconCross;

            levelNameInput.text = fileMenu.LevelName == null ? "" : fileMenu.LevelName;

            levelWidthInput.text = GridManager.Instance.GridWidth.ToString();
            levelHeightInput.text = GridManager.Instance.GridHeight.ToString();
        }
    }
}