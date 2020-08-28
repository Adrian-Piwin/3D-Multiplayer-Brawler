using UnityEngine;
using System.Collections;

public class HingeJointTarget : MonoBehaviour {

    public HingeJoint hj;
    public Transform target;
    [Tooltip("Only use one of these values at a time. Toggle invert if the rotation is backwards.")]
    public bool x, y, z, invert, offsetPositive;
    private float xoffset, yoffset, zoffset;

	void Start ()
    {   
        xoffset = target.localEulerAngles.x;
        yoffset = target.localEulerAngles.y;
        zoffset = target.localEulerAngles.z;
	}
	
	void Update ()
    {
        if (hj != null)
        {
            if (x)
            {
                JointSpring js;
                js = hj.spring;

                if (offsetPositive)
                    js.targetPosition = target.transform.localEulerAngles.x - xoffset;
                else
                    js.targetPosition = xoffset - target.transform.localEulerAngles.x;

                if (js.targetPosition > 180)
                    js.targetPosition = js.targetPosition - 360;
                if (invert)
                    js.targetPosition = js.targetPosition * -1;

                js.targetPosition = Mathf.Clamp(js.targetPosition, hj.limits.min + 1, hj.limits.max - 1);

                hj.spring = js;
            }
            else if (y)
            {
                JointSpring js;
                js = hj.spring;

                if (offsetPositive)
                    js.targetPosition = target.transform.localEulerAngles.y - yoffset;
                else
                    js.targetPosition = yoffset - target.transform.localEulerAngles.y;

                if (js.targetPosition > 180)
                    js.targetPosition = js.targetPosition - 360;
                if (invert)
                    js.targetPosition = js.targetPosition * -1;

                js.targetPosition = Mathf.Clamp(js.targetPosition, hj.limits.min + 1, hj.limits.max - 1);

                hj.spring = js;
            }
            else if (z)
            {
                JointSpring js;
                js = hj.spring;

                if (offsetPositive)
                    js.targetPosition = target.transform.localEulerAngles.z - zoffset;
                else
                    js.targetPosition = zoffset - target.transform.localEulerAngles.z;

                if (js.targetPosition > 180)
                    js.targetPosition = js.targetPosition - 360;
                if (invert)
                    js.targetPosition = js.targetPosition * -1;

                js.targetPosition = Mathf.Clamp(js.targetPosition, hj.limits.min + 1, hj.limits.max - 1);

                hj.spring = js;
            }
        }
    }
}
