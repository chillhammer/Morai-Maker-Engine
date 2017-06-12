using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public enum SpriteName { Ground, Tree }

    [System.Serializable]
    public struct SpriteData
    {
        public SpriteName Name;
        public Sprite Sprite;
        public bool Stretch;
        
        public int Width;
        public int Height;
        public bool Functional;
    }

    public class SpriteManager : Singleton<SpriteManager>
    {
        [SerializeField]
        private List<SpriteData> spriteList;
        private Dictionary<SpriteName, SpriteData> spriteDictionary;

        protected override void Awake()
        {
            base.Awake();

            // Create sprite dictionary
            spriteDictionary = new Dictionary<SpriteName, SpriteData>();
            foreach(SpriteData data in spriteList)
                spriteDictionary[data.Name] = data;
        }

        public static SpriteData GetSpriteData(SpriteName sprite)
        {
            return Instance.spriteDictionary[sprite];
        }
    }
}