using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Teleporting : MonoBehaviour
{
    private SteamVR_TrackedObject trackedObj;
    public Transform cameraRigTransform;
    public Transform headTransform;
    public GameObject LaserPrefab;
    public GameObject teleportReticlePrefab;
    public Vector3 teleportReticleOffset;
    public float altura;
    public LayerMask validTeleportMask, invalidTeleportMask;
    public int vertexCount = 1;
    public bool defaultCurve = true;

    public float distancia;
    private GameObject reticle;
    private Vector3 hitPoint;
    private LineRenderer laser;

    private Shader reticleShader;
    private bool shouldTeleport;
    private bool verde, rojo;
    
  
        //public List<Vector3> pointList = new List<Vector3>();

    public Vector3[] pointList;

    public Vector3 controllerPosition_dbg, laserPointPosition_dbg;
   // public List<Vector3> puntosBezier_dbg;

/********************REFERENCIAS AL CONTROL**********************/
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {

            trackedObj = GetComponent<SteamVR_TrackedObject>(); // se establece en la variable el objeto rastreado

    }
/***************************************************************/

    void Start()
    {
        laser = Instantiate(LaserPrefab).gameObject.GetComponent<LineRenderer>();//tomamos el linerender del prefab del laser
        reticle = Instantiate(teleportReticlePrefab);//tomamos el objeto del prefab del target
        reticleShader = reticle.GetComponent<Renderer>().material.shader;//tomamos el shader del material original del prefab del target
        verde = false;
        rojo = false;

        pointList = new Vector3[vertexCount];
    }

    void Update()
    {
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            RaycastHit hit;

            if (verde == false && rojo == false)
            {
                laser.enabled = false; //se desactiva tanto el laser
                reticle.SetActive(false);//como el target
                shouldTeleport = false;
            }

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, validTeleportMask))
            {
                hitPoint = hit.point; //asignamos a la variable la posición de choque del raycast con un objeto
                
                MarkerTeleport(hitPoint, reticle, true); // se manda a llamar el metodo del target con la opción de teletransporte activa
                verde = true;

            }
            else
            {
                verde = false;
            }

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, invalidTeleportMask))
            {
                hitPoint = hit.point; //asignamos a la variable la posición de choque del raycast con un objeto
                MarkerTeleport(hitPoint, reticle, false); // se manda a llamar el metodo del target con la opción de teletransporte inactiva
                rojo = true;
            }
            else
            {
                rojo = false;
            }

        }
        else
        {

            laser.enabled = false; //se desactiva tanto el laser
            reticle.SetActive(false);//como el target
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport)
        {

            Teleport();
        }
        controllerPosition_dbg = trackedObj.transform.position;
        laserPointPosition_dbg = hitPoint;

    }


    private void ShowLaser(Vector3 point)
    {
        if (defaultCurve)
        {
            for (int i = 0; i < vertexCount; i++)
            {
                float t = i / (float)vertexCount;

                // var alturaP = trackedObj.transform.position.y * altura * Vector3.Distance(trackedObj.transform.position, point);
                var p0 = trackedObj.transform.position;
                //var p1 = new Vector3(point.x, alturaP, point.z);
                var p2 = point;
                var p1 = (p0+p2)/2;

                //distancia = Vector3.Distance(p0, p2);
                distancia = Vector2.Distance(new Vector2(p0.x, p0.z), new Vector2(p2.x, p2.z));
                var alturaP = Mathf.Abs(p1.y * altura);

                if (p0.y > p2.y)
                {
                    p1 = new Vector3(p1.x, p0.y + alturaP, p1.z);

                }
                else
                {
                    p1 = new Vector3(p1.x, p2.y + alturaP, p1.z);

                }

                var bezier = CalculateLerpCurve(t, p0, p1, p2);
                pointList[i] = bezier;

            }
        }
        else
        {
            for (int i = 0; i < vertexCount; i++)
            {
                float t = i / (float)vertexCount;

               // var alturaP = trackedObj.transform.position.y * altura * Vector3.Distance(trackedObj.transform.position, point);
                var p0 = trackedObj.transform.position;
              //var p1 = new Vector3(point.x, alturaP, point.z);
                var p2 = point;
                var p1 = (p0 + p2) / 2;

                distancia = Vector2.Distance(new Vector2(p0.x, p0.z), new Vector2(p2.x, p2.z));

                var alturaP = Mathf.Abs(p1.y * altura);

                if (p0.y > p2.y)
                {
                    p1 = new Vector3(p1.x, p0.y + alturaP, p1.z);

                }
                else
                {
                    p1 = new Vector3(p1.x, p2.y + alturaP, p1.z);

                }

                var bezier = CalculateQuadraticCurve(t, p0, p1, p2);
                pointList[i] = bezier;
            }
        }

        laser.positionCount = pointList.Length;
        laser.SetPositions(pointList);

    }


    private Vector3 CalculateQuadraticCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // B(t) = (1-t)2 P0 + 2(1-t)t P1 + t2 P2 , 0 < t < 1
        float u = 1 - t;
        float uu = u * u;
        float tt = t * t;

        Vector3 point = uu * p0 + 2 * u * t * p1 + tt * p2;
        return point;
    }


    private Vector3 CalculateLerpCurve(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        var line1 = Vector3.Lerp(p0, p1, t);
        var line2 = Vector3.Lerp(p1, p2, t);
        var bezier = Vector3.Lerp(line1, line2, t);
        return bezier;
    }

    private void Teleport()
    {
        shouldTeleport = false;
        reticle.SetActive(false);
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        cameraRigTransform.position = hitPoint + difference;
    }

    public void MarkerTeleport(Vector3 point, GameObject reticleVerdadero, bool flag)
    {
        laser.enabled = true;
        ShowLaser(point);
        reticleVerdadero.SetActive(true);
        reticleVerdadero.GetComponent<Renderer>().material.shader = reticleShader;
        laser.GetComponent<Renderer>().material.shader = reticleShader;
        if (flag)
        {
            reticleVerdadero.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.006751776f, 1f, 0f, 6705883f));
            laser.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.006751776f, 1f, 0f, 6705883f));
        }
        if (!flag)
        {
            reticleVerdadero.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1, 0.1323529f, 0.2221092f, 0.57640706f));
            laser.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1, 0.1323529f, 0.2221092f, 0.57640706f));
        }
        reticle.transform.position = hitPoint + teleportReticleOffset;
        shouldTeleport = flag;
    }
}
