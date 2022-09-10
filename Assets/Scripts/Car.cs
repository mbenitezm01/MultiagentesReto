using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public int nextX;
    public int nextZ;
    public Quaternion nextAngle;
    public string respawning;

    private void Start()
    {
        respawning = "false";
        Renderer carRenderer = transform.GetChild(0).GetComponent<Renderer>();
        carRenderer.material.SetColor("_Color", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
    }

    public void newPos(int x, int z)
    {
        nextX = x;
        nextZ = z;
    }

    public void newAngle(Quaternion angle)
    {
        nextAngle = angle;
    }
}
