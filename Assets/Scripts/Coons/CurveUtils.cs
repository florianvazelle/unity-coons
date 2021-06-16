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
    
    static public List<Curve> Coons(Curve c0, Curve c1, Curve d0, Curve d1) {
        List<Vector3> points = new List<Vector3>();

        // Points du patch

        Vector3 C0_0 = c0.edges[0].start;
        Vector3 C0_1 = c0.edges[c0.edges.Count - 1].end;
        Vector3 C1_0 = c1.edges[0].start;
        Vector3 C1_1 = c1.edges[c1.edges.Count - 1].end;

        Vector3 D0_0 = d0.edges[0].start;
        Vector3 D0_1 = d0.edges[d0.edges.Count - 1].end;
        Vector3 D1_0 = d1.edges[0].start;
        Vector3 D1_1 = d1.edges[d1.edges.Count - 1].end;

        Debug.Log($"C0_0==D0_0 - {C0_0} == {D0_0}");
        Debug.Log($"C0_1==D1_0 - {C0_1} == {D1_0}");
        Debug.Log($"C1_0==D0_1 - {C1_0} == {D0_1}");
        Debug.Log($"C1_1==D1_1 - {C1_1} == {D1_1}");

        Debug.Assert(C0_0 == D0_0);
        Debug.Assert(C0_1 == D1_0);
        Debug.Assert(C1_0 == D0_1);
        Debug.Assert(C1_1 == D1_1);

        // Directions

        Vector3 dir_C0 = C0_1 - C0_0;
        Vector3 dir_C1 = C1_1 - C1_0;

        Vector3 dir_D0 = D0_1 - D0_0;
        Vector3 dir_D1 = D1_1 - D1_0;

        // Nombre de subdivision

        int u = c0.edges.Count + 1;
        int v = d0.edges.Count + 1;

        // On calcule

        List<Curve> curves = new List<Curve>();
        for(int j = 0; j < v; j++)
        {
            Vector3? lastPoint = null;
            curves.Add(new Curve());

            for (int i = 0; i < u; i++)
            {
                float s = ((float)i / u);
                float t = ((float)j / v);

                Vector3 rc = new Vector3();
                Vector3 rd = new Vector3();

                bool foundU = false, foundV = false;
                
                if (i <= c0.edges.Count) {
                    foundU = true;
                    rc = (1 - t) * c0.At(i) + t * c1.At(i);
                }
                
                if (j <= d0.edges.Count) {
                    foundV = true;
                    rd = (1 - s) * d0.At(j) + s * d1.At(j);
                }

                if (!foundU || !foundV) {
                    break;
                }
                
                Vector3 rcd = C0_0 * (1 - s) * (1 - t) + C0_1 * s * (1 - t) + C1_0 * (1 - s) * t + C1_1 * s * t;

                Vector3 point = rc + rd - rcd;

                if (lastPoint != null)
                    curves[curves.Count - 1].Add((Vector3)lastPoint, point);

                lastPoint = point;
            }
        }

        // Finalisation du quadrillage

        int initLength = curves.Count;
        for (int i = 0; i < initLength - 1; i++) {
            curves.Add(new Curve());

            for(int j = 0; j < curves[i].edges.Count; j++) {
                curves[curves.Count - 1].Add(curves[i].At(j), curves[i + 1].At(j));
            }
        }

        return curves;
    }
}