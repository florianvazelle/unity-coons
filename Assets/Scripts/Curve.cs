using System.Collections.Generic;
using UnityEngine;

public class Curve {
    public List<Edge> edges; 

    public Curve() {
        edges = new List<Edge>();
    }

    public void Add(Vector3 pointA, Vector3 pointB) {
        edges.Add(new Edge(pointA, pointB));
    }

    public void Render() {
        foreach (var edge in edges) {
            edge.Render();
        }
    }
}