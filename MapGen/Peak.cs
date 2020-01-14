using System;
using UnityEngine;

public class Peak {
    public Vector2 position;

    public Peak(Vector2 position) {
        this.position = position;
    }

    public float Elevation
    { get; set; }

    public float SlopeConst
    { get; set; }

    public float CurrentHeight
    { get; set; }

    public int CurrentRadius
    { get; set; }

    public int AffectedFields
    { get; set; }

    public int StepCount
    { get; set;  }
}
