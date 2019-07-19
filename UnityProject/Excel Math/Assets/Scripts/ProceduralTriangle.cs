using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[ExecuteAlways]
public class ProceduralTriangle : MonoBehaviour {

    public Vector3[] vertices;

    private Mesh mesh;

    void OnRenderObject() {
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices;
        mesh.triangles = new int[3] { 0, 1, 2 };
    }
}
