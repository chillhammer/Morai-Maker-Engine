using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
    public class GridObject : MonoBehaviour
    {
        public SpriteData Sprite { get; private set; }

        // Top left corner grid coordinates
        public int X { get; private set; }
        public int Y { get; private set; }

        [SerializeField]
        private Image image;

        public void Initialize(SpriteData data, int x, int y)
        {
            Sprite = data;
            image.sprite = data.Sprite;
            RectTransform rect = ((RectTransform)transform);
            rect.sizeDelta = new Vector2(data.Width, data.Height);

            // TODO Stretching

            Reposition(x, y);
        }

        public void Reposition(int x, int y)
        {
            transform.position = new Vector2(x + (float)Sprite.Width / 2, y + (float)Sprite.Height / 2);
        }
    }
}