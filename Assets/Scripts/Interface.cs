using System;
using System.Collections.Generic;
using UnityEngine;
using RapidGUI;
using static InterfaceUtils;



public class Interface : MonoBehaviour
{
    private const int MAX_CURVES = 10;  

    public GameObject pointPrefab;

    private Rect windowRect = new Rect(0, 0, 300, 200);     // Rect window for ImGUI
    private List<Curve> curves;
    private Vector3? lastPoint;
    private int curveIndex;
    private Camera cam;

    void Start() {
        curves = new List<Curve>();   
        lastPoint = null;     
        curveIndex = 0;
        cam = Camera.main;

        // Pool curve
        for (int i = 0; i < MAX_CURVES; i++) {
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
            if (lastPoint != null) {
                curves[curveIndex].Add((Vector3)lastPoint, worldPosition);
            }
            lastPoint = worldPosition;
            Instantiate(pointPrefab, worldPosition, Quaternion.identity);
        }
    }

    private void OnGUI() {
        windowRect = GUI.ModalWindow(GetHashCode(), windowRect, DoGUI, "Actions", RGUIStyle.darkWindow);
    }

    public void DoGUI(int windowID) {
       int curveIndexOld = RGUI.Slider(curveIndex, 0, MAX_CURVES, "Current curve:" + curveIndex);
       if (curveIndexOld != curveIndex) {
           lastPoint = null;
           curveIndex = curveIndexOld;
       }
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