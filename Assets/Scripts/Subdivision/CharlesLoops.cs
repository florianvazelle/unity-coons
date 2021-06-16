using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InterfaceUtils;

public static class CharlesLoops {

    // get all neighbors of one vertex in a list of triangles
    public static List<Vector3> getNeighbors(Vector3 p, in List<Triangle> triangles) {
        var neighborhoods = triangles
            .Where(t => t.vertices.Contains(p))
            .Select(t => {
                for (int i = 0; i < 3; i++)
                    if (t.edges[i].start == p) {
                        return t.edges[i].end;
                    } else if (t.edges[i].end == p) {
                        return t.edges[i].start;
                    }
                Debug.Assert(false);
                return Vector3.zero;
            })
            .ToList();
        return neighborhoods;
    }

    public static List<Vector3> getEdgeNeighbors(Edge current_edge, Triangle current_triangle, ref List<Triangle> triangles) {
        List<Vector3> edgeNeighbors = new List<Vector3>();

        Vector3 vRight = current_triangle.vertices[current_triangle.GetOtherPoint(current_edge)];

        /* On récupère la face/triangle adjacente */
        List<Triangle> adjacent_triangles = triangles.Where(t => t.hasEdge(current_edge) && !t.isEqual(current_triangle)).ToList();
        Debug.Assert(adjacent_triangles.Count != 0);
        Triangle opposite_triangle = adjacent_triangles[0];

        /* On récupère le point opposé (de la face adjacente) */
        Vector3 vLeft = opposite_triangle.vertices[opposite_triangle.GetOtherPoint(current_edge)];

        edgeNeighbors.Add(vRight);
        edgeNeighbors.Add(vLeft);
        return edgeNeighbors;
    }

    public static List<Triangle> getDisturbedTriangles(in List<Triangle> triangles) {
        List<Triangle> diturbedTriangles = new List<Triangle>();

        // move each vertices of each triangle
        for(int i = 0; i < triangles.Count; i++)
        {
            List<Vector3> vertices = new List<Vector3>();
            for (int j = 0; j < 3; j++)
            {
                // get neighbors of each vertex of each triangle
                List<Vector3> neighbors = CharlesLoops.getNeighbors(triangles[i].vertices[j], in triangles);
                
                // determine n
                float n = neighbors.Count;
                
                // determine alpha
                float alpha;
                if(n == 3) {
                    alpha = 3f/16f;
                } else {
                    alpha = (1f/n) * ((5f/8f) - Mathf.Pow( (3f/8f) + (1f/4f) * Mathf.Cos((2f * Mathf.PI)/n) , 2));
                }
                
                // sum all the neighbors
                Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);
                foreach(var vec in neighbors) {
                    sum += vec;
                }

                // create new point v'
                vertices.Add(((1 - (n * alpha)) * triangles[i].vertices[j]) + alpha * sum); // + sum
            }

            diturbedTriangles.Add(new Triangle(vertices[0], vertices[1], vertices[2]));
        }

        return diturbedTriangles;
    }

    public static List<Triangle> subdivideByEdge(ref List<Triangle> triangles, ref List<Triangle> disturbedTriangles) {
        List<Triangle> subdivideTriangles = new List<Triangle>();

        // for each triangle
        for(int i = 0; i < triangles.Count; i++) {
            // create list for new vertex
            List<Vector3> e = new List<Vector3>();
            // for each edge
            for(int j = 0; j < triangles[i].edges.Count; j++) {
                // get edge neighbors
                List<Vector3> edgeNeighbors = getEdgeNeighbors(triangles[i].edges[j], triangles[i], ref triangles);
                if(edgeNeighbors.Count != 2)
                    Debug.Log("error: didn't find neighbors");
                // create e
                Vector3 ej = ((3f/8f) * (triangles[i].edges[j].start + triangles[i].edges[j].end)) + ((1f/8f) * (edgeNeighbors[0] + edgeNeighbors[1]));
                e.Add(ej);
            }

            if(e.Count != 3)
                Debug.Log("error: error while subdivide e != 3");

            Triangle t1 = new Triangle(disturbedTriangles[i].vertices[0], e[1], e[2]);
            Triangle t2 = new Triangle(disturbedTriangles[i].vertices[1], e[0], e[2]);
            Triangle t3 = new Triangle(disturbedTriangles[i].vertices[2], e[2], e[1]);
            Triangle t4 = new Triangle(e[0], e[1], e[2]);

            subdivideTriangles.Add(t1);
            subdivideTriangles.Add(t2);
            subdivideTriangles.Add(t3);
            subdivideTriangles.Add(t4);

            // Debug.Log("disturbedTriangles[i].vertices[0] = " + disturbedTriangles[i].vertices[0]);
            // Debug.Log("disturbedTriangles[i].vertices[1] = " + disturbedTriangles[i].vertices[1]);
            // Debug.Log("disturbedTriangles[i].vertices[2] = " + disturbedTriangles[i].vertices[2]);
            // Debug.Log("e[0] = " + e[0]);
            // Debug.Log("e[1] = "+ e[1]);
            // Debug.Log("e[2] = " + e[2]);
        }

        return subdivideTriangles;
    }
}