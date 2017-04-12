using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLight : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        vertices = new Vector3[3];
        vertices[0] = transform.position;
        vertices[1] = transform.position + Vector3.right;
        vertices[2] = transform.position + Vector3.up;
        mesh = meshFilter.mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.normals = new Vector3[] { new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f) };
    }
}
