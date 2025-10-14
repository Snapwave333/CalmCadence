using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deterministic event seeding for spawn points.
/// </summary>
public class EventSeeder
{
    private readonly System.Random _random;

    public EventSeeder(int seed)
    {
        _random = new System.Random(seed);
    }

    public Vector3 PickWeighted(IReadOnlyList<(Vector3 pos, float weight)> points)
    {
        if (points == null || points.Count == 0) return Vector3.zero;
        float total = 0f;
        for (int i = 0; i < points.Count; i++) total += Mathf.Max(0f, points[i].weight);
        if (total <= 0f) return points[0].pos;
        double r = _random.NextDouble() * total;
        float accum = 0f;
        for (int i = 0; i < points.Count; i++)
        {
            accum += Mathf.Max(0f, points[i].weight);
            if (r <= accum) return points[i].pos;
        }
        return points[points.Count - 1].pos;
    }
}


