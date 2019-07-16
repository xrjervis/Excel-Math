using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CylinderAttributesDisplay : MonoBehaviour {
    [HideInInspector]
    private double radius;
    [HideInInspector]
    private double height;
    [HideInInspector]
    private double volume;

    public Canvas canvas;
    public Text displayText;
    
    // Start is called before the first frame update
    void Start() {
       
    }

    // Update is called once per frame
    void Update() {
        radius = 5.0 * transform.localScale.x; //or z
        height = 10.0 * transform.localScale.y;
        volume = Mathf.PI * radius * radius * height;

        // Set text position and orientation
        Transform cube1 = transform.GetChild(0);
        Transform cube2 = transform.GetChild(1);
        if(cube1.position.x > cube2.position.x) {
            canvas.transform.position = this.transform.position + Camera.main.transform.right * (Mathf.Abs(cube1.position.x - this.transform.position.x) + 0.2f);
        }
        else {
            canvas.transform.position = this.transform.position + Camera.main.transform.right * (Mathf.Abs(cube2.position.x - this.transform.position.x) + 0.2f);
        }

        // 3D text's orientation is backwards
        canvas.transform.LookAt(2 * canvas.transform.position - Camera.main.transform.position);

        displayText.text = "Radius: " + radius.ToString("n2") + "\n" + "Height: " + height.ToString("n2") + "\n" + "Volume: " + volume.ToString("n2");
    }

}
