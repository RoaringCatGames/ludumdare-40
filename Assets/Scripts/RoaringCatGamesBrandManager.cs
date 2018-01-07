using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoaringCatGamesBrandManager : MonoBehaviour {

	private string siteUrl = "http://roaringcatgames.com";
	
	public void OpenRoaringCatGamesSite(){
		Application.OpenURL(siteUrl);
	}
}
