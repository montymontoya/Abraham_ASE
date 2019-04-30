using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class ShiftSscreenCircularMenu : Interactable
{
    public GameObject[] scrns2Hide;

    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo

    public GameObject nxtScreen;

    /************CON ESTO SE LLAMAN LOS METODOS DE LA INTERACCION DEPENDIENDO DEL BOTON PRESIONADO*************/
    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        if (button == pickupButton) // Si se presionó el botón de sostener
        {
            foreach (var item in scrns2Hide)
            {
                item.SetActive(false);
            }
            
            nxtScreen.SetActive(true);
        }
    }
}
