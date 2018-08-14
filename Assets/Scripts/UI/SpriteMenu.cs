using Assets.Scripts.Core;
using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class SpriteMenu : Lockable
    {
        [SerializeField]
        private SpriteMenuObject spritePrefab;
        [SerializeField]
        private Transform spriteParent;
        [SerializeField]
        private Camera spriteMenuCamera;
        [SerializeField]
        private GridPlacement gridPlacement;

        private List<SpriteMenuObject> spriteObjects;
        private SpriteMenuObject selectedSprite;

        private void Start()
        {
            // Get list of sprites for initial tab
            List<SpriteData> sprites = SpriteManager.Instance.GetSpriteList(SpriteManager.Instance.GetTagList()[0]);

            // Calculate max sprite height
            float maxHeight = 0;
            foreach(SpriteData sprite in sprites)
                maxHeight = Mathf.Max(maxHeight, sprite.Height);

            // Calculate some positioning values
            float padding = 0.2f;
            // - Sprite height is weighted towards 5 for more consistent scale between tabs
            float scale = (1 - padding * 2) / ((maxHeight + 5) / 2);
            float spriteX = padding;
            float spriteY = -1.5f;

            // Create a row of menu sprites
            spriteObjects = new List<SpriteMenuObject>();
            foreach(SpriteData sprite in sprites)
            {
                SpriteMenuObject temp = Instantiate(spritePrefab, new Vector2(spriteX + sprite.Width * scale / 2, spriteY), Quaternion.identity, spriteParent.transform);
                temp.Initialize(this, sprite);
                temp.SetScale(scale);
                spriteObjects.Add(temp);
                spriteX += sprite.Width * scale + padding;
            }

            // Adjust the sprite menu camera
            spriteMenuCamera.transform.position = new Vector3(0, spriteY, spriteMenuCamera.transform.position.z);
            spriteMenuCamera.GetComponent<CameraScroll>().SetBounds(0, spriteX);

            // Select first sprite by default
            SelectSprite(spriteObjects[0]);
        }

        public void SelectSprite(SpriteMenuObject sprite)
        {
            gridPlacement.CurrentSprite = sprite.Sprite;
            if(selectedSprite)
                selectedSprite.SetOutline(false);
            selectedSprite = sprite;
            selectedSprite.SetOutline(true);
        }

        public void DisplaySprites(List<SpriteData> sprites)
        {
            if(!IsLocked)
                StartCoroutine(DisplaySpritesCoroutine(sprites));
        }

        private IEnumerator DisplaySpritesCoroutine(List<SpriteData> sprites)
        {
            AddLock(this);
            spriteMenuCamera.GetComponent<CameraScroll>().AddLock(this);

            // Calculate max sprite height
            float maxHeight = 0;
            foreach(SpriteData sprite in sprites)
                maxHeight = Mathf.Max(maxHeight, sprite.Height);

            // Calculate some positioning values
            float padding = 0.2f;
            // - Sprite height is weighted towards 5 for more consistent scale between tabs
            float scale = (1 - padding * 2) / ((maxHeight + 5) / 2);
            float spriteX = spriteMenuCamera.transform.position.x - spriteMenuCamera.orthographicSize * spriteMenuCamera.aspect + padding;
            float spriteY = -0.5f;

            // Create a row of menu sprites
            List<SpriteMenuObject> newSpriteObjects = new List<SpriteMenuObject>();
            foreach(SpriteData sprite in sprites)
            {
                SpriteMenuObject temp = Instantiate(spritePrefab, new Vector2(spriteX + sprite.Width * scale / 2, spriteY), Quaternion.identity, spriteParent.transform);
                temp.Initialize(this, sprite);
                temp.SetScale(scale);
                newSpriteObjects.Add(temp);
                spriteX += sprite.Width * scale + padding;

                // - Reselect current sprite
                if(sprite.Equals(gridPlacement.CurrentSprite))
                    SelectSprite(temp);
            }

            // Scroll in the new row
            float offsetY = 0;
            for(int i = 0; i < 20; i++)
            {
                offsetY = Mathf.Lerp(offsetY, -1, 0.2f);
                foreach(SpriteMenuObject sprite in spriteObjects)
                    sprite.transform.position = new Vector3(sprite.transform.position.x, spriteMenuCamera.transform.position.y + offsetY, sprite.transform.position.z);
                foreach(SpriteMenuObject sprite in newSpriteObjects)
                    sprite.transform.position = new Vector3(sprite.transform.position.x, spriteY + offsetY, sprite.transform.position.z);

                yield return new WaitForFixedUpdate();
            }

            // Reset menu sprite positioning
            float offsetX = spriteMenuCamera.orthographicSize * spriteMenuCamera.aspect - spriteMenuCamera.transform.position.x;
            spriteMenuCamera.transform.position = new Vector3(0, spriteMenuCamera.transform.position.y, spriteMenuCamera.transform.position.z);
            spriteMenuCamera.GetComponent<CameraScroll>().SetBounds(0, spriteX + offsetX);
            foreach(SpriteMenuObject sprite in newSpriteObjects)
                sprite.transform.position = new Vector3(sprite.transform.position.x + offsetX, spriteMenuCamera.transform.position.y, sprite.transform.position.z);

            // Update list of menu sprites
            foreach(SpriteMenuObject sprite in spriteObjects)
                Destroy(sprite.gameObject);
            spriteObjects = newSpriteObjects;

            RemoveLock(this);
            spriteMenuCamera.GetComponent<CameraScroll>().RemoveLock(this);
        }
    }
}