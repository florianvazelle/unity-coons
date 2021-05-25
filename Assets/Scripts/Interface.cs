using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using RapidGUI;
using static InterfaceUtils;

public class Interface2D : MonoBehaviour
{
    public GameObject pointPrefab;

    private Rect windowRect = new Rect(0, 0, 250, 500);     // Rect window for ImGUI
    private List<Curve> curve;
    private Event event;

    void Start() {
        curve = new List<Edge>();
        event = new Event();
    }

    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            Debug.Log("create Point");
            Vector3 worldPosition = new Vector3();
            Vector2 mousePosition = new Vector2();

            mousePosition.x = event.mousePosition.x;
            mousePosition.y = cam.pixelHeight - event.mousePosition.y;
            worldPosition = cam.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 10)); // TODO : faire l'axe Z paramètrable
            Instantiate(pointPrefab, worldPosition, Quaternion.identity);
        }
    }

    private void OnGUI() {
        windowRect = GUI.ModalWindow(GetHashCode(), windowRect, DoGUI, "Actions", RGUIStyle.darkWindow);
    }

    public void DoGUI(int windowID) {
    }

    void OnPostRender() {
        foreach (var curve in curves) {
            curve.render();
        }
    }

    public void ResetData() {
        curve.Clear();
    }
}