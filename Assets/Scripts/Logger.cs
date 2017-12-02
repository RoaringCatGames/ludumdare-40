using System;
using UnityEngine;

public class Logger {
  public static void Log(params object[] inputs){
    MonoBehaviour.print(String.Concat(inputs));
  }
}