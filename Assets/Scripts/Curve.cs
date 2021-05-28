using System.Collections.Generic;
using UnityEngine;

public class Curve
{
    public List<Edge> edges;

    public Curve()
    {
        edges = new List<Edge>();
    }

    public void Add(Vector3 pointA, Vector3 pointB)
    {
        edges.Add(new Edge(pointA, pointB));
    }

    public void Render(Color color)
    {
        foreach (var edge in edges)
        {
            edge.SetColor(color);
            edge.Render();
        }
    }

    public Vector3 At(int idx) {
        return ((idx == edges.Count) ? edges[idx - 1].end : edges[idx].start);
    }

    public Curve SimpleCornerCutting(float u, float v, int nbSubDiv)
    {
        Debug.Assert(u >= 0);
        Debug.Assert(v >= 0);
        Debug.Assert(u + v <= 1);

        Curve CurveOut = new Curve();
        List<Edge> cloneEdges = new List<Edge>(edges);

        for (int j = 0; j < nbSubDiv; j++)
        {
            Vector3? lastPCorner = null;
            List<Edge> subEdges = new List<Edge>();
            
            //Creation d'une courbe subdiviser
            for (int i = 0; i < cloneEdges.Count; i++)
            {   
                // Calcule Vecteur et les point Corner
                // https://math.stackexchange.com/questions/175896/finding-a-point-along-a-line-a-certain-distance-away-from-another-point
                Vector3 direction = (cloneEdges[i].end - cloneEdges[i].start).normalized;
                float distance = Vector3.Distance(cloneEdges[i].start, cloneEdges[i].end);

                Vector3 PCorner = CurveUtils.ProjectionPoint(cloneEdges[i], u);
                Vector3 PCorner2 = CurveUtils.ProjectionPoint(cloneEdges[i], u + (1 - (u + v)));

                if (lastPCorner != null) {
                    subEdges.Add(new Edge((Vector3)lastPCorner, PCorner));
                }

                //Ajouter les point Corner dans une nouvelle liste
                subEdges.Add(new Edge(PCorner, PCorner2));

                lastPCorner = PCorner2;
            }

            cloneEdges = new List<Edge>(subEdges);
        }

        if (cloneEdges.Count > 0) {
            cloneEdges[0].start = edges[0].start;
            cloneEdges[cloneEdges.Count - 1].end = edges[edges.Count - 1].end;
        }
        CurveOut.edges = new List<Edge>(cloneEdges);

        return CurveOut;
    }
}