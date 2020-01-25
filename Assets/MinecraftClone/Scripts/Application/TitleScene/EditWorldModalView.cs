using UnityEngine;
using UnityEngine.UI;

namespace MinecraftClone.Application.TitleScene
{
    class EditWorldModalView : MonoBehaviour
    {
        [SerializeField] public string worldId = null;
        [SerializeField] public InputField worldNameField = null;
        [SerializeField] public Text worldSeedField = null;
        [SerializeField] public Button saveButton = null;
        [SerializeField] public Button cancelButton = null;

        public void SetEnabled(bool enabled)
        {
            gameObject.SetActive(enabled);
        }

        public void Init(string worldId, string worldName = "", string worldSeed = "")
        {
            this.worldId = worldId;
            worldNameField.text = worldName;
            worldSeedField.text = worldSeed;
        }
    }
}