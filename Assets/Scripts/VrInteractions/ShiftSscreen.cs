using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class ShiftSscreen : Interactable
{
    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo

    public GameObject nxtScreen;
    public GameObject thisScreen;
    public bool imThePrimChild;
    public GameObject setMeTrueChild;
    /************CON ESTO SE LLAMAN LOS METODOS DE LA INTERACCION DEPENDIENDO DEL BOTON PRESIONADO*************/
    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        thisScreen.SetActive(false);
        nxtScreen.SetActive(true);
        if (imThePrimChild)
        {
            setMeTrueChild.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    public override void ButtonPressUp(EVRButtonId button, ControllerInput controller)
    {

    }

    /***********ESTO SE CARGA AL INICIO O PRIMER FRAME*********/
    void Awake()
    {

    }
    /*************/
}
