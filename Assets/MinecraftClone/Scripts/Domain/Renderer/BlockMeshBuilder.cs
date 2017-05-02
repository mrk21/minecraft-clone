using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MinecraftClone.Infrastructure;

namespace MinecraftClone.Domain.Renderer {
	class BlockMeshBuilder {
		private List<Vector3> vertices;
		private List<int> triangles;
		private List<Vector2> uv;

		private Vector3 position;
		private TextureItem texture;

		public BlockMeshBuilder() {
			this.vertices = new List<Vector3> ();
			this.triangles = new List<int> ();
			this.uv = new List<Vector2> ();
		}

		public BlockMeshBuilder AddBlockMesh(Vector3 position, TextureItem texture) {
			this.position = position;
			this.texture = texture;
			return this;
		}

		public BlockMeshBuilder AddXZPlane()  { return AddTriangles (Triangles.Normal ).AddVertices (Vertices.XZ ).AddUV (UV.Base); }
		public BlockMeshBuilder AddXYPlane()  { return AddTriangles (Triangles.Reverse).AddVertices (Vertices.XY ).AddUV (UV.Base); }
		public BlockMeshBuilder AddYZPlane()  { return AddTriangles (Triangles.Reverse).AddVertices (Vertices.YZ ).AddUV (UV.Base); }
		public BlockMeshBuilder AddXZ2Plane() { return AddTriangles (Triangles.Reverse).AddVertices (Vertices.XZ2).AddUV (UV.Base); }
		public BlockMeshBuilder AddXY2Plane() { return AddTriangles (Triangles.Normal ).AddVertices (Vertices.XY2).AddUV (UV.Base); }
		public BlockMeshBuilder AddYZ2Plane() { return AddTriangles (Triangles.Normal ).AddVertices (Vertices.YZ2).AddUV (UV.Base); }

		public Mesh ToMesh() {
			var mesh = new Mesh();
			mesh.vertices = vertices.ToArray ();
			mesh.triangles = triangles.ToArray ();
			mesh.uv = uv.ToArray ();
			mesh.RecalculateBounds();
			mesh.RecalculateNormals();
			return mesh;
		}

		private static class Vertices {
			public static readonly Vector3[] XZ = new Vector3[] {
				new Vector3(0, 0, 0),
				new Vector3(1, 0, 0),
				new Vector3(1, 0, 1),
				new Vector3(0, 0, 1),
			};
			public static readonly Vector3[] XY  = XZ.Select (v => Quaternion.AngleAxis (-90, Vector3.right  ) * v).ToArray ();
			public static readonly Vector3[] YZ  = XZ.Select (v => Quaternion.AngleAxis ( 90, Vector3.forward) * v).ToArray ();
			public static readonly Vector3[] XZ2 = XZ.Select (v => v + Vector3.up     ).ToArray ();
			public static readonly Vector3[] XY2 = XY.Select (v => v + Vector3.forward).ToArray ();
			public static readonly Vector3[] YZ2 = YZ.Select (v => v + Vector3.right  ).ToArray ();
		}

		private static class Triangles {
			public static readonly int[] Normal = new int[] {
				0, 1, 2,
				0, 2, 3,
			};
			public static readonly int[] Reverse = new int[] {
				0, 2, 1,
				0, 3, 2,
			};
		}

		private static class UV {
			public static readonly Vector2[] Base = new Vector2[] {
				new Vector2 (0, 0),
				new Vector2 (0, 1),
				new Vector2 (1, 1),
				new Vector2 (1, 0),
			};
		}

		private BlockMeshBuilder AddTriangles(int[] value) {
			var c = vertices.Count;
			triangles.AddRange (value.Select (v => v + c).ToArray ());
			return this;
		}

		private BlockMeshBuilder AddVertices(Vector3[] value) {
			vertices.AddRange (value.Select (v => v + position).ToArray ());
			return this;
		}

		private BlockMeshBuilder AddUV(Vector2[] value) {
			uv.AddRange (value.Select(v => (v + texture.Offset) * texture.Scale).ToArray ());
			return this;
		}
	}
}
