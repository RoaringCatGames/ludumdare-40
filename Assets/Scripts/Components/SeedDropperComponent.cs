using UnityEngine;

public class SeedDropperComponent : MonoBehaviour
{
  void Start() {
    BranchManager.instance.PlantTree();
  }
}