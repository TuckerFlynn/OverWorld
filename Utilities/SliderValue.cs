using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour {
    public Text text;
    public Slider slider;
    public int scale = 1;

    void Start () {
        ValueToText();
    }

    public void ValueToText () {
        string num;

        if (slider.wholeNumbers == false) {
            num = (scale * Mathf.Round(slider.value * 100)/100).ToString();
        } else {
            num = (scale * slider.value).ToString();
        }

        text.text = num;
    }
}
