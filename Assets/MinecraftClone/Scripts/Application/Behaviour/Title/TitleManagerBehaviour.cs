using UnityEngine;

namespace MinecraftClone.Application.Behaviour.Title
{
    class TitleManagerBehaviour : MonoBehaviour
    {
        [Header("Screens")]
        [SerializeField]
        public GameObject topScreen = null;
        [SerializeField]
        public GameObject createWorldScreen = null;

        void Start()
        {
            topScreen.GetComponent<Canvas>().enabled = true;
            createWorldScreen.GetComponent<Canvas>().enabled = false;
        }
    }
}