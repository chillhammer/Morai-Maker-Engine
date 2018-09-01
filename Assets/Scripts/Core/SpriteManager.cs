using Assets.Scripts.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{

    [System.Serializable]
    public struct SpriteData
    {
        public string Name;
        public Sprite Sprite;

        public int Width;
        public int Height;

        public bool Functional;
        public bool MaintainAspect;
        public bool HoldToPlace;
    }

    public class SpriteManager : Singleton<SpriteManager>
    {
        [System.Serializable]
        private struct TagData
        {
            public string Tag;
            public List<SpriteData> Sprites;
        }

        [SerializeField]
        private List<TagData> tagList;
        
        private Dictionary<string, List<SpriteData>> tagDictionary;
        private Dictionary<string, SpriteData> spriteDictionary;

        protected override void Awake()
        {
            base.Awake();

            // Initialize sprite cache
            tagDictionary = new Dictionary<string, List<SpriteData>>();
            spriteDictionary = new Dictionary<string, SpriteData>();
            foreach(TagData tagData in tagList)
            {
                tagDictionary[tagData.Tag] = tagData.Sprites;
                foreach(SpriteData sprite in tagData.Sprites)
                    spriteDictionary[sprite.Name] = sprite;
            }
        }

        public SpriteData GetSprite(string name)
        {
			if (!spriteDictionary.ContainsKey (name)) {
				return spriteDictionary ["Goomba"];
			}
            return spriteDictionary[name];
        }

        public List<SpriteData> GetSpriteList(string tag)
        {
            List<SpriteData> clone = new List<SpriteData>();
            if(tagDictionary.ContainsKey(tag))
                foreach(SpriteData data in tagDictionary[tag])
                    clone.Add(data);
            return clone;
        }

        public List<string> GetTagList()
        {
            List<string> clone = new List<string>();
            foreach(TagData tagData in tagList)
                clone.Add(tagData.Tag);
            return clone;
        }
    }
}