using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour {
    public Text text;
    public Slider slider;

    void Start () {
        ValueToText();
    }

    public void ValueToText () {
        string num;
        if (slider.wholeNumbers == false) {
            num = (Mathf.Round(slider.value * 100)/100).ToString();
        } else {
            num = slider.value.ToString();
        }
        text.text = num;
    }
}
