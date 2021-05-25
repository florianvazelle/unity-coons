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
        // voir pour debug si l'index n'existe pas
        if(c1.edges.Count != c2.edges.Count) {
            Debug.Log("Curves doesn't have the same amount of points");
            return new List<Edge>();
        }

        List<Edge> edges = new List<Edge>();
        for (int i = 0; i < c1.edges.Count; i++){
            edges.Add(new Edge(c1.edges[i].start, c2.edges[i].start));

            if(i == c1.edges.Count - 1){
                edges.Add(new Edge(c1.edges[i].end, c2.edges[i].end));
            }
        }

        return edges;
    }

    static List<List<Vector3>> ComputePoint(Curve c1, Curve c2, int nbSubdiv) {
        return SplitEdges(ConstructFace(c1, c2), nbSubdiv);
    }

    static List<List<Vector3>> GenerateBox(List<Edge> border, int nbSubdiv) {
        List<List<Vector3>> lines = SplitEdges(border, nbSubdiv);

        Curve c1 = new Curve();
        List<Edge> tmp1 = new List<Edge>();
        for(int i = 0; i < lines[0].Count - 1; i++) {
            tmp1.Add(new Edge(lines[0][i], lines[0][i + 1]));
        }
        c1.edges = tmp1;

        Curve c2 = new Curve();
        List<Edge> tmp2 = new List<Edge>();
        for(int i = 0; i < lines[1].Count - 1; i++) {
            tmp2.Add(new Edge(lines[1][i], lines[1][i + 1]));
        }
        c2.edges = tmp2;

        return ComputePoint(c1, c2, nbSubdiv);
    }
    
    static List<Curve> Coons(Curve c1, Curve c2, Curve c3, Curve c4) {
        int nbSubdiv = 5;

        List<List<Vector3>> a = ComputePoint(c1, c2, nbSubdiv);
        List<List<Vector3>> b = ComputePoint(c3, c4, nbSubdiv);

        List<Edge> border = new List<Edge>() {
            new Edge(c1.edges[0].start, c1.edges[c1.edges.Count - 1].end),
            new Edge(c2.edges[0].start, c2.edges[c2.edges.Count - 1].end),
        };

        List<List<Vector3>> c = GenerateBox(border, nbSubdiv);

        List<Curve> curves = new List<Curve>();
        for (int i = 0; i < a.Count; i++) {
            curves[i] = new Curve();

            for (int j = 0; j < b.Count; j++) {

                Vector3 p1 = a[i][j] + b[j][i] - c[i][j];
                Vector3 p2 = a[i][j + 1] + b[j + 1][i] - c[i][j + 1];

                curves[i].Add(p1, p2);
            }
        }

        for (int j = 0; j < b.Count; j++) {
            curves[j + a.Count] = new Curve();
            
            for (int i = 0; i < a.Count; i++) {

                Vector3 p1 = a[i][j] + b[j][i] - c[i][j];
                Vector3 p2 = a[i + 1][j] + b[j][i + 1] - c[i + 1][j];

                curves[j + a.Count].Add(p1, p2);
            }
        }

        return curves;
    }
}