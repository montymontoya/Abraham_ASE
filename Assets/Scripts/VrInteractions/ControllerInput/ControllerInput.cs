using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using System.Linq;

public class ControllerInput : MonoBehaviour
{
    /************************************************************************/
    private bool originalRigidbody;//Variables de respaldo para los estados kinematic y gravedad del objeto a interactuar
    /************************************************************************/
    private Material material;
    public Material outline;
    public Material[] materiales;
    public GameObject modelo;
    public MeshRenderer[] Renderer;
    //public GameObject lastObj;
    protected Dictionary<EVRButtonId, List<Interactable>> scriptButtons; // Se crea un diccionario de la lista de los scripts que usan botones
    protected SteamVR_TrackedObject trackedObj; // variable para referencia del objeto rastreado (control)

    public Stack ColliderStack = new Stack();

    public SteamVR_Controller.Device device // acceso al objeto rastreado
    {
        get
        {
            return SteamVR_Controller.Input((int)trackedObj.index);
        }
    }

    public delegate void TouchpadPress();
    public static event TouchpadPress OnTouchpadPress;
    protected List<EVRButtonId> buttonsTracked;// lista de los botones del control rastreado

    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>(); // se establece en la variable el objeto rastreado
        scriptButtons = new Dictionary<EVRButtonId, List<Interactable>>(); // se define en la variable el diccionario de scripts que usan los botones
        buttonsTracked = new List<EVRButtonId>() // se define la lista de los botones a rastrear
        {
            EVRButtonId.k_EButton_SteamVR_Trigger, // gatillo
            EVRButtonId.k_EButton_Grip
        };

        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        ColliderStack.Push(this.gameObject);
        Invoke("DelayedInvoke", 1.0f); // obtenemos los materiales de los hijos de este objeto
    }

    void OnTriggerEnter(Collider collider) // al entrar a un objeto
    {
        var papi = InteractableDetector(collider.transform); // detectamos al objeto (sea el padre o no) que tiene algun script interactable 
        if (papi.gameObject.GetComponent<Interactable>())// si el objeto detectado (si no tiene interactable será el último) tiene un Interactable
        {
            if (papi.gameObject != (GameObject)ColliderStack.Peek())//nos aseguramos que este objeto no este ya en la pila de objetos con Interactable
            {
                ColliderStack.Push(papi.gameObject);//si no está, lo metemos a la pila
                Renderer[0].materials = new Material[2]; // creamos un nuevo MeshRender con 2 materiales
                Renderer[0].materials = materiales;//a estos nuevos materiales se les asigna el valor del material original del control y el del outliner
                Interactable[] interactables = papi.gameObject.GetComponents<Interactable>(); // se accede al script o los scripts de tipo Interactable dentro del objeto en colision
                Recorrer(interactables);
            }
        }
    }

    /***************OBTENER MATERIAL DEL CONTROL*************/
    void DelayedInvoke()
    {
        Renderer = modelo.GetComponentsInChildren<MeshRenderer>();
        material = Renderer[0].material;
        materiales = new Material[2];
        materiales[0] = material;
        materiales[1] = outline;
    }

    void OnTriggerStay(Collider collider) // mientras se mantiene el control dentro de un objeto
    {
        var papi = InteractableDetector(collider.transform);// detectamos al objeto (sea el padre o no) que tiene algun script interactable 
        if (papi.gameObject == (GameObject)ColliderStack.Peek())//nos aseguramos que este objeto esté ya en la pila de objetos con Interactable (y que sea el primero en la pila)
        {
            Renderer[0].materials = new Material[2]; // creamos un nuevo MeshRender con 2 materiales
            Renderer[0].materials = materiales;//a estos nuevos materiales se les asigna el valor del material original del control y el del outliner
            Interactable[] interactables = papi.gameObject.GetComponents<Interactable>(); // se accede al script o los scripts de tipo Interactable dentro del objeto en colision
            Recorrer(interactables);
        }
    }

    void Recorrer(Interactable[] interactables)
    {
        /* * * * * * * * SE RECORRE PARA CADA SCRIPT DENTRO DEL OBJETO * * * * * * */
        for (int i = 0; i < interactables.Length; i++) //
        {
            Interactable interactable = interactables[i]; // se asigna uno solo de ellos en cada iteracion
            for (int b = 0; b < buttonsTracked.Count; b++) // se recorren los botones rastreados (3 solamente)
            {
                EVRButtonId button = buttonsTracked[b]; // se guarda en la variable button al boton rastreado

                if (device.GetPressDown(button)) // si se está presionando ese boton
                {
                    if (!scriptButtons.ContainsKey(button) || !scriptButtons[button].Contains(interactable))
                    // si el boton no es una clave del diccionario aun o 
                    // si el boton si está en el diccionario pero no tiene asigando ese script actual
                    {
                        interactable.ButtonPressDown(button, this); // se envía la información del boton presionado al script actual
                        if (!scriptButtons.ContainsKey(button)) // Si es la primera vez que se usa el boton
                            scriptButtons.Add(button, new List<Interactable>()); // se agrega dicho boton a la lista de botones utilizado
                        scriptButtons[button].Add(interactable); // se agrega el script a la lista de scripts que usa ese boton
                    }
                }
            }
        }
    }
    void Update()
    {
        EVRButtonId[] pressKeys = scriptButtons.Keys.ToArray(); // se guarda en la variable pressKeys los botones en uso del diccionario de scripts 

        for (int i = 0; i < pressKeys.Length; i++) // se recorren los botones
        {
            if (device.GetPressUp(pressKeys[i])) // si se soltó ese boton
            {
                //Invoke("DelayedRemat", 0.1f);
                List<Interactable> releaseObjects = scriptButtons[pressKeys[i]];

                for (int j = 0; j < releaseObjects.Count; j++) // se recorre la lista de los scritps que usan botones
                {
                    releaseObjects[j].ButtonPressUp(pressKeys[i], this); // se envía al script el boton liberado
                }

                scriptButtons[pressKeys[i]].Clear(); // se limpia el script actual respecto al boton liberado
            }
        }
    }
    void OnTriggerExit(Collider collider)
    {
        var papi = InteractableDetector(collider.transform);
        if (papi.gameObject.GetComponent<Interactable>())
        {
            if (papi.gameObject == (GameObject)ColliderStack.Peek())
            {
                ColliderStack.Pop();
                Invoke("DelayedRemat", 0.1f);
            }
        }
    }
    /*******ESTO SIRVE PARA DETECTAR AL PADRE DEL NIVEL MÁS ALTO********/
    public Transform InteractableDetector(Transform hijo)
    {
        if (!hijo.GetComponent<Interactable>())
        {
            if (hijo.parent != null)
            {
                hijo = InteractableDetector(hijo.parent);
            }
        }
        return hijo;
    }

    /**********************************************************/

    void DelayedRemat()
    {
        Renderer[0].materials = new Material[1];
        Renderer[0].material = material;
    }

    

}
