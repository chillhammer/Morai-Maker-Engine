using Assets.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class TabMenu : MonoBehaviour
    {
        [SerializeField]
        private Button tabPrefab;
        [SerializeField]
        private SpriteMenu spriteMenu;

        private void Start()
        {
            // Generate tabs
            foreach(string tag in SpriteManager.Instance.GetTagList())
            {
                Button temp = Instantiate(tabPrefab, transform);
                temp.GetComponentInChildren<Text>().text = tag;
                temp.onClick.AddListener(delegate { spriteMenu.SelectTab(tag); });
            }
        }
    }
}