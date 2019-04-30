using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivePanelOn : MonoBehaviour
{
    public GameObject panel;
    public GameObject myCam;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == myCam.tag)
        {
            panel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == myCam.tag)
        {
            panel.SetActive(false);
        }
    }
}
