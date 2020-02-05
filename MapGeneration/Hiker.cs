/* Hikers are pseudo-random walkers used to generate the world height map.
 * When provided with the same seed they will always walk the exact same route, 
 * allowing any map to be reproduced. Three different methods of hiking are 
 * available: 'Strictly Guided', 'Loosely Guided', and 'Vaguely Guided'. Each
 * method forces the Hiker to move from a start point to end point but with
 * diminishing directness. 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hiker 
{
    
    private uint seed;
    private System.Random rnd;

    // Every Hiker requires that a seed is provided when it is created
    public Hiker(int seed) {
        this.seed = (uint)seed;
        rnd = new System.Random(seed);
    }

    // IMPORTANT: 0 = north/up, 1 = east/right, 2 = south/down, 3 = west/left;

    public List<Vector2Int> StrictGuide (Vector2Int s, Vector2Int e) {
        // List of Vector2Int steps for the Hiker to take
        List<Vector2Int> steps = new List<Vector2Int>();
        // If start and end points are the same return only one step with no movement, aka return nothing and end the hike.
        if (s.Equals(e)) {
            steps.Add(Vector2Int.zero);
            return steps;
        }

        Vector2Int start = s;
        Vector2Int position = s;
        Vector2Int end = e;
        Vector2Int dir;

        while ((int)position.x != (int)end.x && (int)position.y != (int)end.y) {
            if (steps.Count == 0) {
                // If the total distance in x is more than the total distance in y ... 
                if (Mathf.Abs(start.x-end.x) > Mathf.Abs(start.y-end.y)) {
                    // If end.x is to the west of start.x, move west, else move right
                    dir = (start.x-end.x) > 0 ? Vector2Int.left : Vector2Int.right;
                    position += dir;
                    steps.Add(dir);
                } else {
                    // If end.x is to the south of start.y, move south, else move north
                    dir = (start.y - end.y) > 0 ? Vector2Int.down : Vector2Int.up;
                    position += (dir);
                    steps.Add(dir);
                }
            } else {
                // If the remaining distance in x (as a fraction of the total) is more than the remaining distance in y
                if (RemFraction(start.x,position.x,end.x) > RemFraction(start.y,position.y,end.y)) {
                    dir = (position.x - end.x) > 0 ? Vector2Int.left : Vector2Int.right;
                    position += dir;
                    steps.Add(dir);
                } else {
                    dir = (position.y - end.y) > 0 ? Vector2Int.down : Vector2Int.up;
                    position += (dir);
                    steps.Add(dir);
                }
            }
            position.x = Mathf.RoundToInt(position.x);
            position.y = Mathf.RoundToInt(position.y);

            // As a precaution, prevent the Hikers from taking more than 1000 steps in one hike
            if (steps.Count > 1000) break;
        }

        return steps;
    }

    public List<Vector2Int> LooseGuide (Vector2Int s, Vector2Int e) {
        // List of Vector2Int steps for the Hiker to take
        List<Vector2Int> steps = new List<Vector2Int>();
        Vector2Int p = s;
        Vector2Int dir = new Vector2Int();
        // If start and end points are the same return only one step with no movement, aka return nothing and end the hike.
        if (s.Equals(e))
        {
            steps.Add(Vector2Int.zero);
            return steps;
        }
        // Keep taking steps until the Hiker's position is at the end point
        while ((int)p.x != (int)e.x && (int)p.y != (int)e.y) {
            // Find the direction of the strict step from the current position
            Vector2 strictDir = new Vector2();
            // If the remaining distance in x (as a fraction of the total) is more than the remaining distance in y ...
            if (RemFraction(s.x, p.x, e.x) > RemFraction(s.y, p.y, e.y)) {
                strictDir = (p.x - e.x) > 0 ? Vector2Int.left : Vector2Int.right;
            } else {
                strictDir = (p.y - e.y) > 0 ? Vector2Int.down : Vector2Int.up;
            }
            Vector2Int leftTurn = strictDir.Rotate(90.0f).ToInt();
            Vector2Int rightTurn = strictDir.Rotate(-90.0f).ToInt();
            // Looseness is how far from the strict path the Hiker has wandered
            float looseness = GetLooseness(s, p, e);
            // Assign float ranges 0.0 to 1.0 which determine the step direction
            float strictLim = 0.5f * (1 + looseness / 100);
            float rightLim = strictLim + (1 - strictLim) / 2;
            // leftLim doesn't need to be set, will always be 1.0f

            // Upper limits are 98% strict, 1% right, 1% left
            if (strictLim > 0.98f) {
                strictLim = 0.98f;
                rightLim = 0.99f;
            }
            // Get random floats in sequence from hiker seed and use them to determine direction
            float limit = (float)rnd.NextDouble();
            if (limit < strictLim) {
                dir = strictDir.ToInt();
            } else if (limit < rightLim) {
                dir = rightTurn;
            } else {
                dir = leftTurn;
            }

            p += (dir);
            steps.Add(dir);
            // As a precaution, prevent the Hikers from taking more than 1000 steps in one hike
            if (steps.Count > 1000) break;
        }

        return steps;
    }

    // Return the fraction of the remaining distance from a point moving from start to end ** not vectors
    private float RemFraction (float s, float p, float e) {
        float totDist = e - s;
        float remDist = e - p;
        return remDist / totDist;
    }

    // Finds the orthogonal distance between the current Hiker position 
    // and the nearest point on the direct line btwn the start and end points
    private float GetLooseness (Vector2Int s, Vector2Int p, Vector2Int e) {
        float looseness = 0.0f;

        float m1 = (e.y - s.y) / (e.x - s.x);
        float b1 = s.y - s.x * m1;
        float m2 = -1 / m1;
        float b2 = p.y - p.x * m2;

        float iX = (b2 - b1) / (m1 - m2);
        float iY = m1 * iX + b1;

        Vector2 i = new Vector2(iX, iY);
        looseness = (p - i).magnitude;

        return looseness;
    }
}