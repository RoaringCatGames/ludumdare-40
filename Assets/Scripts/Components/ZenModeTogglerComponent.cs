using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ZenModeTogglerComponent : MonoBehaviour {

  void Start() {
    GetComponent<Toggle>().isOn = GameStateManager.instance.IsZenMode;
  }
}