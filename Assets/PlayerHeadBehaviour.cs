using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadBehaviour : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			transform.Rotate (new Vector3 (4, 0, 0));
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			transform.Rotate (new Vector3 (-4, 0, 0));
		}
	}
}