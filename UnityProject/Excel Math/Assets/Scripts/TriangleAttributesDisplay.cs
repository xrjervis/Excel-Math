using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleAttributesDisplay : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

        Transform cube1 = transform.GetChild(0);
        Transform cube2 = transform.GetChild(1);
        Transform cube3 = transform.GetChild(2);

        List<Vector3> vertsInModelSpace = new List<Vector3>();
        vertsInModelSpace.Add(transform.InverseTransformPoint(cube1.position));
        vertsInModelSpace.Add(transform.InverseTransformPoint(cube2.position));
        vertsInModelSpace.Add(transform.InverseTransformPoint(cube3.position));

        GetComponent<MeshFilter>().mesh.SetVertices(vertsInModelSpace);

        SphereCollider collider = GetComponent<SphereCollider>();
        collider.center = (vertsInModelSpace[0] + vertsInModelSpace[1] + vertsInModelSpace[2]) / 3;
        collider.radius = Mathf.Min((vertsInModelSpace[0] - collider.center).magnitude, (vertsInModelSpace[1] - collider.center).magnitude, (vertsInModelSpace[2] - collider.center).magnitude) - 0.15f;
    }
}
