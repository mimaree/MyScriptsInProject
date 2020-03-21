using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GamepadAnalogMover : MonoBehaviour
{
    public bool cursorisvisible;
    private Transform playerTransform;
    private Transform directionObject;
    private Transform camTransform;
    private Vector3 move;
    private Vector3 camForward;
    private Vector3 finalVector;
    private Vector3 camF;
    private Vector3 camR;
    private float magnitudeM;
    private float magnitude = 5f;
    private bool isMove;
    private GameObject createDrictionObject;
    private bool isOnWall = false;

    public void SetIsOnWall(bool isOnWall) {
        this.isOnWall = isOnWall;
    }

    public bool GetIsMove() {
        return isMove;
    }

    public Transform GetDirectionObject() {
        return directionObject;
    }

    public float GetMagintudeObjectMove() {
        return Mathf.Clamp(magnitudeM, 0, 1.0f);
    }

    private void Awake() {
        playerTransform = GameObject.FindWithTag("Player").transform; // Strzelam ze bedziesz mial inna nazwe u siebie xd             
        try {
            directionObject = GameObject.Find("/MoveCube").transform;
            InputManager.target = directionObject.gameObject;
        } catch (System.Exception) {
            createDrictionObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            createDrictionObject.name = "MoveCube";
            BoxCollider boxCollider = createDrictionObject.GetComponent<BoxCollider>();
            MeshRenderer meshRenderer = createDrictionObject.GetComponent<MeshRenderer>();
            // meshRenderer.enabled = false; 
            boxCollider.enabled = false;
            directionObject = createDrictionObject.transform;
            InputManager.target = createDrictionObject;
        }
        camTransform = GameObject.FindWithTag("MainCamera").transform; // Tu u ciebie pewnie samo /Main Camera  Pozdrawiam cieplutko
    }
    private void Update() {
        move = InputManager.MainJoystick();

        if (move == Vector3.zero) // new vector(0,0,0)
        {
            isMove = false;
        } else {
            isMove = true;
        }

        magnitudeM = move.magnitude;
        InputManager.analogStrength = Mathf.Clamp(magnitudeM, 0, 1.0f);
        move = move.normalized * magnitude;


        if (isOnWall) {
            CamRotarion();
            finalVector = (playerTransform.up * move.y + playerTransform.right * move.x);

            directionObject.position = playerTransform.position - finalVector;
            // CameraForWardDebug();
            CursorVisible();

        } else {
            CamRotarion();

            finalVector = (camF * move.y + camR * move.x);

            directionObject.position = playerTransform.position - finalVector;
            directionObject.position = new Vector3(directionObject.position.x
                                                      , playerTransform.position.y
                                                      , directionObject.position.z);
            // CameraForWardDebug();
            CursorVisible(); // To do usunieca raczej albo gdies indziej dac
        }
    }

    private void CameraForWardDebug() {
        move = new Vector3(move.x, 0, move.y);
        camForward = playerTransform.position - camTransform.position;
        camForward = new Vector3(camForward.x, 0, camForward.z);
        Debug.DrawRay(camTransform.position, camForward, Color.yellow);
        Debug.Log(camTransform.position);
        Debug.DrawRay(playerTransform.position, move, Color.red);
        Debug.DrawRay(playerTransform.position, finalVector, Color.blue);
        Debug.Log(isMove);
        Debug.Log(magnitudeM + "//n" + move);
    }

    private void CamRotarion() {
        camF = camTransform.forward;
        camR = camTransform.right;

        camF.y = 0;
        camR.y = 0;

        camF = camF.normalized;
        camR = camR.normalized;
    }

    private void CursorVisible() {
        if (cursorisvisible) {
            Cursor.lockState = CursorLockMode.Confined;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = cursorisvisible;

    }
}