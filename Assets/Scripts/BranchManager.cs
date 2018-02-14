using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using LitterBox.Models;
using LitterBox.Utils;

public class BranchManager : MonoBehaviour
{

  public static BranchManager instance;
	public GameObject finalAnimation;
	

	private bool _isZenMode = false;
	private bool _isMarkedForReset = false;
  private List<BranchComponent> _branches = new List<BranchComponent>();
	private bool _hasGameEnded = false;

	private GameObject _uiRoot;	
	private GameObject _uiSocial;
	private GameObject _currentTree;


  // Use this for initialization
  void Awake()
  {
    if (instance == null)
    {
      instance = this;			
			
			_uiRoot = GameObject.Find("GameUI");
			_uiSocial = GameObject.Find("TwitterShareUI");		
			_isZenMode = GameStateManager.instance.IsZenMode;
			// DontDestroyOnLoad(instance.gameObject);
    }
    else if (this != instance)
    {
      Destroy(gameObject);
    }
  }

	void Update(){
		if(_isMarkedForReset){
			_isMarkedForReset = false;
			// Delete All Branches
			_branches.ForEach((branch) => {		
				if(branch != null) {
					Destroy(branch.gameObject);
				}
			});
			_branches.Clear();	
			_branches = new List<BranchComponent>();
			Destroy(_currentTree);

			// Start a new Root Tree
			PlantTree();
			// GameObject prefab = GameStateManager.instance.GetTreePrefab();			
			// _currentTree = Instantiate(prefab, new Vector3(0f, -2f, 0f), Quaternion.identity);
			// BranchComponent trunk = _currentTree.GetComponent<BranchComponent>();
			// trunk.SetZenMode(_isZenMode);

			//Set our flag to play again
			_hasGameEnded = false;
		}
	}
	
  // Update is called once per frame
  void LateUpdate()
  {
		if(!_hasGameEnded){
			if(!_isZenMode){
				bool collisionFound = _checkForCollisions();

				if(collisionFound){
					_hasGameEnded = true;
					// Trigger Final Flowering
					// Stop All Branches from Growing further
					_branches.ForEach((b) => b.isGrowing = false);

					// Clear all branch Selection
					foreach(var spi in FindObjectsOfType(typeof(SelectionPointInteractions))){
						if(spi is GameObject){
							Destroy(spi);
						}else if(spi is SelectionPointInteractions){
							Destroy(((SelectionPointInteractions)spi).gameObject);
						}
					}
				}
			}

			if(!_hasGameEnded){
				bool isStillGrowing = _isTreeStillGrowing();
				if(!isStillGrowing){
					_hasGameEnded = true;
				}
			}
		}
  }

	public void PlantTree() {
		PlantTree(GameStateManager.instance.TreeTypeKey);
	}

	public void PlantTree(TreeTypeKey key) {
			GameObject prefab = GameStateManager.instance.GetTreePrefab(key);//treePrefabs.FirstOrDefault((t) => t.Key == key);
			_currentTree = Instantiate(prefab, new Vector3(0f, -2f, 0f), Quaternion.identity);	
			BranchComponent trunk = _currentTree.GetComponentInChildren<BranchComponent>();
			trunk.SetZenMode(_isZenMode);	
	}

  public void AddBranch(BranchComponent branch)
  {
    _branches.Add(branch);
  }

	public bool IsGameOver(){
		return _hasGameEnded;
	}

	public void ResetScene(){
		_isMarkedForReset = true;
	}

	/**
	 * UI Management
	 **/
	public void ToggleUIEnabled(bool shouldShow){
		_uiRoot.GetComponent<Canvas>().enabled = shouldShow;
	}

	public void ToggleShareUI(bool shouldShow){
		string animationName = shouldShow ? "twitter-ui-enter" : "twitter-ui-leave";
		_uiSocial.GetComponent<Animator>().Play(animationName);
	}

	private bool _isTreeStillGrowing(){
		int growingBranchCount = _branches.Where((b) => b.isGrowing).Count();

		// If we are growing any branches, or any branch points are still active
		//	the game is still alive. Also if the first branch hasn't been added
		//	yet, we are still alive.
		return _branches.Count == 0 || growingBranchCount > 0 || FindObjectsOfType(typeof(SelectionPointInteractions)).Length > 0;
	}
	private bool _checkForCollisions(){
		bool shouldEndGame = false;
    // For every Branch that is growing, we need to check if it's crossing other branches.
    foreach (BranchComponent branch in _branches.Where((b) => b.isGrowing && b.LineSegments.Count > 0))
    {
      LineSegment latestSegment = branch.WorldLineSegments.Last();
      foreach (BranchComponent other in _branches)
      {
        if (branch != other && (branch.parentBranch == null || branch.parentBranch != other))
        {
          foreach (LineSegment line in other.WorldLineSegments)
          {
            // If the segments are the same origin, we assume
            //	this is where they branch off.
            bool areSameOrigin = latestSegment.StartPoint == line.StartPoint;
            bool doCross = !areSameOrigin && LineUtils.CrossProductIntersectTest(latestSegment, line);
            if (doCross)
            {              
							bool areCrossing = false;
							Vector2 point = LineUtils.GetIntersectionPointCoordinates(latestSegment.StartPoint, latestSegment.EndPoint, line.StartPoint, line.EndPoint, out areCrossing);
							// PLAY THE CRACK!
							SoundManager.instance.PlayBackgroundSfx(3);
							ZoomToPointComponent zoomer = gameObject.AddComponent<ZoomToPointComponent>();
							zoomer.targetPoint = point;
							//zoomer.timeToZoom = 0.75f;
							zoomer.pauseTime = 0.5f;
							
              shouldEndGame = true;

							// make sure branches aren't growing??
							branch.isGrowing = false;
							other.isGrowing = false;
							break;
            }
          }

					if(shouldEndGame){
						return shouldEndGame;
					}
        }
      }
    }
		return false;
	}
}
