using UnityEngine;
using UnityEditor;
using System.Collections;

public class CreateCone : ScriptableWizard {

    public int numVertices = 10;
    public float radiusTop = 0f;
    public float radiusBottom = 4.63f;
    public float length = 10.91f;
    public float openingAngle = 0f;
    public bool outside = true;
    public bool inside = false;
    public bool addCollider = false;

    [MenuItem("GameObject/Create Other/Cone")]
    static void CreateWizard()
    {
        DisplayWizard("Create Cone", typeof(CreateCone));
    }

	void OnWizardCreate()
    {
        GameObject newCone = new GameObject("Cone");
        if (openingAngle > 0f && openingAngle < 180f)
        {
            radiusTop = 0f;
            radiusBottom = length = Mathf.Tan(openingAngle * Mathf.Deg2Rad / 2);
        }
        string meshName = newCone.name + numVertices + "v" + radiusTop + "t" + radiusBottom + "b" + length + "l" + length + (outside ? "o" : "") + (inside ? "i" : "");
        string meshPrefabPath = "Assets/Editor/" + meshName + ".asset";
        Mesh mesh = AssetDatabase.LoadAssetAtPath(meshPrefabPath, typeof(Mesh)) as Mesh;

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = meshName;

            int multiplier = (outside ? 1 : 0) + (inside ? 1 : 0);
            int offset = outside && inside ? 2 * numVertices : 0;

            Vector3[] vertices = new Vector3[2 * multiplier * numVertices];
            Vector3[] normals = new Vector3[2 * multiplier * numVertices];
            Vector2[] uvs = new Vector2[2 * multiplier * numVertices];
            
            float slope = Mathf.Atan((radiusBottom - radiusTop) / length);
            float slopeSin = Mathf.Sin(slope);
            float slopeCos = Mathf.Cos(slope);

            int i;

            for (i = 0; i < numVertices; i++)
            {
                float angle = 2 * Mathf.PI * i / numVertices;
                float angleSin = Mathf.Sin(angle);
                float angleCos = Mathf.Cos(angle);
                float angleHalf = 2 * Mathf.PI * (i + 0.5f) / numVertices;
                float angleHalfSin = Mathf.Sin(angleHalf);
                float angleHalfCos = Mathf.Cos(angleHalf);

                vertices[i] = new Vector3(radiusTop * angleCos, radiusTop * angleSin, 0f);
                vertices[i + numVertices] = new Vector3(radiusBottom * angleCos, radiusBottom * angleSin, length);

                if (radiusTop == 0f)
                {
                    normals[i] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                }
                else
                {
                    normals[i] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);
                }

                if (radiusBottom == 0f)
                {
                    normals[i + numVertices] = new Vector3(angleHalfCos * slopeCos, angleHalfSin * slopeCos, -slopeSin);
                }
                else
                {
                    normals[i + numVertices] = new Vector3(angleCos * slopeCos, angleSin * slopeCos, -slopeSin);
                }

                uvs[i] = new Vector2(1f * i / numVertices, 1);
                uvs[i + numVertices] = new Vector2(1f * i / numVertices, 0);

                if (outside && inside)
                {
                    vertices[i + 2 * numVertices] = vertices[i];
                    vertices[i + 3 * numVertices] = vertices[i + numVertices];
                    uvs[i + 2 * numVertices] = vertices[i];
                    uvs[i + 3 * numVertices] = vertices[i + numVertices];
                }

                if (inside)
                {
                    normals[i + offset] = -normals[i];
                    normals[i + numVertices + offset] = -normals[i + numVertices];
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;

            int[] tris;
            int cnt = 0;
            if (radiusTop == 0f)
            {
                tris = new int[numVertices * 3 * multiplier];
                if (outside)
                {
                    for (i = 0; i < numVertices; i++)
                    {
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = i;
                        if (i == numVertices - 1)
                        {
                            tris[cnt++] = numVertices;
                        }
                        else
                        {
                            tris[cnt++] = i + 1 + numVertices;
                        }
                    }
                }
                if (inside)
                {
                    for (i = offset; i < numVertices + offset; i++)
                    {
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = i;
                        if (i == numVertices - 1 + offset)
                        {
                            tris[cnt++] = numVertices + offset;
                        }
                        else
                        {
                            tris[cnt++] = i + 1 + numVertices;
                        }
                    }
                }
            }
            else if (radiusBottom == 0f)
            {
                tris = new int[numVertices * 3 * multiplier];
                if (outside)
                {
                    for (i = 0; i < numVertices; i++)
                    {
                        tris[cnt++] = i;
                        if (i == numVertices - 1)
                        {
                            tris[cnt++] = 0;
                        }
                        else
                        {
                            tris[cnt++] = i + 1;
                        }
                        tris[cnt++] = i + numVertices;
                    }
                }
                if (inside)
                {
                    for (i = offset; i < numVertices + offset; i++)
                    {
                        if (i == numVertices - 1 + offset)
                        {
                            tris[cnt++] = offset;
                        }
                        else
                        {
                            tris[cnt++] = i + 1;
                        }
                        tris[cnt++] = i;
                        tris[cnt++] = i + numVertices;
                    }
                }
            }
            else
            {
                tris = new int[numVertices * 6 * multiplier];
                if (outside)
                {
                    for (i = 0; i < numVertices; i++)
                    {
                        int ip1 = i + 1;
                        if (ip1 == numVertices)
                        {
                            ip1 = 0;
                        }
                        tris[cnt++] = i;
                        tris[cnt++] = ip1;
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = ip1 + numVertices;
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = ip1;
                    }
                }
                if (inside)
                {
                    for (i = offset; i < numVertices + offset; i++)
                    {
                        int ip1 = i + 1;
                        if (ip1 == numVertices + offset)
                        {
                            ip1 = offset;
                        }
                        tris[cnt++] = i;
                        tris[cnt++] = ip1;
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = ip1 + numVertices;
                        tris[cnt++] = i + numVertices;
                        tris[cnt++] = ip1;
                    }
                }
            }
            mesh.triangles = tris;
            AssetDatabase.CreateAsset(mesh, meshPrefabPath);
            AssetDatabase.SaveAssets();
        }
        MeshFilter mf = newCone.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        newCone.AddComponent<MeshRenderer>();

        if (addCollider)
        {
            MeshCollider mc = newCone.AddComponent<MeshCollider>();
            mc.sharedMesh = mf.sharedMesh;
        }
        Selection.activeObject = newCone;
    }
}
