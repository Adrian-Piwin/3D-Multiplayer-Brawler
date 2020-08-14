using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    public bool isRagdoll;

    public GameObject root;

    private List<HingeJointSettings> hingeJointList;

    private List<bool> ragdollPart;

    // Start is called before the first frame update
    void Start()
    {
        hingeJointList = new List<HingeJointSettings>();
        ragdollPart = new List<bool>();
        toggleRagdoll(false);
    }

    public void toggleRagdoll(bool toggle){
        if (!toggle){
            disableRagdoll(root.transform);
        }else{
            enableRagdoll(root.transform, 0, 0);
            hingeJointList.Clear();
            ragdollPart.Clear();
        }
    }

    public void disableRagdoll(Transform root) {
        foreach (Transform child in root) {
            HingeJoint childhj = child.gameObject.GetComponent<HingeJoint>();
            if (childhj != null){
                HingeJointSettings hingeJointSettings;
                hingeJointSettings.limits = childhj.limits;
                hingeJointSettings.spring = childhj.spring;
                hingeJointSettings.connectedBody = childhj.connectedBody;
                hingeJointList.Add(hingeJointSettings);
                Destroy(childhj);
            }

            Rigidbody childRb = child.gameObject.GetComponent<Rigidbody>();
            if (childRb!= null){
                Destroy(childRb);
            }

            if (childRb != null && childhj != null)
                ragdollPart.Add(true);
            else ragdollPart.Add(false);

            disableRagdoll(child);
        }
    }

    public void enableRagdoll(Transform root, int index, int jointIndex) {
        foreach (Transform child in root) {
            if (ragdollPart[index]){
                Rigidbody rb = child.gameObject.AddComponent<Rigidbody>() as Rigidbody;

                HingeJoint childhj = child.gameObject.AddComponent<HingeJoint>() as HingeJoint;
                childhj.limits = hingeJointList[jointIndex].limits;
                childhj.spring = hingeJointList[jointIndex].spring;
                childhj.connectedBody = hingeJointList[jointIndex].connectedBody;

                jointIndex = jointIndex + 1;
            }

            index += 1;
            
            enableRagdoll(child, index, jointIndex);
        }
    }

    struct HingeJointSettings{
        public JointLimits limits;
        public JointSpring spring;
        public Rigidbody connectedBody;
    }

}
