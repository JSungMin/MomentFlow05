using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicLight : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;
    private Vector3[] vertices;
    private MyQuad myQuad;

    private int rayNum;

    private void Awake()
    {
        rayNum = 50;
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        myQuad = new MyQuad();
    }

    private void Start()
    {
        // Vector3.zero가 worldPosition과 같다
        // 시계방향으로 돌아가는 상황에서 노멀이 사용자를 향한다
        vertices = new Vector3[rayNum + 1];
        float perRadian = (2 * Mathf.PI) / (float)rayNum;
        float theta = 0.0f;
        float radius = 1.0f;
        vertices[0] = Vector3.zero;
        for(int i = 1; i<= rayNum; i++)
        {
            vertices[i] = new Vector3(Mathf.Cos(theta) * radius, -Mathf.Sin(theta) * radius, 0.0f);
            theta += perRadian;
        }
        
        mesh = meshFilter.mesh;
        mesh.Clear();

        // mesh filter의 mesh를 가져온 후 vertices을 할당한다
        mesh.vertices = vertices;
        // indices 배열과 같다
        int[] triangles = new int[rayNum * 3];
        for (int i = 0, j = 0; j < rayNum; i+= 3, j++)
        {
            triangles[i] = 0;
            triangles[i + 1] = j + 1;
            if (j == rayNum - 1)
                triangles[i + 2] = 1;
            else
                triangles[i + 2] = j + 2;
        }

        mesh.triangles = triangles;
    }
}


public class MyRay
{
    Vector3 origin;
    Vector3 direction;
    float length;

    private bool IsIntersect(MyQuad myQuad)
    {
        if (direction.y == 0.0f || direction.x == 0.0f)
            return false;

        MyRay[] quadOutLine;
        quadOutLine = new MyRay[4];

        Vector3 myQuadPosition = myQuad.gameObject.transform.position;

        // 아래
        quadOutLine[0].origin = new Vector3(myQuadPosition.x - 0.5f * myQuad.width, myQuadPosition.x - 0.5f * myQuad.height, 0.0f);
        quadOutLine[0].direction = Vector3.right;
        quadOutLine[0].length = myQuad.width;

        // 위
        quadOutLine[1].origin = new Vector3(myQuadPosition.x - 0.5f * myQuad.width, myQuadPosition.y + 0.5f * myQuad.height, 0.0f);
        quadOutLine[1].direction = Vector3.right;
        quadOutLine[1].length = myQuad.width;

        // 왼쪽
        quadOutLine[2].origin = new Vector3(myQuadPosition.x - 0.5f * myQuad.width, myQuadPosition.y - 0.5f * myQuad.height, 0.0f);
        quadOutLine[2].direction = Vector3.up;
        quadOutLine[2].length = myQuad.height;

        // 오른쪽
        quadOutLine[3].origin = new Vector3(myQuadPosition.x + 0.5f * myQuad.width, myQuadPosition.y - 0.5f * myQuad.height, 0.0f);
        quadOutLine[3].direction = Vector3.up;
        quadOutLine[3].length = myQuad.height;

        

        return true;
    }

    public Vector3 findTangentPoint(MyQuad myQuad)
    {
        if (IsIntersect(myQuad))
            return Vector3.zero;
        else
            return origin + direction * length;
    }
}