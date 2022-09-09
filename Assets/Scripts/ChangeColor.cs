using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public Material green, yellow, red;
    public string direction;
    private int state;
    // Start is called before the first frame update
    void Start()
    {
        if (direction == "north")
        {
            GetComponent<MeshRenderer>().material = green;
            state = 1;
        }
        else
        {
            GetComponent<MeshRenderer>().material = red;
            state = 0;
        }
    }

    // Update is called once per frame
    /*
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            if (state == 1 && direction != "up")
            {
                StartCoroutine(ChangeToRed());
                state = 0;
            }
            else if (direction == "up" && state != 1)
            {
                StartCoroutine(ChangeToGreen());
                state = 1;
            }
        }
        if (Input.GetKey(KeyCode.W)) 
        {
            if (state == 1 && direction != "down") 
            {
                StartCoroutine(ChangeToRed());
                state = 0;
            }
            else if (direction == "down" && state != 1)
            {
                StartCoroutine(ChangeToGreen());
                state = 1;
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            if (state == 1 && direction != "left")
            {
                StartCoroutine(ChangeToRed());
                state = 0;
            }
            else if (direction == "left" && state != 1)
            {
                StartCoroutine(ChangeToGreen());
                state = 1;
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (state == 1 && direction != "right")
            {
                StartCoroutine(ChangeToRed());
                state = 0;
            }
            else if (direction == "right" && state != 1)
            {
                StartCoroutine(ChangeToGreen());
                state = 1;
            }
        }
    }
    */

    public void colorChanger(string color)
    {
        if (color == "green")
        {
            GetComponent<MeshRenderer>().material = green;
        }
        else if (color == "yellow")
        {
            GetComponent<MeshRenderer>().material = yellow;
        }
        else if (color == "red")
        {
            GetComponent<MeshRenderer>().material = red;
        }

        /*
        if (dir == direction)
        {
            if (color == "green")
            {
                GetComponent<MeshRenderer>().material = green;
            }
            else if (color == "yellow")
            {
                GetComponent<MeshRenderer>().material = yellow;
            }
            else if (color == "red")
            {
                GetComponent<MeshRenderer>().material = red;
            }
        }
        */
    }
    /*
    private IEnumerator ChangeToGreen()
    {
        yield return new WaitForSeconds(8);
        GetComponent<MeshRenderer>().material = green;
    }

    private IEnumerator ChangeToRed()
    {
        GetComponent<MeshRenderer>().material = yellow;
        yield return new WaitForSeconds(5);
        GetComponent<MeshRenderer>().material = red;
    }
    */
}
