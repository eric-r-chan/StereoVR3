using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Globalization;


public class IMGViewer : MonoBehaviour {


    public GameObject Camera;
    public GameObject Rover;

    public string IMGURL = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/02120/FLB_585689057RADLF0720202FHAZ00206M1.IMG";


    float AZIMUTH_FOV;
    float ELEVATION_FOV;
    Vector3 CAMERA_POS;
    float INSTRUMENT_AZIMUTH;
    float INSTRUMENT_ELEVATION;
    bool LOCAL_INSTRUMENTS = true;
    Vector3 ORIGIN_OFFSET_VECTOR;
    Quaternion ORIGIN_ROTATION_QUATERNION;

	// Use this for initialization
	void Start ()
    {
        //Regex parts = new Regex(@"^\d+\t(\d+)\t.+?\t(item\\[^\t]+\.ddj)");

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
                CAMERA_POS = new Vector3 (float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
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
                                          
                /*
                ORIGIN_ROTATION_QUATERNION = new Quaternion(
                    float.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
                    float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                          float.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                          float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture)
                          );*/
            }

        }




        Debug.Log("Instrument Azimuth, Instrument Elevation");
        Debug.Log(INSTRUMENT_AZIMUTH);
        Debug.Log(INSTRUMENT_ELEVATION);


        Camera.transform.localPosition = CAMERA_POS;
        //Camera.transform.localRotation = Quaternion.Euler(-INSTRUMENT_ELEVATION, INSTRUMENT_AZIMUTH, 0);

        Rover.transform.rotation = ORIGIN_ROTATION_QUATERNION;
        //Quaternion newQuat = new Quaternion(-.01f, -.16f, -.13f, .969f);
        //Rover.transform.rotation = newQuat;
        //Debug.Log("Quaternion");
        //Debug.Log(newQuat);
        Rover.transform.position = ORIGIN_OFFSET_VECTOR;

        Camera.transform.localEulerAngles = new Vector3(-INSTRUMENT_ELEVATION, INSTRUMENT_AZIMUTH, 0);



        //Camera.transform.rotation = Quaternion.Euler(30, 90, 0);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
