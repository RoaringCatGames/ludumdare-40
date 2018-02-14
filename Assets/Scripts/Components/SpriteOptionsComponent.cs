using System;
using System.Linq;
using UnityEngine;

public class SpriteOptionsComponent : MonoBehaviour {

  public TreeTypeKeyedSprite[] spriteMap;
  public Sprite defaultSprite;

  public Sprite GetSprite(TreeTypeKey key) {
    TreeTypeKeyedSprite keyedSprite = spriteMap.FirstOrDefault((c) => c.Key == key);
    return keyedSprite != null ? keyedSprite.sprite : defaultSprite;
  }
}