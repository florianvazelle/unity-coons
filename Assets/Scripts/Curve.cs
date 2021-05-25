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

        // Display real-time mouse movement
        if (edges.Count > 0) {
            // Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)); // TODO : faire l'axe Z paramètrable
            // Edge tmp = new Edge(edges[edges.Count - 1].end, worldPosition);
            // tmp.Render();
        }
    }
}