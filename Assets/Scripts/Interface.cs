using System;
using System.Collections.Generic;
using UnityEngine;
using RapidGUI;
using static InterfaceUtils;



public class Interface : MonoBehaviour {

    private const int MAX_CURVES = 5;  

    public GameObject pointPrefab;

    private List<Curve> curves;
    private Vector3? lastPoint;
    private int curveIndex;
    private int subDivision;
    private int curveConstructorIndex1;
    private int curveConstructorIndex2;
    private Camera cam;

    void Start() {
        curves = new List<Curve>();   
        lastPoint = null;     
        curveIndex = 0;
        cam = Camera.main;
        subDivision = 0;

        // Pool curve
        for (int i = 0; i < MAX_CURVES * 2; i++) {
            curves.Add(new Curve());
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("create Point");
            Vector3 worldPosition = new Vector3();
            Vector2 mousePosition = new Vector2();

            mousePosition.x = Input.mousePosition.x;
            mousePosition.y = Input.mousePosition.y;
            worldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10)); // TODO : faire l'axe Z paramètrable

            if (lastPoint == null) {
                if (curves[curveIndex].edges.Count > 0) {
                    lastPoint = curves[curveIndex].edges[curves[curveIndex].edges.Count - 1].end;
                }
            }

            if (lastPoint != null) {
                curves[curveIndex].Add((Vector3)lastPoint, worldPosition);
            }
            lastPoint = worldPosition;
            // Instantiate(pointPrefab, worldPosition, Quaternion.identity);
        }
        
        curves[curveIndex + MAX_CURVES] = curves[curveIndex].SimpleCornerCutting(0.25f, 0.25f, subDivision);

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            subDivision += (Input.GetAxis("Mouse ScrollWheel") > 0 ? 1 : -1);
            subDivision = (int)Mathf.Clamp(subDivision, 0, 6);
        }
        
    }

    private void OnGUI() {
        if (transform.parent == null) {
            GUILayout.Label("Actions");
            DoGUI();
        }
    }

    public void DoGUI() {
        int curveIndexOld = RGUI.Slider(curveIndex, 0, MAX_CURVES, $"Current curve: {curveIndex}");
        if (curveIndexOld != curveIndex) {
            lastPoint = null;
            curveIndex = curveIndexOld;
        }
       
        GUILayout.Label($"Subdivision: {subDivision}");

        if (GUILayout.Button("Generate 4 Chaikin curves")) {
            List<Vector3> points = new List<Vector3>() {
                new Vector3(-3.1f, 1.5f, 0.0f),
                new Vector3(-0.1f, 2.6f, 0.0f),
                new Vector3(2.7f, 1.2f, 0.0f),
                new Vector3(4.8f, -0.4f, 0.0f),
                new Vector3(1.0f, -1.6f, 0.0f),
                new Vector3(-2.4f, -2.9f, 0.0f),
                new Vector3(-3.2f, -1.5f, 0.0f),
                new Vector3(-1.5f, -0.3f, 0.0f),
            };

            curves[0].edges = new List<Edge>() { new Edge(points[0], points[1]), new Edge(points[1], points[2]) };
            curves[1].edges = new List<Edge>() { new Edge(points[2], points[3]), new Edge(points[3], points[4]) };
            curves[2].edges = new List<Edge>() { new Edge(points[4], points[5]), new Edge(points[5], points[6]) };
            curves[3].edges = new List<Edge>() { new Edge(points[6], points[7]), new Edge(points[7], points[0]) };

            for (int i = 0; i < 4; i++) {
                curves[i + MAX_CURVES] = curves[i].SimpleCornerCutting(0.25f, 0.25f, 6);
            }

            curveIndex = 4;
        }

        if (GUILayout.Button("Coons")) CurveUtils.Coons(curves[0 + MAX_CURVES], curves[2 + MAX_CURVES], curves[1 + MAX_CURVES], curves[3 + MAX_CURVES]);
    }

    void OnPostRender() {
        foreach (var curve in curves) {
            curve.Render();
        }

         // Display real-time mouse movement
        if (curves.Count > 0) {
            Curve currentCurve = curves[curveIndex];
            
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)); // TODO : faire l'axe Z paramètrable
            if (currentCurve.edges.Count > 0) {
                Edge lastEdge = currentCurve.edges[currentCurve.edges.Count - 1];
                Edge tmp = new Edge(lastEdge.end, worldPosition);
                tmp.Render();
            } else if (lastPoint != null) {
                Edge tmp = new Edge((Vector3)lastPoint, worldPosition);
                tmp.Render();
            }
        }
    }

    public void ResetData() {
        curves.Clear();
    }
}