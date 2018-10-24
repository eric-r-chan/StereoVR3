using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
//using System.Xml;
using System.Xml.Linq;
using System.Linq;





public class RKSMLDrawer : MonoBehaviour
{


    public List<Vector3> RKSML_Path;
    public string RKSMLURL = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/01686/poses_corr_smoothed.rksml";
    public GameObject Plane;
    public GameObject Player;
    public bool draw3DPath = false;
    public Shader cubeshader;
    public float FOV = 45;

    void Start()
    {

        LineRenderer lr = gameObject.AddComponent<LineRenderer>() as LineRenderer;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        //lr.material = new Material(Shader.Find("Default-Line"));
        //lr.SetColors(Color.white, Color.white);
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.startColor = Color.white;
        lr.endColor = Color.white;


        XDocument xdoc = XDocument.Load(RKSMLURL);
        XNamespace ns = "RPK";
        int index = 0;
        //int numVertices = 1;

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
                    q_z = (float)knot;}


            if (x > -100000 || y > -100000 || z > -100000)
            {
                //Debug.Log(index);
                //lr.SetPosition(index, new Vector3(x, z, y));
                index++;
                RKSML_Path.Add(new Vector3(y, -z, x));
                if (index % 100 == 0)
                {
                   



                    if (draw3DPath)
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
                    else
                    {
                        Vector3 initPoint = new Vector3(-10.71f, 0.805f, -7.151f);
                        //Vector3 initPoint = new Vector3(x, -z, y);
                        Vector3 cameraRelative = Player.transform.InverseTransformPoint(initPoint);
                        //Vector3 cameraRelative = Player.transform.InverseTransformPoint(initPoint);
                        float S = 1 / Mathf.Tan(FOV / 2.0f * Mathf.PI / 180f);
                        //Debug.Log("S:");
                        //Debug.Log(S);
                        Debug.Log("CAMERA RELATIVE");
                        Debug.Log(cameraRelative);
                        Vector3 projectedPoint = new Vector3(cameraRelative.x * S / -cameraRelative.y, 0, cameraRelative.z * S / -cameraRelative.y);


                        /*
                        Vector3 initPoint = new Vector3(-10.71f, 0.805f, -7.151f);
                        Vector3 cameraRelative = Player.transform.InverseTransformPoint(initPoint);

                        //Vector3 cameraRelative = new Vector3(-7.98, 10, -2);
                        Debug.Log("CAMERA RELATIVE");
                        Debug.Log(cameraRelative);
                        //Vector3 initPoint = new Vector3(0, 5, 0); // y axis is distance from camera
                        //Vector3 point = new Vector3(cameraRelative.x, cameraRelative.z, cameraRelative.y);
                        //Vector3 point = new Vector3(5, )
                        //Vector3 point = new Vector3(y, x, -z);
                        //Vector4 point = new Vector4(y, -z, x, 1);
                        //Vector4 conv = point * p_matrix;
                        Vector3 converted = p_matrix.MultiplyPoint(cameraRelative);
                        Debug.Log("MULTIPLIED");
                        Debug.Log(converted);
                        //Vector3 render = new Vector3(converted.x, 0, converted.y);
                        Vector3 render = new Vector3(converted.x, 0, converted.z);
                        //Debug.Log(converted);
                        //converted.y = 0;
                        */

                        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.parent = Plane.transform;
                        cube.transform.localPosition = projectedPoint;
                        cube.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                }
            }


            //Debug.Log(RKSML_Path.ToArray());
            lr.positionCount = index;
            //lr.SetPositions(RKSML_Path.ToArray());


            //lr.SetPosition(0, new Vector3(0, 0, 0));
            //lr.SetPosition(1, new Vector3(10, 10, 10));
            // GameObject.Destroy(myLine, 20);

        }


    }
}

