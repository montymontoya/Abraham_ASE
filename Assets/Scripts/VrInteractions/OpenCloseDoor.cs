using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class OpenCloseDoor : Interactable
{
    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo

    public Transform bisagra;
    public float openSpeed = 1;
    public float maxOpenAngle = 90;
    public float minOpenAngle = 0;
    public bool flag;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var y =Mathf.Abs(transform.localEulerAngles.y);
        if (flag)
        { 
            if (y<maxOpenAngle)
                transform.localEulerAngles -= new Vector3(0, openSpeed * Time.deltaTime, 0);
        }
        else
        {
            if (y > minOpenAngle)
                transform.localEulerAngles += new Vector3(0, openSpeed * Time.deltaTime, 0);
        }
    }

    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        flag = !flag;

    }
}
