using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//[RequireComponent(typeof(Rigidbody))]

public class Pick_n_Throw : Interactable
{

    public EVRButtonId pickupButton = EVRButtonId.k_EButton_SteamVR_Trigger;//Botón predefinido para sostener algo es el gatillo

    public bool pickable;
    //throwable;
    public int fuerza = 1;
    //public float breakDistance=;
    public float breakingForce = 100000;
    public float breakingTorque = 100000;
    public bool held;
    public ControllerInput control1;
    private Mesh meshDelMeshCollider;

    public bool OriginalKinematic, OriginalGravity;//Variables de respaldo para los estados kinematic y gravedad del objeto a sostener
    private float OriginalWeight;

    /************CON ESTO SE LLAMAN LOS METODOS DE LA INTERACCION DEPENDIENDO DEL BOTON PRESIONADO*************/
    public override void ButtonPressDown(EVRButtonId button, ControllerInput controller)
    {
        
        if (pickable && !held)
        {
            if (button == pickupButton) // Si se presionó el botón de sostener
            {
                if (hideIt)
                {
                    base.ButtonPressDown(button, controller);
                }
                

                control1 = controller;
                Pickup(control1); // Se manda a llamar el método de sostener
                StartCoroutine(LongVibration(1, 3999));
            }
        }                 // breakDistance = Vector3.Distance(controller.transform.position, this.transform.position);
    }

    public override void ButtonPressUp(EVRButtonId button, ControllerInput controller)
    {
        if (pickable && held)
        {
            if (button == pickupButton) // Si se dejó de presionar el botón de sostener
            {
                if (hideIt)
                {
                    base.ButtonPressUp(button, controller);
                }

                Release(control1); // Se manda a llamar el método de soltar
                control1 = null;
            }
        }
    }

    /***********ESTO SE CARGA AL INICIO O PRIMER FRAME*********/
    void Awake()
    {
        held = false;

    }
    /*************/

    public void Pickup(ControllerInput controller) // Cuando se sostiene algo
    {
        if (controller.transform.GetComponent<MeshCollider>())
        {
            meshDelMeshCollider = controller.transform.GetComponent<MeshCollider>().sharedMesh;
            Destroy(controller.transform.gameObject.GetComponent<MeshCollider>());
        }
        
        if (GetComponent<Rigidbody>())
        {
            if (!held)
            {
                OriginalKinematic = GetComponent<Rigidbody>().isKinematic;//se respalda el estado kinematic original
                OriginalGravity = GetComponent<Rigidbody>().useGravity;//se respalda el estado de gravedad original
                OriginalWeight = GetComponent<Rigidbody>().mass;//se respalda el peso original
            }
        }
        else
        {
            this.gameObject.AddComponent<Rigidbody>();
            OriginalKinematic = GetComponent<Rigidbody>().isKinematic;//se respalda el estado kinematic original
            OriginalGravity = GetComponent<Rigidbody>().useGravity;//se respalda el estado de gravedad original
            OriginalWeight = 1;
        }

        GetComponent<Rigidbody>().isKinematic = false;
        //GetComponent<Rigidbody>().useGravity = false;//Se desactiva la gravedad de dicho estado
        GetComponent<Rigidbody>().mass = 0.1f; // se le asigna una masa de 1
        AddFixedJoint(this.transform,controller.transform);// se crea una articulación en el control con el que choca este objeto mediante el metodo AddFixedJoint
        
        held = true;
    }
    private void Update()
    {
        if (!held)
        {
            if (this.GetComponent<FixedJoint>())
            {
                DestroyFixedJoint(this.transform);
            }
        }   
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

    private void OnJointBreak(float breakforce)
    {

        Release(control1);
    }

    public void Release(ControllerInput controller)
    {

        if (this.GetComponent<FixedJoint>())
        {
            DestroyFixedJoint(this.transform);
            GetComponent<Rigidbody>().isKinematic = OriginalKinematic;
            GetComponent<Rigidbody>().useGravity = OriginalGravity;
            GetComponent<Rigidbody>().mass = OriginalWeight;
            GetComponent<Rigidbody>().velocity = controller.device.velocity * fuerza;
            GetComponent<Rigidbody>().angularVelocity = controller.device.angularVelocity * fuerza;   
            held = false;

            if (controller.transform.GetComponent<MeshCollider>())
            {
                controller.gameObject.GetComponent<MeshCollider>().convex = true;
                controller.gameObject.GetComponent<MeshCollider>().isTrigger = true;
            }
            else
            {
                if (meshDelMeshCollider)
                {
                    Debug.Log("TENIA MESH?");
                    controller.gameObject.AddComponent<MeshCollider>().sharedMesh = meshDelMeshCollider;
                    controller.gameObject.GetComponent<MeshCollider>().convex = true;
                    controller.gameObject.GetComponent<MeshCollider>().isTrigger = true;
                }
                
            }
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
}
