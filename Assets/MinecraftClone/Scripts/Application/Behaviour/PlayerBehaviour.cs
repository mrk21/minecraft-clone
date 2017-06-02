using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class PlayerBehaviour : MonoBehaviour {
		public TerrainService terrainService;

		private float velocityScale = 1f;
		private bool isTurningRight = false;
		private bool isTurningLeft = false;
		private bool isMovingToForward = false;
		private bool isMovingToBack = false;
		private bool isMovingToRight = false;
		private bool isMovingToLeft = false;
		private bool isJumping = false;

		void Start() {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
		}

		void Update () {
			velocityScale = 1f;

			if (terrainService.BlockUnderPlayer () is FluidBlock || terrainService.BlockUnderPlayer (Vector3.up) is FluidBlock) {
				GetComponent<Rigidbody> ().drag = 3f;
				velocityScale = 0.5f;
			} else {
				GetComponent<Rigidbody> ().drag = 0.01f;
			}

			isTurningRight = Input.GetKey (KeyCode.RightArrow);
			isTurningLeft = Input.GetKey (KeyCode.LeftArrow);
			isMovingToForward = Input.GetKey (KeyCode.W);
			isMovingToBack = Input.GetKey (KeyCode.S);
			isMovingToLeft = Input.GetKey (KeyCode.A);
			isMovingToRight = Input.GetKey (KeyCode.D);
			isJumping = Input.GetKey (KeyCode.Space);
		}

		void FixedUpdate () {
			if (isTurningRight) transform.Rotate (new Vector3 (0, 4, 0));
			if (isTurningLeft) transform.Rotate (new Vector3 (0, -4, 0));

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
	}
}