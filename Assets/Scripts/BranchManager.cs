﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BranchManager : MonoBehaviour
{

  public static BranchManager instance;
	public GameObject finalAnimation;
	public GameObject treePrefab;

	public bool isZenMode = false;

	private bool _isMarkedForReset = false;
  private List<BranchComponent> _branches = new List<BranchComponent>();
	private bool _hasGameEnded = false;

	//private GameObject runningFinalBG;

  // Use this for initialization
  void Awake()
  {
    if (instance == null)
    {
      instance = this;

			if(treePrefab != null){
				Instantiate(treePrefab, new Vector3(0f, -2f, 0f), Quaternion.identity);
			}

			// LineSegment AB = new LineSegment(-1, -1, 1, 1);
			// LineSegment CD = new LineSegment(-1, 1, 1, -1);

			// bool doCross = LineUtils.CrossProductIntersectTest(AB, CD);

			// LineSegment EF = new LineSegment(-1, -1, -1, 1);
			// LineSegment GH = new LineSegment(1, 1, 1, -1);

			// bool secondDoCross = LineUtils.CrossProductIntersectTest(EF, GH);

			// Logger.Log("AB->CD Cross: ", doCross, " EF->GH Cross: ", secondDoCross);
    }
    else if (this != instance)
    {
      Destroy(gameObject);
    }
    //DontDestroyOnLoad(gameObject);
  }

	void Update(){
		if(_isMarkedForReset){
			_isMarkedForReset = false;
			// Delete All Branches
			_branches.ForEach((branch) => {
				Destroy(branch.gameObject);
			});
			_branches.Clear();			
			// Start a new Root Tree
			Instantiate(treePrefab, new Vector3(0f, -2f, 0f), Quaternion.identity);

			// if(runningFinalBG != null){
			// 	Destroy(runningFinalBG);
			// }
			//Set our flag to play again
			_hasGameEnded = false;
		}
	}
	
  // Update is called once per frame
  void LateUpdate()
  {
		if(!_hasGameEnded){
			if(!isZenMode){
				bool collisionFound = _checkForCollisions();

				if(collisionFound){
					_hasGameEnded = true;
					// Trigger Final Flowering
					// Stop All Branches from Growing further
					_branches.ForEach((b) => b.isGrowing = false);
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
					//runningFinalBG = Instantiate(finalAnimation, new Vector3(), Quaternion.identity);
				}
			}
		}
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
