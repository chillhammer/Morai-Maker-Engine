using Assets.Scripts.Core;
using System.Collections;
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
        private bool busy;

        // TODO Temporary
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
                SelectTab(Random.Range(0f, 1f) > 0.5f ? "Block" : "Decor");
        }

        private void Start()
        {
            // Calculate max sprite width and height
            List<SpriteData> sprites = SpriteManager.Instance.GetSpriteList(SpriteManager.Instance.GetTagList()[0]);
            float maxWidth = 0, maxHeight = 0;
            foreach(SpriteData sprite in sprites)
            {
                maxWidth = Mathf.Max(maxWidth, sprite.Width);
                maxHeight = Mathf.Max(maxHeight, sprite.Height);
            }

            // Calculate some positioning values
            float padding = 0.2f;
            // - Sprite height is weighted towards 10 for more consistent scale between tabs
            float scale = (1 - padding * 2) / ((maxHeight + 10) / 2);
            float spriteX = padding;
            float spriteY = -1.5f;

            // Create a row of menu sprites
            spriteObjects = new List<SpriteMenuObject>();
            foreach(SpriteData sprite in sprites)
            {
                SpriteMenuObject temp = Instantiate(spritePrefab, new Vector2(spriteX + sprite.Width * scale / 2, spriteY), Quaternion.identity, spriteParent);
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
            GridPlacement.Instance.CurrentSprite = sprite.Sprite;
            if(selectedSprite)
                selectedSprite.SetOutline(false);
            selectedSprite = sprite;
            selectedSprite.SetOutline(true);
        }

        public void SelectTab(string tabName)
        {
            if(!busy)
                StartCoroutine(SelectTabCoroutine(tabName));
        }

        private IEnumerator SelectTabCoroutine(string tabName)
        {
            busy = true;
            spriteMenuCamera.GetComponent<CameraScroll>().MouseScroll = false;

            // Calculate max sprite width and height
            List<SpriteData> sprites = SpriteManager.Instance.GetSpriteList(tabName);
            float maxWidth = 0, maxHeight = 0;
            foreach(SpriteData sprite in sprites)
            {
                maxWidth = Mathf.Max(maxWidth, sprite.Width);
                maxHeight = Mathf.Max(maxHeight, sprite.Height);
            }

            // Calculate some positioning values
            float padding = 0.2f;
            // - Sprite height is weighted towards 10 for more consistent scale between tabs
            float scale = (1 - padding * 2) / ((maxHeight + 10) / 2);
            float spriteX = padding;
            float spriteY = -0.5f;

            // Create a row of menu sprites
            List<SpriteMenuObject> newSpriteObjects = new List<SpriteMenuObject>();
            foreach(SpriteData sprite in sprites)
            {
                SpriteMenuObject temp = Instantiate(spritePrefab, new Vector2(spriteX + sprite.Width * scale / 2, spriteY), Quaternion.identity, spriteParent);
                temp.Initialize(this, sprite);
                temp.SetScale(scale);
                newSpriteObjects.Add(temp);
                spriteX += sprite.Width * scale + padding;
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

            busy = false;
            spriteMenuCamera.GetComponent<CameraScroll>().MouseScroll = true;
        }
    }
}