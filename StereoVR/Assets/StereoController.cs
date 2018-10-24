using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StereoController : MonoBehaviour
{
    public GameObject LeftPlane;
    public GameObject RightPlane;


    Material LeftMaterial;
    Material RightMaterial;

    public string LeftURL = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/02120/NLB_585695355EDR_D0720310TRAV00730M2.IMG";
    public string RightURL = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/02120/NRB_585695355EDR_D0720310TRAV00730M2.IMG";


    void LoadImages()
    {
        StartCoroutine(Download());
    }

    IEnumerator Download()
    {
        Debug.Log("Loading Images");
        // Start a download of the given URL
        using (WWW www = new WWW(LeftURL))
        {
            // Wait for download to complete
            yield return www;

            // assign texture
            LeftPlane.GetComponent<Renderer>().material.mainTexture = www.texture;
        }

        using (WWW www = new WWW(RightURL))
        {
            // Wait for download to complete
            yield return www;

            // assign texture
            RightPlane.GetComponent<Renderer>().material.mainTexture = www.texture;
        
        }
    }

    public string stringToEdit = "Hello World";
    public Texture btnTexture;


    void OnGUI()
    {

        GUIStyle mobileStyle = new GUIStyle(GUI.skin.textField);
        mobileStyle.fontSize = 50;
        Font myFont = (Font)Resources.Load("Fonts/comic", typeof(Font));
        mobileStyle.font = myFont;
        mobileStyle.normal.textColor = Color.white;
        //mobileStyle.hover.textColor = Color.red;



        // Make a text field that modifies stringToEdit.
        LeftURL = GUI.TextField(new Rect(10, 150, 2800, 100), LeftURL, 200, mobileStyle);
        RightURL = GUI.TextField(new Rect(10, 270, 2800, 100), RightURL, 200, mobileStyle);



        if (!btnTexture)
        {
            Debug.LogError("Please assign a texture on the inspector");
            return;
        }

        if (GUI.Button(new Rect(500, 10, 300, 100), btnTexture))
        {
            LoadImages();
            Debug.Log("Clicked the button with an image");
        }
    }
}


