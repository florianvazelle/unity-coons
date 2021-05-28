using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent (typeof (Camera))]
public class ArcballScript : MonoBehaviour
{
    // Is true when the user wants to rotate the camera
    bool ballEnabled = false;
 
    float rotationSpeed = 1f;
    float radius = 5f;
 
    // The mouse cursor's position during the last frame
    Vector3 last = new Vector3();
 
    // The target that the camera looks at
    Vector3 target = new Vector3 (0, 0, 0);
 
    // The spherical coordinates
    Vector3 sc = new Vector3 ();
 
    void Start ()
    {
        this.transform.position = new Vector3 (radius, 0.0f, 0.0f);
        this.transform.LookAt (target);
        sc = getSphericalCoordinates (this.transform.position);
    }
 
    Vector3 getSphericalCoordinates(Vector3 cartesian)
    {
        float r = Mathf.Sqrt(
            Mathf.Pow(cartesian.x, 2) + 
            Mathf.Pow(cartesian.y, 2) + 
            Mathf.Pow(cartesian.z, 2)
        );
 
        float phi = Mathf.Atan2(cartesian.z / cartesian.x, cartesian.x);
        float theta = Mathf.Acos(cartesian.y / r);
 
        if (cartesian.x < 0)
            phi += Mathf.PI;
 
        return new Vector3 (r, phi, theta);
    }
 
    Vector3 getCartesianCoordinates(Vector3 spherical)
    {
        Vector3 ret = new Vector3 ();
 
        ret.x = spherical.x * Mathf.Cos (spherical.z) * Mathf.Cos (spherical.y);
        ret.y = spherical.x * Mathf.Sin (spherical.z);
        ret.z = spherical.x * Mathf.Cos (spherical.z) * Mathf.Sin (spherical.y);
 
        return ret;
    }
     
    // Update is called once per frame
    void Update ()
    {
        // Whenever the left mouse button is pressed, the
        // mouse cursor's position is stored for the arc-
        // ball camera as a reference.
        if (Input.GetMouseButtonDown(0))
        {
            // last is a global vec3 variable
            last = Input.mousePosition;
 
            // This is another global variable
            ballEnabled = true;
        }
 
        // When the user releases the left mouse button,
        // all we have to do is to reset the flag.
        if (Input.GetMouseButtonUp (0))
            ballEnabled = false;
 
        if (ballEnabled)
        {
            // Get the deltas that describe how much the mouse cursor got moved between frames
            float dx = (last.x - Input.mousePosition.x) * rotationSpeed;
            float dy = (last.y - Input.mousePosition.y) * rotationSpeed;
 
            // Only update the camera's position if the mouse got moved in either direction
            if (dx != 0f || dy != 0f)
            {
                // Rotate the camera left and right
                sc.y += dx * Time.deltaTime;
 
                // Rotate the camera up and down
                // Prevent the camera from turning upside down (1.5f = approx. Pi / 2)
                sc.z = Mathf.Clamp (sc.z + dy * Time.deltaTime, -1.5f, 1.5f);
 
                // Calculate the cartesian coordinates for unity
                transform.position = getCartesianCoordinates (sc) + target;
 
                // Make the camera look at the target
                transform.LookAt (target);
            }
 
            // Update the last mouse position
            last = Input.mousePosition;
        }
    }
}