using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollison : MonoBehaviour
{
    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10.0f;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;
    LayerMask collisionLayer = ~4096;
    public float smoothNew = 0.05f;
    public float adjustmentDistance = -8f;
    public bool colliding = false;
    public Vector3[] adjustedCameraClipPoints;
    Camera camera;
    private Transform followObj;
    public Transform playarTransform;
    public void SetMaxDistance(float max)
    {
        maxDistance = max;
    }

    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    public float GetDistance()
    {
        return distance;
    }

    public void Initialize(Camera cam)
    {
        followObj = transform.parent;
        camera = cam;
        adjustedCameraClipPoints = new Vector3[5];
    
    }

    public void Start()
    {
        Initialize(GetComponent<Camera>());
    }

    public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
    {
        if (!camera)
            return;

        //cleat the cotents of inirArray
        intoArray = new Vector3[5];

        float z = camera.nearClipPlane;
        float x = Mathf.Tan(camera.fieldOfView / 3.41f) * z;
        float y = x / camera.aspect;

        // top left
        intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;  //added and rotated the point relative to camera
        // top right
         intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;  //added and rotated the point relative to camera
        //bottomleft
         intoArray[2] = (atRotation * new Vector3(-x,-y, z)) + cameraPosition;  //added and rotated the point relative to camera
        //botto, righy
         intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;  //added and rotated the point relative to camera
        //camera's psootion
        var camF = camera.transform.forward;
        intoArray[4] = cameraPosition - camera.transform.forward; 
    }
  
    public void FixedUpdate()
    {
        var start = transform.parent.position;
        UpdateCameraClipPoints(transform.position, transform.parent.rotation, ref adjustedCameraClipPoints);
        //dar deug lines
        for (int i = 0; i < 5; i++)
        {
           // Debug.DrawLine(followObj.position, adjustedCameraClipPoints[i], Color.green);
        }
        
       distance = DistanceCalculate(followObj.position);

        transform.localPosition = Vector3.Slerp(transform.localPosition,
                                              dollyDir * distance,
                                              Time.deltaTime * smooth);
    }

    private void OnDrawGizmosSelected()
    {
       // Gizmos.color = Color.red;
       // Gizmos.DrawWireSphere(transform.position, .1f);
    }

    public float DistanceCalculate(Vector3 from) //form nie wiem z kotrej strony
    {
        float distance = maxDistance;
        float[] disnaceTab = new float[5];
        for (int i = 0; i < adjustedCameraClipPoints.Length; i++)
        {
            Ray ray = new Ray(from, adjustedCameraClipPoints[i] - from);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDistance, collisionLayer))
            {
                if(hit.distance < distance)
                {
                    distance = hit.distance;
                }
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
                disnaceTab[i] = hit.distance;
             //   Debug.DrawLine(followObj.position, hit.point, Color.red);
            }
            else
            {
                disnaceTab[i] = maxDistance;
            }
        }

        return distance;    
    }
}
