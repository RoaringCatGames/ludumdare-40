using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigationController : MonoBehaviour {

	public string sceneName = "MenuScreen";
	
	public void TransitionToScene(){
		SceneManager.LoadScene(sceneName);
	}
}
