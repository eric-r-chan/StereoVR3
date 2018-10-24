using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureLoad : MonoBehaviour
{
    Material m_Material;
    public string url = "https://s3-us-gov-west-1.amazonaws.com/msl-stereo-images/N_L000_0606_ILT031PER_S_1256_UNCORM1.png";


	// Use this for initialization
	/*void Awake ()
    {
        m_Material = GetComponent<Renderer>().material;
        m_Material.mainTexture = LoadPNG("/Users/ericchan/Downloads/testimg.png");
	}*/
    /*
    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
    */





    IEnumerator Start()
    {
        // Start a download of the given URL
        using (WWW www = new WWW(url))
        {
            // Wait for download to complete
            yield return www;

            // assign texture
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = www.texture;
        }
    }
}


