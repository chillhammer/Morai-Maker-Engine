using Assets.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SpriteMenu : MonoBehaviour
    {
        [SerializeField]
        private SpriteMenuObject spritePrefab;
        [SerializeField]
        private Transform spriteParent;
        [SerializeField]
        private Camera spriteMenuCamera;

        private List<SpriteMenuObject> spriteObjects;
        private SpriteMenuObject selectedSprite;

        private void Start()
        {
            // Calculate max sprite height
            List<SpriteData> sprites = SpriteManager.Instance.GetSpriteList();
            float spriteX = 1, spriteY = 0;
            foreach(SpriteData sprite in sprites)
                spriteY = Mathf.Max(spriteY, sprite.Height);
            spriteY = spriteY / 2 + 1;

            // Create a row of menu sprites
            spriteObjects = new List<SpriteMenuObject>();
            foreach(SpriteData sprite in sprites)
            {
                SpriteMenuObject temp = Instantiate(spritePrefab, new Vector2(spriteX + (float)sprite.Width / 2, -spriteY), Quaternion.identity, spriteParent);
                temp.Initialize(this, sprite);
                spriteObjects.Add(temp);
                spriteX += sprite.Width + 1;
            }

            // Adjust the sprite menu camera
            spriteMenuCamera.orthographicSize = spriteY;
            spriteMenuCamera.transform.position = new Vector3(0, -spriteY, spriteMenuCamera.transform.position.z);
            spriteMenuCamera.GetComponent<CameraScroll>().SetBounds(0, spriteX);

            // Select first sprite by default
            SelectSprite(spriteObjects[0]);
        }

        public void SelectSprite(SpriteMenuObject sprite)
        {
            GridPlacement.Instance.CurrentSprite = sprite.Sprite;
            if(selectedSprite)
                selectedSprite.SetOutline(false);
            selectedSprite = sprite;
            selectedSprite.SetOutline(true);
        }
    }
}