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

    static public void constructSurface(ref List<Curve> curves, int idx1, int idx2) { // Curve c1, Curve c2
        // voir pour debug si l'index n'existe pas
        if(curves[idx1].edges.Count != curves[idx2].edges.Count) {
            Debug.Log("Curves doesn't have the same amount of points");
            return;
        }
        Debug.Log(curves.Count);
        for (int i = 0; i < curves[idx1].edges.Count; i++){
            Curve c = new Curve();
            c.Add(curves[idx1].edges[i].start, curves[idx2].edges[i].start);
            curves.Add(c);

            if(i == curves[idx1].edges.Count - 1){
                Curve cend = new Curve();
                cend.Add(curves[idx1].edges[i].end, curves[idx2].edges[i].end);
                curves.Add(cend);
            }
        }
        Debug.Log(curves.Count);
    }

    static List<List<Vector3>> ComputePoint(Curve c1, Curve c2, int nbSubdiv) {
        return SplitEdges(ConstructFace(c1, c2), nbSubdiv);
    }

    static List<List<Vector3>> GenerateBox(List<Edge> border, int nbSubdiv) {
        List<List<Vector3>> lines = SplitEdges(border, nbSubdiv);

        Curve c1 = new Curve();
        c1.edges = lines[0];

        Curve c2 = new Curve();
        c2.edges = lines[1];

        return ComputePoint(c1, c2, nbSubdiv);
    }
    
    static List<Curve> Coons(Curve c1, Curve c2, Curve c3, Curve c4) {
        int nbSubdiv = 5;

        List<List<Vector3>> a1 = ComputePoint(c1, c2, nbSubdiv);
        List<List<Vector3>> b1 = ComputePoint(c3, c4, nbSubdiv);

        List<Edge> border = new List<Edge>() {
            new Edge(c1.edges[0].start, c1.edges[c1.edges.Count - 1].end),
            new Edge(c2.edges[0].start, c2.edges[c2.edges.Count - 1].end),
        };

        List<List<Vector3>> c1 = GenerateBox(border, nbSubdiv);

        List<Curve> curves = new List<Curve>();
        for (int i = 0; i < a1.Count; i++) {
            curves[i] = new Curve();

            for (int j = 0; j < b1.Count; j++) {

                Vector3 p1 = a1[i][j] + b1[j][i] - c1[i][j];
                Vector3 p2 = a1[i][j + 1] + b1[j + 1][i] - c1[i][j + 1];

                curves[i].Add(p1, p2);
            }
        }

        for (int j = 0; j < b1.Count; j++) {
            curves[i] = new Curve();
            
            for (int i = 0; i < a1.Count; i++) {

                Vector3 p1 = a1[i][j] + b1[j][i] - c1[i][j];
                Vector3 p2 = a1[i + 1][j] + b1[j][i + 1] - c1[i + 1][j];

                curves[j + a1.Count].Add(p1, p2);
            }
        }

        return curves;
    }
}