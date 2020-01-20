using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class MainScreenView : MonoBehaviour
    {
        [SerializeField] public InputField currentSeedField = null;
        [SerializeField] public Button closeMenuButton = null;
        [SerializeField] public Button backToTitleButton = null;
    }
}