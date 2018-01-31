using System;
using UnityEngine;

namespace LitterBox.Models {

  [Serializable]
  public class KeyedGameObject {
    public string Key;
    public GameObject Entry;
  }
}