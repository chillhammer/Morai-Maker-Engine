using Assets.Scripts.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        public bool GridView { get; private set; }
        public bool HoverScroll { get; private set; }
        public string LevelName { get; private set; }

        [SerializeField]
        private Image gridViewIcon;
        [SerializeField]
        private Image hoverScrollIcon;
        [SerializeField]
        private InputField levelNameInput;

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
            LevelName = "level";
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
            // Apply the options
            // TODO GridView
            if(HoverScroll && !placeholderHoverScroll)
                Camera.main.GetComponent<CameraScroll>().AddLock(this);
            else if(!HoverScroll && placeholderHoverScroll)
                Camera.main.GetComponent<CameraScroll>().RemoveLock(this);

            // Save the options
            GridView = placeholderGridView;
            HoverScroll = placeholderHoverScroll;
            LevelName = levelNameInput.text.ToLower().Replace(' ', '_');
        }

        public void ResetPlaceholderOptions()
        {
            placeholderGridView = GridView;
            placeholderHoverScroll = HoverScroll;

            gridViewIcon.sprite = GridView ? iconCheck : iconCross;
            hoverScrollIcon.sprite = HoverScroll ? iconCheck : iconCross;

            levelNameInput.text = LevelName;
        }

        public void SetLevelName(string levelName)
        {
            LevelName = levelName.ToLower().Replace(' ', '_');
            levelNameInput.text = LevelName;
        }
    }
}