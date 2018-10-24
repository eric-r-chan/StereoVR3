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

public class ImageController : MonoBehaviour
{
    public string PNGLeftURL;
    public string PNGRightURL;
    public string IMGURL;
    private GameObject cameraObject;

    private float AZIMUTH_FOV;
    private float ELEVATION_FOV;

    private GameObject displayLeft;
    private GameObject displayRight;

    private float displayScaleA;
    private float displayScaleE;
    private int displayDistance;

    public void Start()
    {
        
    }

    // Load stereo displays
    public void LoadImage()
    {
        cameraObject = new GameObject();
        cameraObject.transform.parent = gameObject.transform;
        cameraObject.transform.localPosition = new Vector3(0, 0, 0);
        displayLeft = GameObject.CreatePrimitive(PrimitiveType.Quad);
        displayRight = GameObject.CreatePrimitive(PrimitiveType.Quad);


        displayLeft.transform.parent = cameraObject.transform;
        displayRight.transform.parent = cameraObject.transform;
        displayLeft.transform.localPosition = new Vector3(0, 0, 1);
        displayRight.transform.localPosition = new Vector3(0, 0, 1);

        StartCoroutine(DownloadToDisplay(PNGLeftURL, PNGLeftURL));
    }
    private IEnumerator DownloadToDisplay(string pngLeftURL, string pngRightURL)
    {
        Debug.Log("Loading Images");
        using (WWW www = new WWW(pngLeftURL))
        {
            yield return www;
            displayLeft.GetComponent<Renderer>().material.mainTexture = www.texture;
        }

        using (WWW www = new WWW(pngRightURL))
        {
            yield return www;
            displayRight.GetComponent<Renderer>().material.mainTexture = www.texture;
        }
    }

    // Align Image
    private void AlignImage()
    {
        Vector3 CAMERA_POS = new Vector3(0, 0, 0);
        float INSTRUMENT_AZIMUTH = 0;
        float INSTRUMENT_ELEVATION = 0;
        bool LOCAL_INSTRUMENTS = true;
        Vector3 ORIGIN_OFFSET_VECTOR = new Vector3(0, 0, 0);
        Quaternion ORIGIN_ROTATION_QUATERNION = new Quaternion(0, 0, 0, 1);

        Regex AZIMUTH_FOV_REGEX = new Regex(@"AZIMUTH_FOV\s*=\s*([+-]?[0-9]*[.]?[0-9]+)");
        Regex ELEVATION_FOV_REGEX = new Regex(@"ELEVATION_FOV\s*=\s*([+-]?[0-9]*[.]?[0-9]+)");
        Regex MODEL_COMPONENT_1_REGEX = new Regex(@"MODEL_COMPONENT_1\s*=\s*\(([+-]?[0-9]*[.]?[0-9]+),([+-]?[0-9]*[.]?[0-9]+),([+-]?[0-9]*[.]?[0-9]+)\)");
        Regex INSTRUMENT_AZIMUTH_REGEX = new Regex(@"INSTRUMENT_AZIMUTH\s*=\s*([+-]?[0-9]*[.]?[0-9]+)\ <deg>");
        Regex INSTRUMENT_ELEVATION_REGEX = new Regex(@"INSTRUMENT_ELEVATION\s*=\s*([+-]?[0-9]*[.]?[0-9]+)\ <deg>");
        Regex ORIGIN_OFFSET_VECTOR_REGEX = new Regex(@"ORIGIN_OFFSET_VECTOR\s*=\s*\(([+-]?[0-9]*[.]?[0-9]+),([+-]?[0-9]*[.]?[0-9]+),([+-]?[0-9]*[.]?[0-9]+)\)");
        Regex ORIGIN_ROTATION_QUATERNION_REGEX = new Regex(@"ORIGIN_ROTATION_QUATERNION\s*=\s*\(([+-]?[0-9]*[.]?[0-9]+),([+-]?[0-9]*[.]?[0-9]+),([+-]?[0-9]*[.]?[0-9]+),([+-]?[0-9]*[.]?[0-9]+)\)");

        WebClient client = new WebClient();
        Stream stream = client.OpenRead(IMGURL);
        StreamReader reader = new StreamReader(stream);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            Match match = AZIMUTH_FOV_REGEX.Match(line);
            if (match.Success)
            {
                AZIMUTH_FOV = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            match = ELEVATION_FOV_REGEX.Match(line);
            if (match.Success)
            {
                ELEVATION_FOV = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            match = MODEL_COMPONENT_1_REGEX.Match(line);
            if (match.Success)
            {
                CAMERA_POS = new Vector3(float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                                          -float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                                          float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
            }

            match = INSTRUMENT_AZIMUTH_REGEX.Match(line);
            if (match.Success && LOCAL_INSTRUMENTS)
            {
                INSTRUMENT_AZIMUTH = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }

            match = INSTRUMENT_ELEVATION_REGEX.Match(line);
            if (match.Success && LOCAL_INSTRUMENTS)
            {
                INSTRUMENT_ELEVATION = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                LOCAL_INSTRUMENTS = false;
            }

            match = ORIGIN_OFFSET_VECTOR_REGEX.Match(line);
            if (match.Success)
            {
                ORIGIN_OFFSET_VECTOR = new Vector3(float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                                          -float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                                          float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
            }

            match = ORIGIN_ROTATION_QUATERNION_REGEX.Match(line);
            if (match.Success)
            {

                ORIGIN_ROTATION_QUATERNION = new Quaternion(float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                                          -float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
                                          float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                                          -float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture));
            }

        }

        cameraObject.transform.localPosition = CAMERA_POS;
        gameObject.transform.rotation = ORIGIN_ROTATION_QUATERNION;
        gameObject.transform.position = ORIGIN_OFFSET_VECTOR;
        cameraObject.transform.localEulerAngles = new Vector3(-INSTRUMENT_ELEVATION, INSTRUMENT_AZIMUTH, 0);

        displayScaleA = Mathf.Tan(180 / Mathf.PI * INSTRUMENT_AZIMUTH) * displayDistance;
        displayScaleE = Mathf.Tan(180 / Mathf.PI * INSTRUMENT_ELEVATION) * displayDistance;
        displayLeft.transform.localScale = new Vector3(displayScaleA, displayScaleE, 1);
        displayRight.transform.localScale = new Vector3(displayScaleA, displayScaleE, 1);
    }


    public void Draw2DRKSML(string RKSMLURL)
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

                    Vector3 initPoint = new Vector3(y, -z, x);
                    Vector3 cameraRelative = cameraObject.transform.InverseTransformPoint(initPoint);
                    float SELV = 1f / Mathf.Tan(ELEVATION_FOV / 2.0f * Mathf.PI / 180f);
                    float SAZ = 1f / Mathf.Tan(AZIMUTH_FOV / 2.0f * Mathf.PI / 180f);
                    Vector3 projectedPoint = new Vector3(cameraRelative.x * SAZ / -cameraRelative.z, 0, cameraRelative.y * SELV / -cameraRelative.z);

                    if (Mathf.Abs(projectedPoint.x) < 1 && Mathf.Abs(projectedPoint.y) < 1 && Mathf.Abs(projectedPoint.z) < 1)
                    {
                        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.parent = displayLeft.transform;
                        sphere.transform.localPosition = new Vector3(projectedPoint.x * displayScaleA, projectedPoint.y * displayScaleE, 0);
                        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    }
                }
            }
        }
    }
}
