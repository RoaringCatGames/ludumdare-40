using System;
using System.Linq;
using UnityEngine;

public class ColorOptionsComponent : MonoBehaviour {

  public TreeTypeKeyedColor[] colorMap;
  public Color defaultColor;

  public Color GetColor(TreeTypeKey key) {
    TreeTypeKeyedColor color = colorMap.FirstOrDefault((c) => c.Key == key);
    return color != null ? color.color : defaultColor;
  }
}