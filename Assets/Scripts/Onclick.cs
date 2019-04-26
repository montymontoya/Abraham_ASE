using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Onclick : MonoBehaviour {

    //put script on an empty GameObject, 
    public Canvas pantalla01;
    public Canvas pantalla02;

    private bool PantallaActiva = false;
    private bool PantallaOcutla = true;

    void Update()
    {

        if(Input.GetKeyDown("X Button"))
        {

            PantallaActiva = !PantallaActiva;

        }

    }

}
