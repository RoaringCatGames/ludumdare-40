using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileImageComponent : MonoBehaviour {
    
  private string filePath;  
  private Image imageComponent;

  void Awake(){
    imageComponent = GetComponent<Image>();
  }

  public void ApplyImage(string filePath){
    Texture2D t2d = this._loadTexture(filePath);
    RectTransform rect = imageComponent.transform as RectTransform;
    // Retain Ratio    
    float aspectRatio = (float)t2d.width/(float)t2d.height;   
    bool isWidthBased = aspectRatio >= 1f;
    float newWidth = rect.rect.width;
    float newHeight = rect.rect.height;

    if(isWidthBased){
      newHeight = newWidth/aspectRatio;
    }else{
      newWidth = newWidth * aspectRatio;
    }
    rect.sizeDelta = new Vector2(newWidth, newHeight);

    Sprite screenshotSprite = new Sprite();
    screenshotSprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0, 0), 100f);

    imageComponent.sprite = screenshotSprite;
  }

  private Texture2D _loadTexture(string FilePath)
  {

    // Load a PNG or JPG file from disk to a Texture2D
    // Returns null if load fails
    Texture2D Tex2D;
    byte[] FileData;

    if (File.Exists(FilePath))
    {
      FileData = File.ReadAllBytes(FilePath);
      Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
      if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
        return Tex2D;                 // If data = readable -> return texture
    }
    return null;                     // Return null if load failed
  }
}