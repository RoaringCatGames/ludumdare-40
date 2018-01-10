using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RCGManagement {
	public class RoaringCatGamesBrandManager : MonoBehaviour {

		private string siteUrl = "http://roaringcatgames.com";
		
		public void OpenRoaringCatGamesSite(){
			Logger.Log("Should Show Ad");
			AdsManager.instance.ShowAd((AdWatchState state) => {
				Logger.Log("State of Ad is: ", state);
			});
			Application.OpenURL(siteUrl);
		}
	}
}
