using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoplightController : MonoBehaviour
{
    public void changeLights(string direction, string color)
    {

        for (int i = 0; i < 4; i++)
        {
            if(direction == "north")
            {
                transform.GetChild(0).GetChild(i).GetComponent<ChangeColor>().colorChanger(color);
            }
            else if (direction == "south")
            {
                transform.GetChild(1).GetChild(i).GetComponent<ChangeColor>().colorChanger(color);
            }
            else if (direction == "west")
            {
                transform.GetChild(2).GetChild(i).GetComponent<ChangeColor>().colorChanger(color);
            }
            else if (direction == "east")
            {
                transform.GetChild(3).GetChild(i).GetComponent<ChangeColor>().colorChanger(color);
            }
        }
    }
}
