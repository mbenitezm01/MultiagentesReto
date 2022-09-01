using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorPrefab : MonoBehaviour
{
    public Material green, yellow, red;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            //Debug.Log("Cambiando semaforo a verde");
            GetComponent<MeshRenderer>().material = green;
        }
        if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log("Cambiando semaforo a amarillo");
            GetComponent<MeshRenderer>().material = yellow;
        }
        if (Input.GetKey(KeyCode.E))
        {
            //Debug.Log("Cambiando semaforo a rojo");
            GetComponent<MeshRenderer>().material = red;
        }
    }
}
