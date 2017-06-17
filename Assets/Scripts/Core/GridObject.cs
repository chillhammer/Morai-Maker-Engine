using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Core
{
    public class GridObject : MonoBehaviour
    {
        public SpriteData Data { get; private set; }

        // Top left corner grid coordinates
        public int X { get; private set; }
        public int Y { get; private set; }

        [SerializeField]
        private Image image;

        public void SetSprite(SpriteData data)
        {
            Data = data;
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
        }

        public void SetPosition(int x, int y)
        {
            X = x;
            Y = y;

            transform.position = new Vector2(x + (float)Data.Width / 2, y + (float)Data.Height / 2);
        }
    }
}