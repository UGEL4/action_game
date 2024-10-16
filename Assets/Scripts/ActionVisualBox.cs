using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ActionVisualBox : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private LineRenderer lineRenderer;

    private MeshFilter meshFilter;
 
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        //meshFilter.mesh = GetComponent<MeshFilter>().mesh;
 
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false; // 禁用MeshRenderer渲染
 
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
        lineRenderer.SetVertexCount(24);
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
 
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            lineRenderer.SetPosition(i, vertices[i]);
        }
 
        int[] triangles = meshFilter.mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            lineRenderer.SetPosition(i + 2, vertices[triangles[i]]);
            lineRenderer.SetPosition(i + 1, vertices[triangles[i + 1]]);
            lineRenderer.SetPosition(i + 0, vertices[triangles[i + 2]]);
        }
    }

    void Update()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            lineRenderer.SetPosition(i, vertices[i]);
        }
 
        int[] triangles = meshFilter.mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            lineRenderer.SetPosition(i + 2, vertices[triangles[i]]);
            lineRenderer.SetPosition(i + 1, vertices[triangles[i + 1]]);
            lineRenderer.SetPosition(i + 0, vertices[triangles[i + 2]]);
        }
    }
}
