using UnityEngine;

namespace MinecraftClone.Application.Behaviour {
	class PlayerHeadBehaviour : MonoBehaviour {
		void Update () {
			if (Input.GetKey (KeyCode.UpArrow)) {
				transform.Rotate (new Vector3 (-4, 0, 0));
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				transform.Rotate (new Vector3 (4, 0, 0));
			}
		}
	}
}