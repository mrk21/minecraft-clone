using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinecraftClone.Application.Behaviour {
	class PlayerBehaviour : MonoBehaviour {
		void Update () {
			if (Input.GetKey (KeyCode.W)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					5,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			} else if (Input.GetKey (KeyCode.S)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					-5,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			}

			if (Input.GetKey (KeyCode.A)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					5
				));
			} else if (Input.GetKey (KeyCode.D)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y,
					-5
				));
			}

			if (Input.GetKey (KeyCode.Space)) {
				GetComponent<Rigidbody> ().velocity = transform.TransformDirection (new Vector3(
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).x,
					5,
					transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z
				));
			}

			if (Input.GetKey (KeyCode.RightArrow)) {
				transform.Rotate (new Vector3 (0, 4, 0));
			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				transform.Rotate (new Vector3 (0, -4, 0));
			}
		}
	}
}
