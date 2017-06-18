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

        private Vector2 baseSize;
        private Vector2 baseOutlineDistance;

        public void Initialize(SpriteMenu menu, SpriteData data)
        {
            SpriteMenu = menu;
            Sprite = data;

            // Initialize sprite image
            image.sprite = data.Sprite;
            if(data.MaintainAspect)
            {
                float scale = Mathf.Min(data.Width / data.Sprite.bounds.size.x, data.Height / data.Sprite.bounds.size.y);
                ((RectTransform)transform).sizeDelta = scale * data.Sprite.bounds.size;
            }
            else
            {
                ((RectTransform)transform).sizeDelta = new Vector2(data.Width, data.Height);
            }

            // Store defaults
            baseSize = ((RectTransform)transform).sizeDelta;
            baseOutlineDistance = outline.effectDistance;

            // Set button response
            button.onClick.AddListener(() => SpriteMenu.SelectSprite(this));
        }

        public void SetScale(float scale)
        {
            ((RectTransform)transform).sizeDelta = baseSize * scale;
            outline.effectDistance = new Vector2(baseOutlineDistance.x * scale, baseOutlineDistance.y * scale);
        }

        public void SetOutline(bool enabled)
        {
            outline.enabled = enabled;
        }
    }
}