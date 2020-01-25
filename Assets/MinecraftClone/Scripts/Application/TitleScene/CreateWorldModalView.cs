using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class CreateWorldModalView : MonoBehaviour
    {
        [SerializeField] public InputField worldNameField = null;
        [SerializeField] public InputField worldSeedField = null;
        [SerializeField] public Button createButton = null;
        [SerializeField] public Button cancelButton = null;

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }

        public void Init(string worldName = "New World", string worldSeed = "")
        {
            worldNameField.text = worldName;
            worldSeedField.text = worldSeed;
        }
    }
}