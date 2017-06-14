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
            if(data.Stretch)
            {
                ((RectTransform)transform).sizeDelta = new Vector2(data.Width, data.Height);
            }
            else
            {
                float scale = Mathf.Min(data.Width / data.Sprite.bounds.size.x, data.Height / data.Sprite.bounds.size.y);
                ((RectTransform)transform).sizeDelta = scale * data.Sprite.bounds.size;
            }
        }

        public void SetPosition(int x, int y)
        {
            transform.position = new Vector2(x + (float)Sprite.Width / 2, y + (float)Sprite.Height / 2);
        }
    }
}