using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MinecraftClone.Application.Behaviour
{
    class TargetMarkerBehaviour : MonoBehaviour
    {
        public PlayerBehaviour player;

        private MeshFilter meshFilter;
        private Mesh xMesh;
        private Mesh yMesh;
        private Mesh zMesh;

        void Start()
        {
            meshFilter = GetComponent<MeshFilter>();

            xMesh = CreatePlaneMesh(new Vector3[]{
                new Vector3(0f, -0.5f, -0.5f),
                new Vector3(0f, 0.5f, -0.5f),
                new Vector3(0f, -0.5f, 0.5f),
                new Vector3(0f, 0.5f, 0.5f)
            });
            yMesh = CreatePlaneMesh(new Vector3[]{
                new Vector3(-0.5f, 0f, -0.5f),
                new Vector3( 0.5f, 0f, -0.5f),
                new Vector3(-0.5f, 0f,  0.5f),
                new Vector3( 0.5f, 0f,  0.5f)
            });
            zMesh = CreatePlaneMesh(new Vector3[]{
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3( 0.5f, -0.5f, 0),
                new Vector3(-0.5f,  0.5f, 0),
                new Vector3( 0.5f,  0.5f, 0)
            });

            // SetTargetMarker
            Observable
                .EveryUpdate()
                .Where(_ => EnabledOperation())
                .ThrottleFirst(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => SetTargetMarker())
                .AddTo(gameObject);
        }

        private void SetTargetMarker()
        {
            var distance = 10f;
            Ray ray = GetRay();

            if (Physics.Raycast(ray, out RaycastHit hit, distance))
            {
                float[] x = { (float)Math.Floor(hit.point.x), (float)Math.Ceiling(hit.point.x) };
                float[] y = { (float)Math.Floor(hit.point.y), (float)Math.Ceiling(hit.point.y) };
                float[] z = { (float)Math.Floor(hit.point.z), (float)Math.Ceiling(hit.point.z) };

                if (Math.Abs(hit.normal.x) == 1)
                {
                    var newPosition = new Vector3(
                        x[0],
                        (y[0] + y[1]) / 2f,
                        (z[0] + z[1]) / 2f
                    );
                    newPosition += 0.01f * hit.normal;
                    transform.position = newPosition;
                    meshFilter.mesh = xMesh;
                }
                else if (Math.Abs(hit.normal.y) == 1)
                {
                    var newPosition = new Vector3(
                        (x[0] + x[1]) / 2f,
                        y[0],
                        (z[0] + z[1]) / 2f
                    );
                    newPosition += 0.01f * hit.normal;
                    transform.position = newPosition;
                    meshFilter.mesh = yMesh;
                }
                else if (Math.Abs(hit.normal.z) == 1)
                {
                    var newPosition = new Vector3(
                        (x[0] + x[1]) / 2f,
                        (y[0] + y[1]) / 2f,
                        z[0]
                    );
                    newPosition += 0.01f * hit.normal;
                    transform.position = newPosition;
                    meshFilter.mesh = zMesh;
                }
            }
            else
            {
                meshFilter.mesh = null;
            }
        }

        private Ray GetRay()
        {
            return player.head.mainCamera.ScreenPointToRay(Input.mousePosition);

        }

        private Mesh CreatePlaneMesh(Vector3[] face)
        {
            var mesh = new Mesh();

            List<Vector3> vertextList = new List<Vector3>{
                face[0],
                face[1],
                face[2],
                face[3]
            };

            List<Vector2> uvList = new List<Vector2>{
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            List<int> indexList = new List<int>();
            indexList.AddRange(new[] { 0, 1, 3, 2, 0 });

            mesh.SetVertices(vertextList);
            mesh.SetUVs(0, uvList);
            mesh.SetIndices(indexList.ToArray(), MeshTopology.LineStrip, 0);

            return mesh;
        }

        bool EnabledOperation()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

    }
}