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
        Debug.Assert(u > 0);
        Debug.Assert(v > 0);
        Debug.Assert(u + v < 1);

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
                Vector3 Pv = (cloneEdges[i].end - cloneEdges[i].start);
                Vector3 Pu = Pv.normalized;

                float d = Vector3.Distance(cloneEdges[i].start, cloneEdges[i].end);

                Vector3 PCorner = cloneEdges[i].start + (u * d) * Pu;
                Vector3 PCorner2 = cloneEdges[i].start + ((u + (1 - (u + v))) * d) * Pu;

                if (lastPCorner != null) {
                    subEdges.Add(new Edge((Vector3)lastPCorner, PCorner));
                }

                //Ajouter les point Corner dans une nouvelle liste
                subEdges.Add(new Edge(PCorner, PCorner2));
                lastPCorner = PCorner2;

            }

            cloneEdges = new List<Edge>(subEdges);
        }

        CurveOut.edges = new List<Edge>(cloneEdges);

        return CurveOut;
    }
}