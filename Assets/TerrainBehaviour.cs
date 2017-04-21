using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HSV {
	public int h;
	public int s;
	public int v;

	public HSV(int h, int s, int v) {
		this.h = h;
		this.s = s;
		this.v = v;
	}

	public Color ToRGB() {
		return UnityEngine.Color.HSVToRGB (
			this.h / 255f,
			this.s / 255f,
			this.v / 255f
		);
	}
}

public class Block {
	public const int BlockSize = 1;
	public GameObject obj;

	public Block(GameObject parent, Vector3 position, HSV color) {
		this.init (parent, position, color.ToRGB ());
	}

	public Block(GameObject parent, Vector3 position, Color color) {
		this.init (parent, position, color);
	}

	private void init(GameObject parent, Vector3 position, Color color) {
		this.obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
		this.obj.transform.parent = parent.transform;
		this.obj.name = string.Format ("Block({0},{1},{2})",
			position.x,
			position.y,
			position.z
		);
		this.obj.transform.position = new Vector3 (
			position.x * BlockSize,
			position.y * BlockSize,
			position.z * BlockSize
		);
		this.obj.transform.localScale = new Vector3 (
			BlockSize,
			BlockSize,
			BlockSize
		);

		var renderer = this.obj.GetComponent<Renderer> ();
		renderer.material.color = color;

		if (color.a < 1.0f) {
			renderer.material.SetOverrideTag ("RenderType", "Transparent");
			renderer.material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
			renderer.material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			renderer.material.SetInt ("_ZWrite", 0);
			renderer.material.DisableKeyword ("_ALPHATEST_ON");
			renderer.material.DisableKeyword ("_ALPHABLEND_ON");
			renderer.material.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
			renderer.material.renderQueue = 3000;
		}
	}
}

public class Map {
	public int?[,] map;
	public int size;
	public int minHeight;
	public int maxHeight;
	public int waterHeight;
	public int resolution;
	public System.Random r;

	public const int RandMax = 1000;

	public Map(int size, int minHeight, int maxHeight, int waterHeight, int resolution) {
		this.size = size;
		this.minHeight = minHeight;
		this.maxHeight = maxHeight;
		this.resolution = resolution;
		this.waterHeight = waterHeight;
		this.r = new System.Random();
	}

	public void Draw(GameObject target) {
		foreach ( Transform c in target.transform ) {
			GameObject.Destroy(c.gameObject);
		}

		var r = new System.Random ();

		for (int x = 0; x < this.size; x += resolution) {
			for (int y = 0; y < this.size; y += resolution) {
				var zMaxHeight = this.map [x, y];

				if (zMaxHeight.HasValue) {
					var zMaxHeightValue = NormalizeHeight(zMaxHeight.Value);

					for (int z = zMaxHeightValue - 1; z <= zMaxHeightValue; z++) { 
						var h = r.Next (50, 150);
						new Block (
							target,
							new Vector3 (NormalizePoint(x), z, NormalizePoint(y)),
							new HSV (h, 150, 255)
						);
					}
					for (int z = zMaxHeightValue + 1; z <= waterHeight; z++) {
						new Block (
							target,
							new Vector3 (NormalizePoint(x), z, NormalizePoint(y)),
							new Color (0f, 0f, 255f, 0.2f)
						);
					}
				}
			}
		}
	}

	public void Generate() {
		map = new int?[size, size];
		var v1 = new Vector2 (0, 0);
		var v2 = new Vector2 (size - 1, size - 1);

		map [(int)v1.x, (int)v1.y] = GenerateHeight();
		map [(int)v1.x, (int)v2.y] = GenerateHeight();
		map [(int)v2.x, (int)v1.y] = GenerateHeight();
		map [(int)v2.x, (int)v2.y] = GenerateHeight();
		GenerateImpl (v1, v2, size);
	}

	public int GenerateHeight() {
		return r.Next (0, RandMax);
	}

	public int NormalizePoint(int value) {
		return value / resolution;
	}

	public int NormalizeHeight(int value) {
		var result = 1f * value;
		result *= 1f * (maxHeight - minHeight) / RandMax;
		result += minHeight;
		return (int)result;
	}

	public int GenerateHeightRand(int size) {
		int value = (int)(1f * RandMax * size / this.size);
		return (int)r.Next (-value / 2, value / 2);
	}

	private void GenerateImpl(Vector2 v1, Vector2 v2, int size) {
		if (size <= 0) return;
		var mid = (v1 + v2) / 2;

		if (!this.map [(int)mid.x, (int)mid.y].HasValue) {
			var value =
				map [(int)v1.x, (int)v1.y].Value +
				map [(int)v1.x, (int)v2.y].Value +
				map [(int)v2.x, (int)v1.y].Value +
				map [(int)v2.x, (int)v2.y].Value;
			value /= 4;
			value += GenerateHeightRand (size);
			if (value > RandMax) value = RandMax;
			if (value < 0) value = 0;
			this.map [(int)mid.x, (int)mid.y] = value;
		}

		if (!map [(int)mid.x, (int)v1.y].HasValue) {
			map [(int)mid.x, (int)v1.y] = (
				map [(int)v1.x, (int)v1.y] +
				map [(int)v2.x, (int)v1.y]
			) / 2;
		}
		if (!map [(int)v2.x, (int)mid.y].HasValue) {
			map [(int)v2.x, (int)mid.y] = (
				map [(int)v2.x, (int)v1.y] +
				map [(int)v2.x, (int)v2.y]
			) / 2;
		}
		if (!map [(int)mid.x, (int)v2.y].HasValue) {
			map [(int)mid.x, (int)v2.y] = (
				map [(int)v1.x, (int)v2.y] +
				map [(int)v2.x, (int)v2.y]
			) / 2;
		}
		if (!map [(int)v1.x, (int)mid.y].HasValue) {
			map [(int)v1.x, (int)mid.y] = (
				map [(int)v1.x, (int)v1.y] +
				map [(int)v1.x, (int)v2.y]
			) / 2;
		}

		size /= 2;
		GenerateImpl (new Vector2 (v1.x, v1.y), new Vector2 (mid.x, mid.y), size);
		GenerateImpl (new Vector2(mid.x, v1.y), new Vector2(v2.x, mid.y), size);
		GenerateImpl (new Vector2(v1.x, mid.y), new Vector2(mid.x, v2.y), size);
		GenerateImpl (new Vector2(mid.x, mid.y), new Vector2(v2.x, v2.y), size);
	}
}

public class TerrainBehaviour : MonoBehaviour {
	public Map map;
	public GameObject player;

	// Use this for initialization
	void Start () {
		map = new Map (size: 2500, resolution: 25, minHeight: 0, maxHeight: 35, waterHeight: 17);
		map.Generate ();
		map.Draw (gameObject);
		player = GameObject.Find ("Player");
		player.transform.position = new Vector3 (30, 100, 30);
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("r")) {
			map.Generate ();
			map.Draw (gameObject);
			player.transform.position = new Vector3 (30, 100, 30);
		}
	}
}