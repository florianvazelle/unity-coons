using UnityEngine;

public struct Edge {
    public Vector3 start, end;

    public Edge(Vector3 s, Vector3 e) {
        start = s;
        end = e;
    }

    public bool Contains(Vector3 point) {
        return (start == point || end == point);
    }

    public bool isEqual(Edge edge) {
        return ((start == edge.start && end == edge.end) ||
                (start == edge.end && end == edge.start));
    }
}