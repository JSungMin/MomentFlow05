using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyQuad : MonoBehaviour
{
    public float width;
    public float height;
    public float rotateAngleZ;

    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;
   
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
    }

    private void Start()
    {
        vertices = new Vector3[4];
        vertices[0] = new Vector3(- 0.5f * width, - 0.5f * height, 0.0f);
        vertices[1] = new Vector3(- 0.5f * width, + 0.5f * height, 0.0f);
        vertices[2] = new Vector3(+ 0.5f * width, + 0.5f * height, 0.0f);
        vertices[3] = new Vector3(+ 0.5f * width, - 0.5f * height, 0.0f);

        mesh = meshFilter.mesh;
        mesh.Clear();

        mesh.vertices = vertices;
        // indices 배열과 같다
        int[] triangles = new int[6];

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.triangles = triangles;
    }
}