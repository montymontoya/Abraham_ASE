using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//[RequireComponent(typeof(Rigidbody))]

public class Pick_Children : Interactable
{

    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo

    public bool pickable;
    public Transform papi;
    public int fuerza = 1;
    public float breakDistance;
    public bool held;
    public ControllerInput control1;
    public float breakingForce = 1000;
    public float breakingTorque = 1000;
    /************CON ESTO SE LLAMAN LOS METODOS DE LA INTERACCION DEPENDIENDO DEL BOTON PRESIONADO*************/
    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        
        if (button == pickupButton && pickable) // Si se presionó el botón de sostener
        {
            control1 = controller;
            StartCoroutine(LongVibration(1, 3999));

            Pickup(control1); // Se manda a llamar el método de sostener
        }                      // breakDistance = Vector3.Distance(controller.transform.position, this.transform.position);
                               //

    }

    public override void ButtonPressUp(EVRButtonId button, ControllerInput controller)
    {

        if (button == pickupButton && pickable) // Si se dejó de presionar el botón de sostener
        {
            Release(control1); // Se manda a llamar el método de soltar
            control1 = null;
        }
            

    }
    /************************************************************************/

    public bool OriginalKinematic, OriginalGravity;//Variables de respaldo para los estados kinematic y gravedad del objeto a sostener

    /***********ESTO SE CARGA AL INICIO O PRIMER FRAME*********/
    void Awake()
    {
        if (papi == null)
        {
            papi = ParentDetector(this.transform);
        }


        held = false;
        //Invoke("DelayedInvoke", 1.0f);
    }
    /*************/
    private void Update()
    {
        if (!held)
        {
            if (papi.GetComponent<FixedJoint>())
            {
                DestroyFixedJoint(papi);
            }
        }
    }


    public void Pickup(ControllerInput controller) // Cuando se sostiene algo
    {
        if (papi.GetComponent<Rigidbody>())
        {
            OriginalKinematic = papi.GetComponent<Rigidbody>().isKinematic;//se respalda el estado kinematic original
            OriginalGravity = papi.GetComponent<Rigidbody>().useGravity;//se respalda el estado de gravedad original
        }
        else
        {
            papi.gameObject.AddComponent<Rigidbody>();
            OriginalKinematic = papi.GetComponent<Rigidbody>().isKinematic;//se respalda el estado kinematic original
            OriginalGravity = papi.GetComponent<Rigidbody>().useGravity;//se respalda el estado de gravedad original
        }
        StartCoroutine(LongVibration(1, 3999));
        papi.GetComponent<Rigidbody>().isKinematic = false;
        papi.GetComponent<Rigidbody>().useGravity = false;//Se desactiva la gravedad de dicho estado
        AddFixedJoint(papi, controller.transform);// se crea una articulación en el control con el que choca este objeto mediante el metodo AddFixedJoint
        
        held = true;

    }

    /******ESTO DEFINE LOS PARAMETROS DE LA ARTICULACION******/
    private void AddFixedJoint(Transform createJointOn, Transform objToConnect)
    {
        FixedJoint fx = createJointOn.gameObject.AddComponent<FixedJoint>();//Se agrega el componente de articulacion al control
        fx.connectedBody = objToConnect.gameObject.GetComponent<Rigidbody>(); //y se conecta el objeto al control
        fx.breakForce = breakingForce;//Se define una fuerza de quiebre entre el objeto y el control
        fx.breakTorque = breakingTorque;//Se define una fuerza acumulada de ambos.

    }
    /******ESTO ElIMINA LA ARTICULACION******/
    private void DestroyFixedJoint(Transform itemWithFixedJoint)
    {
        itemWithFixedJoint.GetComponent<FixedJoint>().connectedBody = null;
        Destroy(itemWithFixedJoint.GetComponent<FixedJoint>());
    }


    public void Release(ControllerInput controller)
    {

        if (papi.GetComponent<FixedJoint>())
        {
            DestroyFixedJoint(papi);
            papi.GetComponent<Rigidbody>().isKinematic = OriginalKinematic;
            papi.GetComponent<Rigidbody>().useGravity = OriginalGravity;
            papi.GetComponent<Rigidbody>().velocity = controller.device.velocity * fuerza;
            papi.GetComponent<Rigidbody>().angularVelocity = controller.device.angularVelocity * fuerza;

            held = false;
            Debug.Log("Entró el Release");
        }

    }


    IEnumerator LongVibration(float length, ushort strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime * 30)
        {
            control1.device.TriggerHapticPulse(strength, pickupButton);
            yield return null; //every single frame for the duration of "length" you will vibrate at "strength" amount
        }
    }

    /*******ESTO SIRVE PARA DETECTAR AL PADRE DEL NIVEL MÁS ALTO********/
    public Transform ParentDetector(Transform hijo)
    {
        if (hijo.parent != hijo && hijo.parent != null)
        {
            hijo = ParentDetector(hijo.parent);
        }
        return hijo;
    }
}
