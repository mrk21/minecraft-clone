using UnityEngine;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class PlayerBehaviour : MonoBehaviour {
		public TerrainService terrainService;

		private float velocityScale = 1f;
		private bool isMovingToForward = false;
		private bool isMovingToBack = false;
		private bool isMovingToRight = false;
		private bool isMovingToLeft = false;
		private bool isJumping = false;

        void Start() {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = true;
		}

		void Update () {
			velocityScale = 1f;

			if (terrainService.BlockUnderPlayer () is FluidBlock || terrainService.BlockUnderPlayer (Vector3.up) is FluidBlock) {
				GetComponent<Rigidbody> ().drag = 3f;
				velocityScale = 0.5f;
			} else {
				GetComponent<Rigidbody> ().drag = 0.01f;
			}

			isMovingToForward = EnabledOperation() && Input.GetKey (KeyCode.W);
			isMovingToBack = EnabledOperation() && Input.GetKey (KeyCode.S);
			isMovingToLeft = EnabledOperation() && Input.GetKey (KeyCode.A);
			isMovingToRight = EnabledOperation() && Input.GetKey (KeyCode.D);
			isJumping = EnabledOperation() && Input.GetKey (KeyCode.Space);

			if (EnabledOperation()) {
				float yRotation = 4.0f * Input.GetAxis("Mouse X");
				transform.Rotate(0, yRotation, 0);
			}
			if (EnabledOperation() && Input.GetKeyDown(KeyCode.Escape)) {
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;
			}
			if (!EnabledOperation() && Input.GetMouseButtonDown(0)) {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		void FixedUpdate () {
			if (isMovingToForward) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					5 * velocityScale,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			}
			if (isMovingToBack) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					-5 * velocityScale,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			}
			if (isMovingToLeft) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					5 * velocityScale
				));
			}
			if (isMovingToRight) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					-5 * velocityScale
				));
			}
			if (isJumping) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					5 * velocityScale,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			}
		}

		bool EnabledOperation() {
			return Cursor.lockState == CursorLockMode.Locked;
		}
	}
}