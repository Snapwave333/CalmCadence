using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EventSeederTests
{
    [Test]
    public void Deterministic_Selection_With_Same_Seed()
    {
        var points = new List<(Vector3 pos, float weight)>
        {
            (new Vector3(0,0,0), 1f),
            (new Vector3(1,0,0), 2f),
            (new Vector3(2,0,0), 3f)
        };
        var s1 = new EventSeeder(1234);
        var s2 = new EventSeeder(1234);
        var p1 = s1.PickWeighted(points);
        var p2 = s2.PickWeighted(points);
        Assert.AreEqual(p1, p2);
    }
}


