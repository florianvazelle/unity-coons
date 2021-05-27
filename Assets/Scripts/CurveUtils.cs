using System.Collections.Generic;
using UnityEngine;

public static class CurveUtils 
{
    static public Vector3 ProjectionPoint(Edge edge, float u) {
        Debug.Assert(0 <= u && u <= 1);

        Vector3 direction = (edge.end - edge.start).normalized;
        float distance = Vector3.Distance(edge.start, edge.end);

        return edge.start + (u * distance) * direction;
    }

    static public List<Edge> ConstructFace(Curve c1, Curve c2) { // Curve c1, Curve c2
        Debug.Assert(c1.edges.Count == c2.edges.Count);

        List<Edge> edges = new List<Edge>();
        for (int i = 0; i < c1.edges.Count; i++){
            edges.Add(new Edge(c1.edges[i].start, c2.edges[i].start));

            if(i == c1.edges.Count - 1){
                edges.Add(new Edge(c1.edges[i].end, c2.edges[i].end));
            }
        }

        Debug.Assert(edges.Count == c1.edges.Count + 1);

        return edges;
    }

    static public List<List<Vector3>> SplitEdges(List<Edge> edges, int nbSplit) {
        float unit = (1.0f / (float)nbSplit);
        
        List<List<Vector3>> subPoints = new List<List<Vector3>>();

        for (int j = 0; j < edges.Count; j++) {
            subPoints.Add(new List<Vector3>());

            for (int i = 0; i <= nbSplit; i++) {
                Vector3 Point = ProjectionPoint(edges[j], i * unit);
                subPoints[j].Add(Point);
            }
        }

        return subPoints;
    }

    static List<List<Vector3>> ComputePoint(Curve c1, Curve c2, int nbSubdiv) {
        List<List<Vector3>> points = SplitEdges(ConstructFace(c1, c2), nbSubdiv);

        foreach(var p in points) {
            Debug.Assert(p.Count == nbSubdiv + 1);
        }

        return points;
    }

    static List<List<Vector3>> GenerateBox(List<Edge> border, int initSubdiv, int nbSubdiv) {
        // On détermine les points de nos courbes droites
        List<List<Vector3>> lines = SplitEdges(border, initSubdiv);

        foreach(var l in lines) {
            Debug.Assert(l.Count == initSubdiv + 1);
        }

        // On initialise nos deux courbes droites
        Curve c1 = new Curve();
        Curve c2 = new Curve();

        // On calcule l'ensemble des edges qui raccordent tout les points
        c1.edges = new List<Edge>();
        for(int i = 0; i < lines[0].Count - 1; i++) {
            c1.edges.Add(new Edge(lines[0][i], lines[0][i + 1]));
        }
        Debug.Assert(c1.edges.Count == initSubdiv);

        c2.edges = new List<Edge>();
        for(int i = 0; i < lines[1].Count - 1; i++) {
            c2.edges.Add(new Edge(lines[1][i], lines[1][i + 1]));
        }
        Debug.Assert(c2.edges.Count == initSubdiv);

        // Ensuite, on calcule les points, des edges, qui relient les deux courbes
        return ComputePoint(c1, c2, nbSubdiv);
    }
    
    static public List<Vector3> Coons(Curve c1, Curve c2, Curve c3, Curve c4) {
        int nbSubdiv = c1.edges.Count + 1; // Correspond au nombre de points de la courbe

        // On peux imaginer List<List<Vector3>> comme une matrice de Vector3 ou (cols == row)

        List<List<Vector3>> a = ComputePoint(c1, c2, nbSubdiv);
        Debug.Log($"a.Count={a.Count}");
        Debug.Assert(nbSubdiv == a.Count);
        
        List<List<Vector3>> b = ComputePoint(c3, c4, nbSubdiv);
        Debug.Log($"b.Count={b.Count}");
        Debug.Assert(nbSubdiv == b.Count);

        // On crée deux edges qui représente notre courbe c1 et c2, comme si elles étaient droite 
        List<Edge> border = new List<Edge>() {
            new Edge(c1.edges[0].start, c1.edges[c1.edges.Count - 1].end),
            new Edge(c2.edges[0].start, c2.edges[c2.edges.Count - 1].end),
        };

        List<List<Vector3>> c = GenerateBox(border, nbSubdiv - 1, nbSubdiv);
        Debug.Log($"c.Count={c.Count}");
        Debug.Assert(nbSubdiv == c.Count);

        // On calcule les points du quadriallage en enlevant l'erreur
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < nbSubdiv; i++) {
            for (int j = 0; j < nbSubdiv; j++) {
                Vector3 p = a[i][j] + b[j][i] - c[i][j];
                points.Add(p);
            }
        }

        return points;
    }
}