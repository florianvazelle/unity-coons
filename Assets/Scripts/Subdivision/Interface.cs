using System;
using System.Collections.Generic;
using UnityEngine;
using RapidGUI;
using static InterfaceUtils;


namespace Subdivision
{
    public class Interface : MonoBehaviour
    {

        private const String MODEL_NAME = "Building";

        public GameObject cube, cubeHole, camel, cyl1, cyl2, icos, tet;

        private GameObject? actualPrefab, actualGo;
        private int subDivision;
        private int level, levelOld;
        private Dictionary<string, GameObject> prefabs;

        // Catmull-Clark
        private MeshConverter converter;
        private Subdivision.Core.CatmullClarkSubdivider CCSubdivider;
        private Subdivision.Core.Shape shape;

        void Start()
        {
            subDivision = 1;
            level = levelOld = 0;
            actualPrefab = actualGo = null;

            prefabs = new Dictionary<string, GameObject>(){
                {"Cube", cube},
                {"Cube Hole", cubeHole},
                {"Camel", camel},
                {"Cylindre 1", cyl1},
                {"Cylindre 2", cyl2},
                {"Icos", icos},
                {"Tet", tet},
            };

            // Catmull-Clark
            converter = new MeshConverter();
            CCSubdivider = new Subdivision.Core.CatmullClarkSubdivider();  // Instantier CatmullClark, mais pas encore executer
        }

        private void OnGUI()
        {
            if (transform.parent == null)
            {
                DoGUI();
            }
        }

        public void DoGUI()
        {
            subDivision = RGUI.Slider(subDivision, 1, 6, "Subdivision");

            using (new GUILayout.HorizontalScope())
            {
                foreach (var kvp in prefabs)
                    if (GUILayout.Button(kvp.Key))
                    {
                        GameObject prefab = GameObject.Find(MODEL_NAME);
                        if (prefab != null)
                        {
                            Destroy(prefab);
                        }

                        GameObject go = Instantiate(kvp.Value, new  Vector3(0, 0, 0), Quaternion.identity);
                        go.name = MODEL_NAME;

                        actualPrefab = kvp.Value;

                        // Catmull-Clark
                        actualGo = go;
                        MeshFilter viewedModelFilter = (MeshFilter)actualGo.GetComponent("MeshFilter");
                        shape = converter.OnConvert(viewedModelFilter.mesh); // Convertir le mesh passer en parametre en Shape
                    }
            }

            // Catmull-Clark
            //A chaque fois qu'on click sur le boutton: il prend le shape actuelle est le resubdivise
            if (GUILayout.Button("Catmull-Clark")) {
                if (actualGo != null)
                {
                    shape = CCSubdivider.Subdivide(shape); // Cette fois ci on recupere le Shape obtenir et le subdivise

                    MeshFilter viewedModelFilter = (MeshFilter)actualGo.GetComponent("MeshFilter");
                    viewedModelFilter.mesh = converter.ConvertToMesh(shape); //A la fin on recupere la nouvelle Shape obtenu apres la subdivision. Pour pouvoir réitérer si on le souhaite.
                }
            }

            level = RGUI.Slider(level, 0, 3, "Kobbelt");
            if (level != levelOld || GUILayout.Button("Update"))
            {
                if (actualPrefab != null)
                {
                    MeshFilter viewedModelFilter = (MeshFilter)actualPrefab.GetComponent("MeshFilter");
                    Mesh mesh = viewedModelFilter.sharedMesh;

                    List<Triangle> triangles = new List<Triangle>();
                    for (int i = 0; i < mesh.triangles.Length; i += 3)
                    {
                        int iA = mesh.triangles[i];
                        int iB = mesh.triangles[i + 1];
                        int iC = mesh.triangles[i + 2];

                        Vector3 vA = mesh.vertices[iA];
                        Vector3 vB = mesh.vertices[iB];
                        Vector3 vC = mesh.vertices[iC];

                        triangles.Add(new Triangle(vA, vB, vC));
                    }

                    for (int i = 0; i < subDivision; i++)
                    {
                        Kobbelt.Subdivision(ref triangles, level);
                    }

                    GenerateMeshIndirect(in triangles);

                    levelOld = level;
                }
            }
        }

        static public void GenerateMeshIndirect(in List<Triangle> triangles)
        {
            var vCenter = findCenter(triangles);

            List<Vector3> points3D = new List<Vector3>();
            List<int> indices = new List<int>();

            for (int i = 0; i < triangles.Count; i++)
            {
                List<int> idx = new List<int>();
                for (var j = 0; j < 3; j++)
                {
                    points3D.Add(triangles[i].vertices[j]);
                    indices.Add(i * 3 + j);
                }

                var surfaceNormal = Vector3.Cross(triangles[i].vertices[1] - triangles[i].vertices[0], triangles[i].vertices[2] - triangles[i].vertices[0]).normalized;
                Vector3 center = triangles[i].Center();
                if (Vector3.Dot(surfaceNormal, center - vCenter) < 0)
                {
                    idx.Reverse();
                }

                indices.AddRange(idx);
            }

            GenerateMesh(points3D, indices);
        }

        // https://forum.unity.com/threads/building-mesh-from-polygon.484305/
        static public void GenerateMesh(List<Vector3> vertices, List<int> indices)
        {

            GameObject thisBuilding = GameObject.Find(MODEL_NAME);
            if (thisBuilding == null)
            {
                // Create a building game object
                thisBuilding = new GameObject(MODEL_NAME);
            }

            var center = findCenter(vertices);
            var normals = new List<Vector3>();

            for (int i = 0; i < vertices.Count; i++)
            {
                normals.Add((vertices[i] - center).normalized);
            }

            MeshFilter mf = thisBuilding.GetComponent<MeshFilter>();
            if (mf == null)
            {
                // Create and apply the mesh
                mf = thisBuilding.AddComponent<MeshFilter>();
            }

            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(indices, 0);

            // mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.Optimize();

            MeshRenderer rend = thisBuilding.GetComponent<MeshRenderer>();
            if (rend == null)
            {
                rend = thisBuilding.AddComponent<MeshRenderer>();
            }
            rend.material = new Material(Shader.Find("VR/SpatialMapping/Wireframe"));
        }

        public static Vector3 findCenter(List<Triangle> triangles)
        {
            Vector3 center = Vector3.zero;
            // Only need to check every other spot since the odd indexed vertices are in the air, but have same XZ as previous
            for (int i = 0; i < triangles.Count; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    center += triangles[i].vertices[j];
                }
            }
            return center / (triangles.Count * 3);
        }

        public static Vector3 findCenter(List<Vector3> verts)
        {
            Vector3 center = Vector3.zero;
            // Only need to check every other spot since the odd indexed vertices are in the air, but have same XZ as previous
            for (int i = 0; i < verts.Count; i++)
            {
                center += verts[i];
            }
            return center / verts.Count;
        }
    }
}