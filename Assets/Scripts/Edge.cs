using UnityEngine;

public class Edge {
    public Vector3 start, end;
    public Material lineMat; // Materiaux pour dessiner les lignes

    public Edge(Vector3 s, Vector3 e) {
        start = s;
        end = e;

        lineMat = new Material(Shader.Find("Unlit/Color"));
        lineMat.color = new Color(0, 0, 0);
    }

    public bool Contains(Vector3 point) {
        return (start == point || end == point);
    }

    public bool isEqual(Edge edge) {
        return ((start == edge.start && end == edge.end) ||
                (start == edge.end && end == edge.start));
    }

    public void Render() {
        GL.Begin(GL.LINES);
        lineMat.SetPass(0);
        GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
        GL.Vertex3(start.x, start.y, start.z);
        GL.Vertex3(end.x, end.y, start.z);
        GL.End();
    }
}