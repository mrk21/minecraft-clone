using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class CreateWorldScreenView : MonoBehaviour
    {
        [SerializeField] public Button newWorldButton = null;
        [SerializeField] public Button backButton = null;
        [SerializeField] public CreateWorldModalView createWorldModal = null;
        [SerializeField] public EditWorldModalView editWorldModal = null;
        [SerializeField] public WorldListView worldList = null;

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }
}