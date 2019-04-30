using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class MoveCursorLens : Interactable
{
    public Transform lupa;
    public float deg;
    //public float frstDeg;
    // Start is called before the first frame update
    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo


    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        if (button == pickupButton) // Si se presionó el botón de sostener
        {

            lupa.localEulerAngles=new Vector3(0,0,deg);
        }
    }
}
