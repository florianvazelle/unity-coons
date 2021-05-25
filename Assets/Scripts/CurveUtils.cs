using System.Collections.Generic;
using UnityEngine;

public static class CurveUtils 
{
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
}