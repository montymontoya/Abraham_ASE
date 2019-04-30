using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class OpenCloseScreen : Interactable
{
    public GameObject screen;
    public bool open;
    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo


    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        if (button == pickupButton) // Si se presionó el botón de sostener
        {
            if (open)
            {
                screen.SetActive(true);
            }
            else
            {
                screen.SetActive(false);
            }
        }
    }



}
