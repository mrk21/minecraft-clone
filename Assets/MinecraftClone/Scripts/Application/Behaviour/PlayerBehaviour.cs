using UnityEngine;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Block.Fluid;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Application.Behaviour {
	class PlayerBehaviour : MonoBehaviour {
		public TerrainService terrainService;

		void Start() {
			if (terrainService == null) terrainService = Singleton<TerrainService>.Instance;
		}

		void Update () {
			float velocityScale = 1f;

			if (terrainService.BlockUnderPlayer () is FluidBlock || terrainService.BlockUnderPlayer (Vector3.up) is FluidBlock) {
				GetComponent<Rigidbody> ().drag = 3f;
				velocityScale = 0.5f;
			} else {
				GetComponent<Rigidbody> ().drag = 0.01f;
			}

			if (Input.GetKey (KeyCode.RightArrow)) {
				transform.Rotate (new Vector3 (0, 4, 0));
			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				transform.Rotate (new Vector3 (0, -4, 0));
			}

			if (Input.GetKey (KeyCode.W)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					5 * velocityScale,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			} else if (Input.GetKey (KeyCode.S)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					-5 * velocityScale,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			}

			if (Input.GetKey (KeyCode.A)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					5 * velocityScale
				));
			} else if (Input.GetKey (KeyCode.D)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					-5 * velocityScale
				));
			}

			if (Input.GetKey (KeyCode.Space)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					5 * velocityScale,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			}
		}
	}
}