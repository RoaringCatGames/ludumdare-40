using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FileImageComponent : MonoBehaviour {
    
  [HideInInspector]
  public string filePath;  

  private Image imageComponent;

  void Awake(){
    imageComponent = GetComponent<Image>();
    if(!string.IsNullOrEmpty(filePath)){
      ApplyImage(filePath);
    }
  }

  public Texture2D GetTexture() {
    return imageComponent.sprite.texture;
  }

  public void ApplyImage(Texture2D t2d) {
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

    Sprite screenshotSprite = Sprite.Create(t2d, new Rect(0, 0, t2d.width, t2d.height), new Vector2(0, 0), 100f);

    imageComponent.sprite = screenshotSprite;
  }

  public void ApplyImage(string _filePath){
    this.filePath = _filePath;
    Texture2D t2d = CachedResource.LoadFile(filePath);
    
    this.ApplyImage(t2d);
  }
}