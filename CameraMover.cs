using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    public Transform player;
    public float speed = 1;
    public Transform lockedTarget;
    public bool isLocked;
    public bool forcenUnlock;
    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindWithTag("Player").transform;
    }

    bool LookForTarget() {
        return false;
    }

    void AssignTarget() {
        //todo better
        if(!LookForTarget()) {
            lockedTarget.position = new Vector3(transform.forward.x, player.position.y + 2.0f, transform.forward.z * 5.0f);
        } 
    }

    // Update is called once per frame
    void Update() {
        transform.position = player.position + new Vector3(0, 5, 0);
        if (Input.GetKeyDown(KeyCode.Y))
            isLocked = !isLocked;
        if (forcenUnlock) 
            isLocked = false;

        if (isLocked) {
            if (lockedTarget = null) {
                AssignTarget();
            }
            transform.LookAt(lockedTarget);
        } else {
            transform.eulerAngles += new Vector3(-speed * Input.GetAxis("Mouse Y"), speed * Input.GetAxis("Mouse X"), 0.0f);
            lockedTarget = null;
        }
    }
}
