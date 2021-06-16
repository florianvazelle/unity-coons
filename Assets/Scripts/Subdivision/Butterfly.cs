using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InterfaceUtils;

public static class Butterfly
{

    public static void Subdivision(ref List<Triangle> triangles)
    {
        var newTriangles = new List<Triangle>();
        
        foreach (var ht in triangles)
        {
            /* massive assumption that there is 3 edges in our face */
            Edge e1 = ht.edges[0];
            Edge e2 = ht.edges[1];
            Edge e3 = ht.edges[2];

            /* might need to verify this doesn't pick duplicates */
            Vector3 v1 = e1.start;
            Vector3 v2 = e1.end;
            Vector3 v3 = ht.vertices[ht.GetOtherPoint(e1)];

            Vector3 v4 = SubdivideEdge(triangles, ht, e1, ht.vertices[ht.GetOtherPoint(e1)]);
            Vector3 v5 = SubdivideEdge(triangles, ht, e2, ht.vertices[ht.GetOtherPoint(e2)]);
            Vector3 v6 = SubdivideEdge(triangles, ht, e3, ht.vertices[ht.GetOtherPoint(e3)]);

            Triangle triangle = new Triangle(v4, v6, v1);
            newTriangles.Add(triangle);

            triangle = new Triangle(v5, v4, v2);
            newTriangles.Add(triangle);
            
            triangle = new Triangle(v3, v6, v5);
            newTriangles.Add(triangle);

            triangle = new Triangle(v6, v4, v5);
            newTriangles.Add(triangle);
        }

        triangles = newTriangles;
    }

    private static void GetAdjacentFace(in List<Triangle> triangles, Triangle triangle, Edge edge, out List<Triangle> adjacent_triangles)
    {
        adjacent_triangles = triangles
            .Where(t => t.hasEdge(edge) && !t.isEqual(triangle))
            .ToList();
    }

    private static Vector3 SubdivideEdge(in List<Triangle> triangles, Triangle current_triangle, Edge current_edge, Vector3 current_vertex)
    {
        /* On récupère le point central */
        Vector3 v = Vector3.zero;
        v = v + (current_edge.start / 2.0f);
        v = v + (current_edge.end / 2.0f);

        /* On récupère la face/triangle adjacente */
        List<Triangle> adjacent_triangles;
        GetAdjacentFace(in triangles, current_triangle, current_edge, out adjacent_triangles);
        if (adjacent_triangles.Count == 0) return v;
        Triangle opposite_triangle = adjacent_triangles[0];

        /* On récupère le point opposé (de la face adjacente) */
        Vector3 opposite_vertex = opposite_triangle.vertices[opposite_triangle.GetOtherPoint(current_edge)];

        v = v + (current_vertex / 8.0f);
        v = v + (opposite_vertex / 8.0f);

        foreach (var edge in current_triangle.edges)
        {
            if (!edge.isEqual(current_edge)) continue;

            GetAdjacentFace(in triangles, current_triangle, edge, out adjacent_triangles);
            Debug.Assert(adjacent_triangles.Count != 0);
            Vector3 adjacent_faces_vertex = adjacent_triangles[0].vertices[adjacent_triangles[0].GetOtherPoint(edge)];

            v = v - (adjacent_faces_vertex / 16.0f);
        }

        foreach (var edge in opposite_triangle.edges)
        {
            if (!edge.isEqual(current_edge)) continue;

            GetAdjacentFace(in triangles, opposite_triangle, edge, out adjacent_triangles);
            Debug.Assert (adjacent_triangles.Count != 0);
            Vector3 adjacent_faces_vertex = adjacent_triangles[0].vertices[adjacent_triangles[0].GetOtherPoint(edge)];

            v = v - (adjacent_faces_vertex / 16.0f);
        }

        return v;
    }
}