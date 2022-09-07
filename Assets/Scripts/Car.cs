using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public void moveCar(int x, int z)
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(x, 0, z), 10 * Time.deltaTime);
    }
}
