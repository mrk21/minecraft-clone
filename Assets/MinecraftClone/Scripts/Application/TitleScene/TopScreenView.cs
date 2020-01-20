using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class TopScreenView : MonoBehaviour
    {
        [SerializeField] public Button playButton = null;
        [SerializeField] public Button quitButton = null;

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }
    }
}