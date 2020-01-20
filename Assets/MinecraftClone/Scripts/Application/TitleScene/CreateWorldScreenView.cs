using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class CreateWorldScreenView : MonoBehaviour
    {
        [SerializeField] public InputField worldSeedField = null;
        [SerializeField] public Button createWorldButton = null;
        [SerializeField] public Button cancelButton = null;

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }
}