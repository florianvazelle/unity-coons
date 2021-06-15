using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Triangle
{
    public List<Vector3> vertices;
    public List<Edge> edges;

    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        vertices = new List<Vector3>() {
            p1, p2, p3
        };

        edges = new List<Edge>() {
            new Edge(p1, p2),
            new Edge(p2, p3),
            new Edge(p3, p1)
        };
    }

    public bool isEqual(Triangle triangle)
    {
        Vector3 p11 = vertices[0];
        Vector3 p21 = vertices[1];
        Vector3 p31 = vertices[2];

        Vector3 p12 = triangle.vertices[0];
        Vector3 p22 = triangle.vertices[1];
        Vector3 p32 = triangle.vertices[2];

        return ((p11 == p12 && p21 == p22 && p31 == p32) ||
                (p11 == p12 && p21 == p32 && p31 == p22) ||
                (p11 == p22 && p21 == p12 && p31 == p32) ||
                (p11 == p22 && p21 == p32 && p31 == p12) ||
                (p11 == p32 && p21 == p22 && p31 == p12) ||
                (p11 == p32 && p21 == p12 && p31 == p22));
    }

    // Test si le triangle possède cette arête
    public bool hasEdge(Edge edge)
    {
        return edges.Count(e => e.isEqual(edge)) > 0;
    }


    // Retourne le point du triangle qui n'appartient pas a l'arête
    public Vector3 GetOtherPoint(Edge edge)
    {
        for (var i = 0; i < vertices.Count; i++)
        {
            if (!edge.Contains(vertices[i]))
            {
                return vertices[i];
            }
        }
        // Techniquement cela ne passe jamais ici (edge = 2 points et triangle = 3 points)
        return Vector3.zero;
    }

    public Vector3 Center()
    {
        return (vertices[0] + vertices[1] + vertices[2]) / 3.0f;
    }
}