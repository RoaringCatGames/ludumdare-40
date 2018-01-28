using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRendererComponent : MonoBehaviour {

	public RuntimePlatform[] platformsToHide;

	// Use this for initialization
	void Start () {
		if(platformsToHide != null && platformsToHide.Length > 0){
			foreach(RuntimePlatform p in platformsToHide) {
				if(p == Application.platform) {
					gameObject.SetActive(false);
				}
			}
		}
	}
}
