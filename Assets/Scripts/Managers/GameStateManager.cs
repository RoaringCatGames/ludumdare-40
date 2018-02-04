using System;
using UnityEngine;

[Serializable]
public enum TreeTypeKey
{
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
}