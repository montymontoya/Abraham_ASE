﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerRayInput : MonoBehaviour
{
    protected GameObject hitObject;
    protected RaycastHit rayHit;
    protected Interactable hitObjectObjectInteractable;
    protected ControllerInput controllerInput;

    void Awake()
    {
        //If attached to head, will return null (expected behavior)
        //I'm using this method to differentiate between head and hands
        controllerInput = GetComponent< ControllerInput>();
    }

    void Update()
    {
        //Check if raycast hits anything
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit))
        {
            
            //If the object is the same as the one we hit last frame
            if (hitObject != null && hitObject == rayHit.transform.gameObject)
            {
                //Trigger "Stay" method on Interactable every frame we hit
                RayStay(rayHit);
            }
            //We're still hitting something, but it's a new object
            else
            {
                //Trigger the ray "Exit" method on Interactable
                RayExit();

                //Keep track of new objec that we're hitting, and trigger the ray "Enter" method on Interactable
                hitObject = rayHit.transform.gameObject;
                RayEnter(rayHit);
            }
        }
        else
        {
            //We aren't hitting anything. Trigger ray "Exit" on Interactable
            RayExit();
        }
    }

    protected void RayEnter(RaycastHit hit)
    {
        
        //Asign to class variable so it can be used in RayStay
        hitObjectObjectInteractable = hitObject.GetComponent<Interactable>();
        if (hitObjectObjectInteractable)
        {
            
            //If hit object is an Interactable, trigger RayEnter Method
            hitObjectObjectInteractable.RayEnter(hit, controllerInput ?? null);
        }
    }

    protected void RayStay(RaycastHit hit)
    {
        
        if (hitObjectObjectInteractable)
        {
            hitObjectObjectInteractable.RayStay(hit, controllerInput ?? null);
        }
    }

    protected void RayExit()
    {

        if (hitObjectObjectInteractable)
        {

            hitObjectObjectInteractable.RayExit(controllerInput ?? null);
            hitObjectObjectInteractable = null;
            hitObject = null;
        }
    }
}
