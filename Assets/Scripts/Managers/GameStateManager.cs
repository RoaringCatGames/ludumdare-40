using System;
using System.Linq;
using UnityEngine;

[Serializable]
public enum TreeTypeKey
{
    DEFAULT,
    SAKURA,
    APRICOT
}

public class GameStateManager : MonoBehaviour {

  public static GameStateManager instance;

  void Awake() {
    if(instance == null) {
      instance = this;

      DontDestroyOnLoad(gameObject);
    }else if(instance != this) {
      Destroy(gameObject);
    }
  }

  public TreeTypeKey TreeTypeKey = TreeTypeKey.SAKURA;
  public bool IsZenMode = false;

  public TreeTypeKeyedGameObject[] treeMap;
  public GameObject defaultTreePrefab;

  public TreeTypeKeyedColor[] colorMap;
  public Color defaultColor;


  public GameObject GetTreePrefab() {
    return GetTreePrefab(TreeTypeKey);
  }
  public GameObject GetTreePrefab(TreeTypeKey key) {
    TreeTypeKeyedGameObject pair = treeMap.FirstOrDefault((t) => t.Key == key);
    if (pair != null) {
      return pair.Entry;
    } else {
      return defaultTreePrefab;
    }
  }

  public Color GetColor() {
    return GetColor(TreeTypeKey);
  }

  public Color GetColor(TreeTypeKey key) {
    TreeTypeKeyedColor color = colorMap.FirstOrDefault((c) => c.Key == key);
    return color != null ? color.color : defaultColor;
  }
}