using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class LevelBackground : MonoBehaviour
    {
        private void Awake()
        {
            GridManager.Instance.GridSizeChanged += OnGridSizeChanged;
        }

        private void OnGridSizeChanged(int x, int y)
        {
            transform.position = new Vector3((float)x / 2, (float)y / 2, transform.position.z);
            ((RectTransform)transform).sizeDelta = new Vector2(x, y);
        }
    }
}