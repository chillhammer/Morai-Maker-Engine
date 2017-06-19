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
        private Text levelNameText;

        [SerializeField]
        private Sprite iconCheck;
        [SerializeField]
        private Sprite iconCross;

        public void Awake()
        {
            // Initialize options to defaults
            GridView = true;
            gridViewIcon.sprite = iconCheck;

            HoverScroll = true;
            hoverScrollIcon.sprite = iconCheck;

            SetLevelName("level");
        }

        public void ToggleGridView()
        {
            // TODO
            GridView = !GridView;
            gridViewIcon.sprite = GridView ? iconCheck : iconCross;
        }

        public void ToggleHoverScroll()
        {
            // TODO
            HoverScroll = !HoverScroll;
            hoverScrollIcon.sprite = HoverScroll ? iconCheck : iconCross;
        }

        public void OnLevelName()
        {
            SetLevelName(levelNameText.text);
        }

        public void SetLevelName(string levelName)
        {
            LevelName = levelName.ToLower().Replace(' ', '_');
            levelNameText.text = LevelName;
        }
    }
}