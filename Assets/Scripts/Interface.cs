using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RapidGUI;
using static InterfaceUtils;

public class Interface2D : MonoBehaviour
{
    public GameObject pointPrefab;
    public Color lineColor;

    private Rect windowRect = new Rect(0, 0, 250, 500);     // Rect window for ImGUI
    private Material lineMat;                               // Voronoi Line material
    private List<Edge> edges;                               // Temporaly list with result of Voronoi

    void Start() {
        lineMat = new Material(Shader.Find("Unlit/Color"));
        lineMat.color = lineColor;
        edges = new List<Edge>();
    }

    private void OnGUI() {
        windowRect = GUI.ModalWindow(GetHashCode(), windowRect, DoGUI, "Actions", RGUIStyle.darkWindow);
    }

    public void DoGUI(int windowID) {
    }

    void OnPostRender() {
        foreach (var edge in edges) {
            GL.Begin(GL.LINES);
            lineMat.SetPass(0);
            GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
            GL.Vertex3(edge.start.x, edge.start.y, 0f);
            GL.Vertex3(edge.end.x, edge.end.y, 0f);
            GL.End();
        }
    }

    public void ResetData() {
        edges.Clear();
    }
}