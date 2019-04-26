using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class Raycast : MonoBehaviour
{

    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private string selectableCTag = "SelectableC";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Transform ParentObjetos;

    private Transform _selection;
    private Transform _selectionC;
    private GameObject grabObj;
    private GameObject grabCanv;

    public float rayLenght;
    public LayerMask layerMask;

    private EventSystem EventSystem;
    
    private void Update()
    {    

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if(_selection != null)
        {

            var selectionRenderer = _selection.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            _selection = null;

        }

        if (_selectionC != null)
        {

            _selectionC = null;

        }

        if(Physics.Raycast(ray, out hit))
        {

            var selection = hit.transform;
            var selectionC = hit.transform;

            if (selection.CompareTag(selectableTag))
            {

                var selectionRenderer = selection.GetComponent<Renderer>();

                if (selectionRenderer != null)
                {

                    selectionRenderer.material = highlightMaterial;

                }

                _selection = selection;

            }

            if (selectionC.CompareTag(selectableCTag))
            {

                _selectionC = selectionC;

            }

        }

        if (Input.GetButtonDown("X Button") && !EventSystem.current.IsPointerOverGameObject( ))
        {

            if (Physics.Raycast(ray, out hit, rayLenght, layerMask))
            {

                if (hit.transform.GetComponent<EventTrigger>())
                {

                    PointerEventData pointerData = new PointerEventData(EventSystem);
                    pointerData.button = PointerEventData.InputButton.Left;

                    ExecuteEvents.ExecuteHierarchy(hit.transform.gameObject, pointerData, ExecuteEvents.pointerDownHandler);
                    ExecuteEvents.ExecuteHierarchy(hit.transform.gameObject, pointerData, ExecuteEvents.pointerClickHandler);

                }

            }

        }

        if (Input.GetAxis("RightTrigger") > 0) /*&& !EventSystem.current.IsPointerOverGameObject()*/
        {

            if(_selectionC != null && grabCanv == null)
            {

                grabCanv = _selectionC.gameObject;
                grabCanv.transform.SetParent(transform);

            }

            if (_selection != null && grabObj == null)
            {
                grabObj = _selection.gameObject;
                grabObj.gameObject.GetComponent<Rigidbody>().useGravity = false;
                grabObj.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                grabObj.transform.SetParent(transform);


            }
        }
        else
        { 
            if(grabCanv != null)
            {

                grabCanv.transform.SetParent(null);
                grabCanv = null;

            }
            if(grabObj != null)
            {

                grabObj.gameObject.GetComponent<Rigidbody>().useGravity = true;
                grabObj.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                grabObj.transform.SetParent(null);
                grabObj = null;


            }

        }

    }

}
