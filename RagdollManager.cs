using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    private Collider mainCollider;
    private Collider[] allColliders;
    private Animator animation;
    private Rigidbody[] allRigs;
    private Transform playerTransform;
    private Transform rigTransform;
    private bool isSetVelocityToRagdoll;
    
    private Rigidbody mainRigs;
    public RagdollManager(GameObject gameObject)
    {
        mainCollider = gameObject.GetComponent<Collider>();
        allColliders = gameObject.GetComponentsInChildren<Collider>(true);
        animation = gameObject.GetComponent<Animator>();
        mainRigs = gameObject.GetComponent<Rigidbody>();
        allRigs = gameObject.GetComponentsInChildren<Rigidbody>(true);
        playerTransform = gameObject.transform;
        rigTransform = GameObject.Find("hiro_v4_MESH (2)/rig_hiro/root/ORG-hips/ORG-spine").transform ; //GameObject.FindGameObjectsWithTag("Player").ToString
    }
   
   public void DoRagdoll(bool isRagdoll)
    {
        foreach (var col in allColliders)
        {
            col.enabled = isRagdoll;
            
        }
        foreach (var rig in allRigs)
        {
          
            rig.useGravity = isRagdoll;
        }

        mainCollider.enabled = !isRagdoll;
        mainRigs.useGravity = !isRagdoll;
        animation.enabled = !isRagdoll;
    }
    
    public void SetTransform()
    {
        //  mainRigs.velocity = Vector3.zero;
        if (isSetVelocityToRagdoll)
        {
            foreach (var rig in allRigs)
            {
                rig.velocity = mainRigs.velocity;
            }
            isSetVelocityToRagdoll = false;
        }
      
        mainRigs.position = rigTransform.position;
    }

    public void SetSIsSetVelocityToRagdoll(bool isSetVelocityToRagdoll)
    {
        this.isSetVelocityToRagdoll = isSetVelocityToRagdoll;
    }
}
