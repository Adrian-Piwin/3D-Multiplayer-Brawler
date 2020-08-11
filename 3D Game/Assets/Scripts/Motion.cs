using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motion : MonoBehaviour
{

    private new HingeJoint hingeJoint;
    public Transform myAnim;
    public bool mirror;

    // Start is called before the first frame update
    void Start()
    {
        hingeJoint = GetComponent<HingeJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myAnim != null){
            JointSpring js = hingeJoint.spring;
            js.targetPosition = myAnim.localEulerAngles.x;

            if (js.targetPosition > 180){
                js.targetPosition = js.targetPosition - 360;
            }

            js.targetPosition = Mathf.Clamp(js.targetPosition, hingeJoint.limits.min + 5, hingeJoint.limits.max - 5);

            if (mirror){
                js.targetPosition = js.targetPosition *= -1;
            }

            hingeJoint.spring = js; 
        }
    }
}
