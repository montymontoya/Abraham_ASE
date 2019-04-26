using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Interactable : MonoBehaviour
{
    public EVRButtonId pickupB = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo

    //public ControllerInput controlX;

    public bool hideIt=true;

    public virtual void ButtonPressDown2(EVRButtonId button, ControllerInput controller, ControllerInput controller1)
    {

    }

    public virtual void ButtonPressUp2(EVRButtonId button, ControllerInput controller, ControllerInput controller1)
    {

    }


    public virtual void TriggerEnter(ControllerInput controller)
    {

    }

    public virtual void TriggerStay(ControllerInput controller)
    {

    }


    public virtual void TriggerExit(ControllerInput controller)
    {

    }


    public virtual void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        if (button == pickupB) // Si se presionó el botón de sostener
        {
            HideIt(controller);
        }
    }

    
    public virtual void ButtonPressUp(EVRButtonId button, ControllerInput controller)
    {
        if (button == pickupB) // Si se dejó de presionar el botón de sostener
        {
            ShowIt(controller);
        }
    }


    public virtual void RayEnter(RaycastHit hit, ControllerInput controller = null)
    {

    }


    public virtual void RayStay(RaycastHit hit, ControllerInput controller = null)
    {
  
    }

    public virtual void RayExit(ControllerInput controller = null)
    {
 
    }

    void HideIt(ControllerInput pad)
    {
        var mesh = pad.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in mesh)
        {
            item.enabled = false;
        }
    }

    void ShowIt(ControllerInput pad)
    {
        var mesh = pad.GetComponentsInChildren<MeshRenderer>();
        foreach (var item in mesh)
        {
            item.enabled = true;
        }
    }
}
