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
    private Vector3             handStartManipulateVector;

    private AudioSource         audioSource;

    [SerializeField]
    private AudioClip           handDetectedClip;
    private AudioClip           handLostClip;



    void GestureEvent_Tapped(TappedEventArgs args) {
        if(args.tapCount == 1) {
            Debug.Log("Tapped");
            if (selectedObject) {
                selectedObject.GetComponent<Renderer>().material.color = Color.white;
            }

            if (gazedObject) {
                gazedObject.GetComponent<Renderer>().material.color = Color.green;
            }

            selectedObject = gazedObject;
        }
        else if (args.tapCount == 2) {
            Debug.Log("Double Tapped");
            if (selectedObject) {
                Transform transform = selectedObject.GetComponent<Transform>();
                transform.position = new Vector3(0, 0, 0);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
        }
    }


    void GestureEvent_HoldStarted(HoldStartedEventArgs args) {

    }

    void GestureEvent_HoldCompleted(HoldCompletedEventArgs args) {

    }

    private void GestureEvent_ManipulationUpdated(ManipulationUpdatedEventArgs args) {
        Debug.Log("Manipulation Updated");
        Transform transform = selectedObject.GetComponent<Transform>();

        if (leftHandState == eHandState.None && rightHandState == eHandState.Ready && selectedObject != null) {
            Vector3 deltaPosition = handPositions[1] - handPositionsLastFrame[1];
            transform.position += deltaPosition;
        }
        else if (leftHandState == eHandState.Ready && rightHandState == eHandState.Ready && selectedObject != null) {
            Vector3 handsVectorLastFrame = handPositionsLastFrame[1] - handPositionsLastFrame[0];
            Vector3 handsVectorThisFrame = handPositions[1] - handPositions[0];

            float vectorLengthLastFrame = handsVectorLastFrame.magnitude;
            float vectorLengthThisFrame = handsVectorThisFrame.magnitude;

            float dot = Vector3.Dot(handsVectorThisFrame.normalized, handStartManipulateVector.normalized);
            // Is scaling
            if (dot > 0.99f) {
                float dotWithUp = Vector3.Dot(handStartManipulateVector.normalized, Vector3.up);
                // Is scaling vertically
                if(dotWithUp > 0.3f || dotWithUp < -0.3f) {
                    float deltaHeight = vectorLengthThisFrame - vectorLengthLastFrame;
                    transform.localScale += new Vector3(0.0f, deltaHeight, 0.0f);
                }
                // Is scaling horizontally
                else {
                    float deltaWidth = vectorLengthThisFrame - vectorLengthLastFrame;
                    transform.localScale += new Vector3(deltaWidth, 0.0f, deltaWidth);
                }
            }
            // Is rotating
            else {
                Vector3 rotationAxis = Vector3.Cross(handStartManipulateVector, handsVectorThisFrame).normalized;
                transform.Rotate(rotationAxis, Vector3.Dot(handsVectorThisFrame.normalized, handsVectorLastFrame.normalized), Space.World);
            }
        }
    }

    private void GestureEvent_ManipulationStarted(ManipulationStartedEventArgs args) {
        Debug.Log("Manipulation Started");
        if (leftHandState == eHandState.Ready && rightHandState == eHandState.Ready && selectedObject != null) {
            handStartManipulateVector = handPositions[1] - handPositions[0];
        }
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
