using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
public class DoSelection : Interactable
{
    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo

    public bool flag;
    public SpriteRenderer mySprtRenderer;
    public Color32 color,slctdColor,nrmlColor;
    public void Start()
    {
        mySprtRenderer = GetComponent<SpriteRenderer>();

        color = Color.white;
        //mySprtRenderer.color = Color.white;
    }

    public void Update()
    {
        slctdColor = new Color32(254, 74, 70, 212);
        nrmlColor = new Color32(255, 255, 255, 255);

        if (flag)
            color = slctdColor;
        else
            color = nrmlColor;

        mySprtRenderer.color = color;
    }
    /************CON ESTO SE LLAMAN LOS METODOS DE LA INTERACCION DEPENDIENDO DEL BOTON PRESIONADO*************/
    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        flag = !flag;

    }


}
