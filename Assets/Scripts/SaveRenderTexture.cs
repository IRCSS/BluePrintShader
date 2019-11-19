using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;




public class SaveRenderTexture : MonoBehaviour {
 
    public enum OutPutType
    {
        JPEG, PNG
    };

    public static string FileName(string name, int width, int height, string format)
    {
        return string.Format(name +"_{0}x{1}_{2}."+format,
                              width, height,
                              System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    

    public static void Save(RenderTexture toSave, OutPutType format, string name)
    {
      
        if(toSave== null)
        {
            Debug.LogError("Attempt to save null renderTexture");
        }
        Texture2D screenShot = new Texture2D(toSave.width, toSave.height, TextureFormat.RGB24, false);
        RenderTexture.active = toSave;
        screenShot.ReadPixels(new Rect(0, 0, toSave.width, toSave.height), 0, 0);
        RenderTexture.active = null;
        byte[] bytes;
        string fileEnding = "";
        switch (format)
        {
            case OutPutType.JPEG:
                bytes = screenShot.EncodeToJPG();
                fileEnding = "jpeg";
                break;
            case OutPutType.PNG:
                bytes = screenShot.EncodeToPNG();
                fileEnding = "png";
                break;
            default:
                bytes = screenShot.EncodeToJPG();
                fileEnding = "jpeg";
                break;
        }


       
                string nameOfTheImage = FileName(name, toSave.width, toSave.height,fileEnding);
                string pathToTheFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name  + "/";
                if (!Directory.Exists(pathToTheFile))
                {
                    Directory.CreateDirectory(pathToTheFile);
                }

                string fileName = pathToTheFile + nameOfTheImage;
                System.IO.File.WriteAllBytes(fileName, bytes);
        Debug.Log("Saved file: " + nameOfTheImage + " in: " + fileName);
        
            }
    

}
