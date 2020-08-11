using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointFollowAnimRot : MonoBehaviour
{

    public bool invert;

    public float torqueForce;
    public float angularDamping;
    public float maxForce;
    public float springForce;
    public float springDamping;
    public bool xLock;
    public bool yLock;
    public bool zLock;
    public Vector3 targetVel;

    public Transform target;
    private GameObject limb;
    private JointDrive drive;
    private SoftJointLimitSpring spring;
    private ConfigurableJoint joint;
    private Quaternion startingRotation;
    float origMaximumForce;
    float origDrag;
    Rigidbody rb;
    PlayerController player;

    void Start()
    {
        targetVel = new Vector3(0f, 0f, 0f);
        rb = GetComponent<Rigidbody>();

        drive.positionSpring = torqueForce;
        drive.positionDamper = angularDamping;
        drive.maximumForce = maxForce;

        spring.spring = springForce;
        spring.damper = springDamping;

        joint = gameObject.GetComponent<ConfigurableJoint>();
        player = FindObjectOfType<PlayerController>();

        joint.slerpDrive = drive;

        joint.linearLimitSpring = spring;
        joint.rotationDriveMode = RotationDriveMode.Slerp;
        joint.projectionMode = JointProjectionMode.None;
        joint.targetAngularVelocity = targetVel;
        joint.configuredInWorldSpace = false;
        joint.swapBodies = true;

        origDrag = rb.drag;
        origMaximumForce = drive.maximumForce;
        startingRotation = Quaternion.Inverse(target.localRotation);
    }

    void LateUpdate()
    {
        if(player != null)
        {
            if (rb)
            {
                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;
                drive.maximumForce = 0;
                rb.drag = 0f;
                return;
            }
            else
            {
                if (xLock) joint.angularXMotion = ConfigurableJointMotion.Locked;
                if (yLock) joint.angularYMotion = ConfigurableJointMotion.Locked;
                if (zLock) joint.angularZMotion = ConfigurableJointMotion.Locked;
                drive.maximumForce = origMaximumForce;
                rb.drag = origDrag;
            }
        }
        if (invert)
            joint.targetRotation = Quaternion.Inverse(target.localRotation * startingRotation);
        else
            joint.targetRotation = target.localRotation * startingRotation;
    }


}