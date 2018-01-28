using System;
using UnityEngine;

namespace LitterBox.Utils
{
  public class Kitten
  {
    public static void Meow(params object[] inputs)
    {
      Debug.Log(String.Concat(inputs));
    }

    public static void Roar(params object[] inputs) {
      Debug.LogError(string.Concat(inputs));
    }
  }
}