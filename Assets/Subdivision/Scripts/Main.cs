using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using Subdivision.Core;

public class Main : MonoBehaviour {

    public Text text;
    public GameObject obj;
    private MeshFilter meshFilter;
    MeshConverter converter;
    CatmullClarkSubdivider CCSubdivider;

    Shape shape;

    // Initialisation
    void Start()
    {
        meshFilter = obj.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = obj.AddComponent<MeshFilter>();
        }

        converter = new MeshConverter(); 
        shape = converter.OnConvert(meshFilter.mesh); //Convertir la mesh passer en parametre en Shape
        text.text = shape.AllPoints.Count.ToString(); // Afficher le nombre de sommet
        CCSubdivider = new CatmullClarkSubdivider();  // Instantier CatmullClark, mais pas encore executer
    } 
    
    //A chaque fois qu'on click sur le boutton: il prend le shape actuelle est le resubdivise
    public void HandleOnSundive()
    {
        shape = CCSubdivider.Subdivide(shape);        //Cette fois ci on recupere le Shape obtenir et le subdivise
        text.text = shape.AllPoints.Count.ToString(); //Reaffichage des sommet
        meshFilter.mesh = converter.ConvertToMesh(shape); //A la fin on recupere la nouvelle Shape obtenu apres la subdivision. Pour pouvoir réitérer si on le souhaite.

        MeshRenderer rend = obj.GetComponent<MeshRenderer>();
        if (rend == null)
        {
            rend = obj.AddComponent<MeshRenderer>();
        }
        rend.material = new Material(Shader.Find("VR/SpatialMapping/Wireframe"));
    }


}

