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

    public void Render()
    {
        foreach (var edge in edges)
        {
            edge.Render();
        }
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
                    Edge edge1 = new Edge((Vector3)lastPCorner, PCorner);
                    edge1.SetColor(Color.red);
                    subEdges.Add(edge1);
                }

                //Ajouter les point Corner dans une nouvelle liste
                Edge edge2 = new Edge(PCorner, PCorner2);
                edge2.SetColor(Color.red);
                subEdges.Add(edge2);
                
                lastPCorner = PCorner2;

            }

            cloneEdges = new List<Edge>(subEdges);
        }

        if (cloneEdges.Count > 0) {
            cloneEdges[0] = edges[0];
            cloneEdges[cloneEdges.Count - 1] = edges[edges.Count - 1];
        }
        CurveOut.edges = new List<Edge>(cloneEdges);

        return CurveOut;
    }
}