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

        public void SetSprite(SpriteData data)
        {
            Sprite = data;
            image.sprite = data.Sprite;
            ((RectTransform)transform).sizeDelta = new Vector2(data.Width, data.Height);

            // TODO Stretching
        }

        public void SetPosition(int x, int y)
        {
            transform.position = new Vector2(x + (float)Sprite.Width / 2, y + (float)Sprite.Height / 2);
        }

        public void SetAlpha(float alpha)
        {
            Color temp = image.color;
            temp.a = alpha;
            image.color = temp;
        }
    }
}