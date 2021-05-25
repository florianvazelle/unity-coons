using UnityEngine;

public struct Curve {
    public List<Edge> edges; 
    private Material lineMat; // Materiaux pour dessiner les lignes

    public Curve() {
        edges = new List<Edge>();
        lineMat = new Material(Shader.Find("Unlit/Color"));
        lineMat.color = Color(0, 0, 0);
    }

    public render() {
        foreach (var edge in edges) {
            GL.Begin(GL.LINES);
            lineMat.SetPass(0);
            GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
            GL.Vertex3(edge.start.x, edge.start.y, edge.start.z);
            GL.Vertex3(edge.end.x, edge.end.y, edge.start.z);
            GL.End();
        }
    }
}