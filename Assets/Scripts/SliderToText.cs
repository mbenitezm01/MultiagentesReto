using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToText : MonoBehaviour
{
    [SerializeField]
    private Slider parentSlider;

    [SerializeField]
    private Text textField;

    public GameObject controller;

    private void Update()
    {
        textField.text = parentSlider.value.ToString();
        controller.GetComponent<TCPIPServerAsync>().stepFrames = (int)parentSlider.value;
    }
}
