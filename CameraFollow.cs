using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float cameraMoveSpeed = 120.0f;
    public GameObject CameraFollowObj;
    public GameObject CameraFollowObjOnHit;
    Vector3 FollowPOS;
    public float clampAngle = 80.0f;
    public float inputSensitivity = 150.0f;
    public GameObject cameraObj;
    public GameObject playerObj;
    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistanceZToPlayer;
    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputZ;
    public float smoothX;
    public float smoothy;
    private float rotY = 0.0f;
    private float rotX = 0.0f;

    float sphereRange;

    GameObject mainCamera;
    CameraCollison cameraCollision;
    SphereCaster sphereCaster;
    BlackStripes blackStripes;
    private int x = 0;
    private int xx = 0;

    Transform targetHit;


    SimpleTouchController simpleTouchControllerCam;

    // Start is called before the first frame update
    void Start()
    {
        simpleTouchControllerCam = GameObject.Find("/Canvas/SimpleTouch JoystickCam").GetComponent<SimpleTouchController>();
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        // wylacza widok kursora
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        blackStripes = (new GameObject("blackStripes")).AddComponent<BlackStripes>();
        mainCamera = GameObject.Find("CameraHolder/Main Camera");
        Debug.Log(mainCamera.name + " = main camera name");

        cameraCollision = GameObject.Find("CameraHolder/Main Camera").GetComponent<CameraCollison>();
        sphereCaster = GameObject.Find("CameraHolder/Main Camera").GetComponent<SphereCaster>();
    }

    void Update()
    {
        switch (x)
        {
            case 0:
                CameraFollowTarget();
                if (Input.GetKeyDown("x"))
                {
                    Debug.Log("PressSpace");
                    x = 1;
                    xx = 1;
                    blackStripes.SlerpStart(150,40);
                }
                    break;
            case 1:
                if (Input.GetKeyDown("x"))
                {
                    Debug.Log("PressSpace");
                    x = 0;
                    xx = 0;
                    blackStripes.SlerpStart(0, -40);     
                }
                break;

        }
      
    }

    private void LateUpdate()
    {
        switch (xx)
        {
        case 0:
                sphereRange = 0;
                CameraUpdaterTarget();
                sphereCaster.SetMaxDistance(cameraCollision.GetDistance());
                sphereCaster.SetSphereRadius(sphereRange);
                break;
        case 1:
                CameraUpdaterTarget();
                sphereRange += Time.deltaTime * 22;          
                    if(sphereCaster.GetCurrentHitObject() != null)
                    {
                        Debug.Log("trafilo");
                        Debug.Log(sphereCaster.GetCurrentHitObject().name);
                        CameraFollowObjOnHit = sphereCaster.GetCurrentHitObject();
                    
                        xx = 3;
                      
                    }
                    sphereCaster.SetMaxDistance(cameraCollision.GetDistance() + sphereRange);
                    sphereCaster.SetSphereRadius(sphereRange);
                break;
        case 3:
                CameraUpdaterTargetOnHit(CameraFollowObjOnHit);
                break;
        }
       
    }

    void CameraUpdaterTarget()
    {
        Transform target = CameraFollowObj.transform;
        float step = cameraMoveSpeed * Time.deltaTime;
      transform.position = Vector3.MoveTowards(transform.position
                                              ,target.position 
                                              ,step);
    }
    void CameraUpdaterTargetOnHit(GameObject hitObject)
    {
        Transform target = hitObject.transform;
        float step = cameraMoveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards( transform.position 
                                                , playerObj.transform.position + Vector3.up * 2
                                                , step);

        transform.LookAt(hitObject.transform);
    }

    void CameraFollowTarget()
    {
        float inputX = Input.GetAxis("RightStickHorizontal");
        float inputZ = Input.GetAxis("RightStickVertical");

        inputX += InputManager.Get_Right_Stick_Horizontal();
        inputZ += InputManager.Get_Right_Stick_Vertical();

        // simpleTouchControllerCam
        inputX += simpleTouchControllerCam.GetTouchPosition.x;
        inputZ += simpleTouchControllerCam.GetTouchPosition.y;

        mouseX = 0; //Input.GetAxis("Mouse X");
        mouseY = 0;//Input.GetAxis("Mouse Y");
        finalInputX = inputX + mouseX;
        finalInputZ = inputZ + mouseY;

        rotY += finalInputX * inputSensitivity * Time.deltaTime;
        rotX += finalInputZ * inputSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -80f, 60f);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation
                                            , localRotation
                                            , Time.deltaTime * 8f);

    }
}
