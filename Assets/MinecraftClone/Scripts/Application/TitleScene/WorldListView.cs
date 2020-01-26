using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class WorldListView : MonoBehaviour
    {
        [SerializeField] private RectTransform itemPrefab = null;
        [SerializeField] private GameObject itemPrefabBase = null;
        private ScrollRect scrollView;

        public ReactiveDictionary<string, WorldListItemView> Items { get; }
            = new ReactiveDictionary<string, WorldListItemView>();

        private void Awake()
        {
            scrollView = GetComponent<ScrollRect>();
            Destroy(itemPrefabBase);
        }

        public void AddItem(Action<WorldListItemView> setter)
        {
            var item = Instantiate(itemPrefab).GetComponent<WorldListItemView>();
            setter(item);
            item.GetComponent<RectTransform>().SetParent(scrollView.content.transform, false);
            Items.Add(item.worldId, item);
        }

        public void DeleteItem(string id)
        {
            Destroy(Items[id].gameObject);
            Items.Remove(id);
        }
    }
}