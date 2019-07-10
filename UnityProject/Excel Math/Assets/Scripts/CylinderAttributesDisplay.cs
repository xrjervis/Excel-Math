using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CylinderAttributesDisplay : MonoBehaviour {
    public double radius = 5.0;
    public double height = 10.0;

    [SerializeField]
    private double volume;

    private GameObject canvas;
    private Text volumeText;
    
    // Start is called before the first frame update
    void Start() {
        canvas = transform.Find("Canvas").gameObject;
        volumeText = canvas.transform.Find("VolumeText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        volume = Mathf.PI * radius * radius * height;
        volumeText.text = "Volume: " + volume;
    }

}
