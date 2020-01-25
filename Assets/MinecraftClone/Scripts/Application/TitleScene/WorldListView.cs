using MinecraftClone.Domain;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class WorldListView : MonoBehaviour
    {
        [SerializeField] private RectTransform itemPrefab = null;
        private ScrollRect scrollView;

        public ReactiveDictionary<string, WorldListItemView> items;
        public Subject<WorldListItemView> onClickJoinButton;
        public Subject<WorldListItemView> onClickDeleteButton;

        private void Awake()
        {
            scrollView = GetComponent<ScrollRect>();
            items = new ReactiveDictionary<string, WorldListItemView>();
            onClickJoinButton = new Subject<WorldListItemView>();
            onClickDeleteButton = new Subject<WorldListItemView>();
        }

        public void AddItem(string id, string name, Seed seed)
        {
            if (items.ContainsKey(id)) return;

            var itemGameObject = GameObject.Instantiate(itemPrefab) as RectTransform;
            itemGameObject.SetParent(scrollView.content.transform, false);

            var item = itemGameObject.GetComponent<WorldListItemView>();

            item.worldId = id;
            item.worldName.text = name;
            item.worldSeed.text = seed.Base.ToString();

            item.joinButton
                .OnClickAsObservable()
                .Subscribe(_ => onClickJoinButton.OnNext(item))
                .AddTo(gameObject);

            item.deleteButton
                .OnClickAsObservable()
                .Subscribe(_ => onClickDeleteButton.OnNext(item))
                .AddTo(gameObject);

            items.Add(id, item);
        }

        public void DeleteItem(string id)
        {
            Destroy(items[id].gameObject);
            items.Remove(id);
        }
    }
}