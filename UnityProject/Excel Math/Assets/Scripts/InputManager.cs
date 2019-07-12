using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;

enum eHandState {
    None,
    Ready
};

public class InputManager : MonoBehaviour {
    public GameObject           gazeCursor;

    [SerializeField]
    private GameObject          gazedObject;

    [SerializeField]
    private GameObject          selectedObject;

    [SerializeField]
    private eHandState          leftHandState;

    [SerializeField]
    private eHandState          rightHandState;

    private GestureRecognizer   recognizer;
    private Vector3[]           handPositions = new Vector3[2];
    private Vector3[]           handPositionsLastFrame = new Vector3[2];

    private AudioSource         audioSource;

    [SerializeField]
    private AudioClip           handDetectedClip;
    private AudioClip           handLostClip;

    void GestureEvent_Tapped(TappedEventArgs args) {
        Debug.Log("Tapped");
        if (selectedObject) {
            selectedObject.GetComponent<Renderer>().material.color = Color.white;
        }

        if (gazedObject) {
            gazedObject.GetComponent<Renderer>().material.color = Color.green;
        }

        selectedObject = gazedObject;
    }

    void GestureEvent_HoldStarted(HoldStartedEventArgs args) {

    }

    void GestureEvent_HoldCompleted(HoldCompletedEventArgs args) {

    }

    private void GestureEvent_ManipulationUpdated(ManipulationUpdatedEventArgs args) {
        Debug.Log("Manipulation Updated");
        if (leftHandState == eHandState.None && rightHandState == eHandState.Ready && selectedObject != null) {
            Transform transform = selectedObject.GetComponent<Transform>();
            Vector3 deltaPosition = handPositions[1] - handPositionsLastFrame[1];
            Debug.Log(deltaPosition);
            transform.position += deltaPosition;
        }
    }

    private void GestureEvent_ManipulationStarted(ManipulationStartedEventArgs args) {
        Debug.Log("Manipulation Started");
    }

    private void GestureEvent_ManipulationCompleted(ManipulationCompletedEventArgs args) {
        Debug.Log("Manipulation Completed");
    }

    private void GestureEvent_NavigationUpdated(NavigationUpdatedEventArgs args) {
        Debug.Log("Navigation Updated");

    }

    private void GestureEvent_NavigationStarted(NavigationStartedEventArgs args) {
        Debug.Log("Navigation Started");
    }

    private void GestureEvent_NavigationCompleted(NavigationCompletedEventArgs args) {
        Debug.Log("Navigation Completed");
    }

    private void InteractionSourceUpdated(InteractionSourceUpdatedEventArgs args) {
        Vector3 pos;
        handPositionsLastFrame[0] = handPositions[0];
        handPositionsLastFrame[1] = handPositions[1];
        if (args.state.sourcePose.TryGetPosition(out pos)) {
            if (args.state.source.handedness == InteractionSourceHandedness.Left) {
                handPositions[0] = pos;
            }
            else if (args.state.source.handedness == InteractionSourceHandedness.Right) {
                handPositions[1] = pos;
            }
        }
    }

    private void InteractionSourceDetected(InteractionSourceDetectedEventArgs args) {
        audioSource.PlayOneShot(handDetectedClip);
        if(args.state.source.handedness == InteractionSourceHandedness.Left) {
            leftHandState = eHandState.Ready;
        }
        else if (args.state.source.handedness == InteractionSourceHandedness.Right) {
            rightHandState = eHandState.Ready;
        }
    }

    private void InteractionSourceLost(InteractionSourceLostEventArgs args) {
        audioSource.PlayOneShot(handLostClip);
        if (args.state.source.handedness == InteractionSourceHandedness.Left) {
            leftHandState = eHandState.None;
        }
        else if (args.state.source.handedness == InteractionSourceHandedness.Right) {
            rightHandState = eHandState.None;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(handPositions[0], 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(handPositions[1], 0.2f);
    }

    void Awake() {
        InteractionManager.InteractionSourceUpdated += InteractionSourceUpdated;
        InteractionManager.InteractionSourceDetected += InteractionSourceDetected;
        InteractionManager.InteractionSourceLost += InteractionSourceLost;

        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold | GestureSettings.DoubleTap | GestureSettings.ManipulationTranslate);

        recognizer.Tapped += GestureEvent_Tapped;
        recognizer.HoldStarted += GestureEvent_HoldStarted;
        recognizer.HoldCompleted += GestureEvent_HoldCompleted;
        recognizer.ManipulationStarted += GestureEvent_ManipulationStarted;
        recognizer.ManipulationUpdated += GestureEvent_ManipulationUpdated;
        recognizer.ManipulationCompleted += GestureEvent_ManipulationCompleted;
        recognizer.NavigationStarted += GestureEvent_NavigationStarted;
        recognizer.NavigationUpdated += GestureEvent_NavigationUpdated;
        recognizer.NavigationCompleted += GestureEvent_NavigationCompleted;

        recognizer.StartCapturingGestures();

        audioSource = GetComponent<AudioSource>();
        handDetectedClip = Resources.Load<AudioClip>("Audio/Click");
        handLostClip = Resources.Load<AudioClip>("Audio/Error");
    }

    void Start() {
        gazeCursor = Instantiate(gazeCursor, new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Update() {
        Vector3 headPosition = this.transform.position;
        Vector3 gazeDirection = this.transform.forward;
        Debug.DrawRay(headPosition, gazeDirection, Color.red);

        RaycastHit hitResult;

        if (Physics.Raycast(headPosition, gazeDirection, out hitResult)) {
            gazedObject = hitResult.collider.gameObject;
            gazeCursor.GetComponentInChildren<MeshRenderer>().enabled = true;
            gazeCursor.GetComponent<Transform>().position = hitResult.point;
            gazeCursor.GetComponent<Transform>().rotation = Quaternion.FromToRotation(Vector3.up, hitResult.normal);
        }
        else {
            gazedObject = null;
            gazeCursor.GetComponentInChildren<MeshRenderer>().enabled = false;
        }

    }
}
