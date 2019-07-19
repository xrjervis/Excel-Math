using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleAttributesDisplay : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Vector3 vert1 = transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[0]);
        Vector3 vert2 = transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[1]);
        Vector3 vert3 = transform.TransformPoint(GetComponent<MeshFilter>().mesh.vertices[2]);

        Transform cube1 = transform.GetChild(0);
        Transform cube2 = transform.GetChild(1);
        Transform cube3 = transform.GetChild(2);

        cube1.position = vert1;
        cube2.position = vert2;
        cube3.position = vert3;
    }
}
