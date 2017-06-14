using Assets.Scripts.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public enum SpriteName { Ground, Tree, Cannon, Castle, Cloud }

    [System.Serializable]
    public struct SpriteData
    {
        public SpriteName Name;
        public Sprite Sprite;
        public bool Stretch;
        
        public int Width;
        public int Height;
        public bool Functional;
        public bool HoldToPlace;
    }

    public class SpriteManager : Singleton<SpriteManager>
    {
        [SerializeField]
        private List<SpriteData> spriteList;
        private Dictionary<SpriteName, SpriteData> spriteDictionary;

        protected override void Awake()
        {
            base.Awake();

            // Create sprite dictionary and remove duplicates
            spriteDictionary = new Dictionary<SpriteName, SpriteData>();
            for(int i = 0; i < spriteList.Count; i++)
            {
                SpriteData data = spriteList[i];
                if(spriteDictionary.ContainsKey(data.Name))
                {
                    spriteList.RemoveAt(i);
                    i--;
                }
                else
                {
                    spriteDictionary[data.Name] = data;
                }
            }
        }

        public SpriteData GetSpriteData(SpriteName sprite)
        {
            return Instance.spriteDictionary[sprite];
        }

        public List<SpriteData> GetSpriteList()
        {
            List<SpriteData> clone = new List<SpriteData>();
            foreach(SpriteData data in spriteList)
                clone.Add(data);
            return clone;
        }
    }
}