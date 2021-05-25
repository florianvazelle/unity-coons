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
        Curve CurveOut = new Curve();
        Vector3? lastPCorner = null;
        List<Edge> cloneEdges = new List<Edge>(edges);

        for (int j = 0; j < nbSubDiv; j++)
        {
            List<Edge> subEdges = new List<Edge>();
            
            //Creation d'une courbe subdiviser
            for (int i = 0; i < cloneEdges.Count; i++)
            {
                //calcule Vecteur et les point Corner
                Vector3 uv = (cloneEdges[i].start + cloneEdges[i].end).normalized;
                Vector3 PCorner = uv * u;
                Vector3 vu = (cloneEdges[i].end + cloneEdges[i].start).normalized;
                Vector3 PCorner2 = vu * v;

                if (lastPCorner != null)
                {
                    subEdges.Add(new Edge((Vector3)lastPCorner, PCorner));
                }

                //Ajouter les point Corner dans une nouvelle liste
                subEdges.Add(new Edge(PCorner, PCorner2));
                lastPCorner = PCorner2;

            }

            cloneEdges = new List<Edge>(subEdges);
            
        }


        return CurveOut;
    }
}