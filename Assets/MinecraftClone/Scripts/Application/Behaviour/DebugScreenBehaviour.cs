using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MinecraftClone.Domain.Block;
using MinecraftClone.Domain.Terrain;

namespace MinecraftClone.Application.Behaviour {
	public class DebugScreenBehaviour : MonoBehaviour {
		public Chunk currentChunk;
		public BaseBlock currentBlock;
		public Vector3 currentPosition;

		void Start () {
			GetComponent<Text> ().text = "";
		}
		
		void Update () {
			string text = "";
			if (currentChunk != null) {
				text += $"CurrentCunk: {currentChunk.Address}\n";
			}
			if (currentBlock != null) {
				text += $"CurrentBlock: {currentBlock}\n";
			}
			if (currentPosition != null) {
				text += $"CurrentPosition: {currentPosition}\n";
			}
			GetComponent<Text> ().text = text;
		}
	}
}