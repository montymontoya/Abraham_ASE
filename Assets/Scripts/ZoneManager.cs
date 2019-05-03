using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public GameObject zone2Active,thisGameObject;
    // Start is called before the first frame update
    void Start()
    {
        thisGameObject = this.GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (thisGameObject.activeSelf)
        {
            zone2Active.SetActive(true);
        }
        
    }
}
