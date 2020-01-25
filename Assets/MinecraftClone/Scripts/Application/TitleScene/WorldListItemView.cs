using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class WorldListItemView : MonoBehaviour
    {
        [SerializeField] public string worldId = null;
        [SerializeField] public Text worldName = null;
        [SerializeField] public Text worldSeed = null;
        [SerializeField] public Button joinButton = null;
        [SerializeField] public Button deleteButton = null;
    }
}