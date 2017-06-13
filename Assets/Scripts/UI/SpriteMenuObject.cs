using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SpriteMenuObject : MonoBehaviour
    {
        public SpriteMenu SpriteMenu { get; private set; }
        public SpriteData Sprite { get; private set; }

        [SerializeField]
        private Image image;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Outline outline;

        public void Initialize(SpriteMenu menu, SpriteData data)
        {
            SpriteMenu = menu;
            Sprite = data;

            // Initialize sprite image
            image.sprite = data.Sprite;
            ((RectTransform)transform).sizeDelta = new Vector2(data.Width, data.Height);
            // TODO Stretching

            // Set button response
            button.onClick.AddListener(delegate { SpriteMenu.SelectSprite(this); });
        }

        public void SetOutline(bool enabled)
        {
            outline.enabled = enabled;
        }
    }
}