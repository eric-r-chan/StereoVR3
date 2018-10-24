using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using System.Linq;

public class CombinedController : MonoBehaviour
{
    public Shader cubeshader;
    public List<GameObject> images;
    public Texture btnTexture;


    void OnGUI()
    {

        GUIStyle mobileStyle = new GUIStyle(GUI.skin.textField);
        mobileStyle.fontSize = 50;
        Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
        mobileStyle.font = myFont;
        mobileStyle.normal.textColor = Color.white;

        if (GUI.Button(new Rect(500, 10, 300, 100), btnTexture))
        {
            LoadAndAlignImages();
        }
    }


    private void Awake()
    {
        GameObject image = new GameObject();
        ImageController imageController = image.AddComponent<ImageController>();
        imageController.PNGLeftURL = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/02119/NLB_585609051EDR_F0720202NCAM00256M1.png";
        imageController.PNGRightURL = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/02119/NLB_585609051EDR_F0720202NCAM00256M1.png";
        imageController.IMGURL = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/02119/NLB_585609051RASLF0720202NCAM00256M1.IMG";
        images.Add(image);
    }

    public void LoadAndAlignImages()
    {
        foreach (GameObject image in images)
        {
            ImageController imageController = image.AddComponent<ImageController>();
            imageController.LoadImage();
        }
    }




    public void Draw3DRKSML(string RKSMLURL)
    {
        List<Vector3> RKSML_Path = new List<Vector3>();

        XDocument xdoc = XDocument.Load(RKSMLURL);
        XNamespace ns = "RPK";
        int index = 0;

        float q_c = 1;
        float q_x = 0;
        float q_y = 0;
        float q_z = 0;


        Matrix4x4 p_matrix = Matrix4x4.Perspective(45, 1, .03f, 1000);
        Debug.Log("PERSPECTIVE MATRIX");
        Debug.Log(p_matrix);

        foreach (XElement node in xdoc.Descendants(ns + "Node"))
        {
            float x = -100001;
            float y = -100001;
            float z = -100001;

            foreach (XElement knot in node.Elements(ns + "Knot"))
            {
                if ((string)knot.Attribute("Name") == "ROVER_Z")
                    z = (float)knot;
                if ((string)knot.Attribute("Name") == "ROVER_Y")
                    y = (float)knot;
                if ((string)knot.Attribute("Name") == "ROVER_X")
                    x = (float)knot;
                if ((string)knot.Attribute("Name") == "QUAT_C")
                    q_c = (float)knot;
                if ((string)knot.Attribute("Name") == "QUAT_X")
                    q_x = (float)knot;
                if ((string)knot.Attribute("Name") == "QUAT_Y")
                    q_y = (float)knot;
                if ((string)knot.Attribute("Name") == "QUAT_Z")
                    q_z = (float)knot;
            }


            if (x > -100000 || y > -100000 || z > -100000) // if we read a timestep with Rover Position specified:
            {
                index++;
                RKSML_Path.Add(new Vector3(y, -z, x));
                if (index % 100 == 0)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<MeshRenderer>().material.shader = cubeshader;
                    cube.transform.localScale = new Vector3(1.0f, 0.25f, 0.25f);
                    cube.transform.position = new Vector3(y, -z, x);
                    cube.transform.rotation = new Quaternion(q_y,
                                          -q_z,
                                          q_x,
                                          -q_c);
                }
            }

        }
    }
}
