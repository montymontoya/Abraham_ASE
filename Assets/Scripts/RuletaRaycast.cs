using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RuletaRaycast : MonoBehaviour
{

    public float rayLenght;
    public LayerMask layerMask;

    float SpeedZ;
    float Tiempo;

    public Canvas[] PantallasRuleta;

    public GameObject Imagen01;
    public GameObject Imagen02;
    public GameObject Imagen03;
    public GameObject Imagen04;

    public GameObject RuletaLupa;

    // Start is called before the first frame update
    void Start()
    {

        SpeedZ = 0;

    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(0, 0, SpeedZ);

        if (Input.GetButtonDown("X Button") && !EventSystem.current.IsPointerOverGameObject( ))
        {

            RaycastHit hit;
            Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, rayLenght, layerMask))
            {

                SpeedZ = 1f;
                Invoke("prueba", 3);

            }
        }
    }

    void prueba()
    {

        SpeedZ = 0;

    }
}
