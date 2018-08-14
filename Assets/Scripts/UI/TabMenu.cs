using Assets.Scripts.Core;
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

        private string currentTab;
        private string pendingTab;

        private void Start()
        {
            // Get initial tab
            currentTab = SpriteManager.Instance.GetTagList()[0];

            // Generate tabs
            foreach(string tag in SpriteManager.Instance.GetTagList())
            {
                Button temp = Instantiate(tabPrefab, transform);
                temp.GetComponentInChildren<Text>().text = tag;
                temp.onClick.AddListener(() => OnButtonClick(tag));
            }
        }

        private void Update()
        {
            if(pendingTab != null && !spriteMenu.IsLocked)
            {
                spriteMenu.DisplaySprites(SpriteManager.Instance.GetSpriteList(pendingTab));
                currentTab = pendingTab;
                pendingTab = null;
            }
        }

        private void OnButtonClick(string tag)
        {
            if(currentTab == null || !currentTab.Equals(tag))
            {
                pendingTab = tag;
            }
        }
    }
}