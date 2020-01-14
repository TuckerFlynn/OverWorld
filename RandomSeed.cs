using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomSeed : MonoBehaviour {
    public InputField seedText;

	void Start () {
        SeedGen();
	}
	
	public void SeedGen () {
        uint rand = (uint) (Random.value * Mathf.Pow(10.0f, 8.0f));
        seedText.text = rand.ToString();
	}
}
