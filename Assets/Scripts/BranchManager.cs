﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BranchManager : MonoBehaviour
{

  public static BranchManager instance;

	public bool isZenMode = false;

  private List<BranchComponent> branches = new List<BranchComponent>();
	private bool hasGameEnded = false;

  // Use this for initialization
  void Awake()
  {
    if (instance == null)
    {
      instance = this;

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
    DontDestroyOnLoad(gameObject);
  }

  // Update is called once per frame
  void LateUpdate()
  {
		if(!hasGameEnded){
			if(!isZenMode){
				bool collisionFound = checkForCollisions();

				if(collisionFound){
					hasGameEnded = true;
					// Trigger Final Flowering
					// Stop All Branches from Growing further
					branches.ForEach((b) => b.isGrowing = false);
					// TODO: Play SFX

				}
			}

			if(!hasGameEnded){
				bool isStillGrowing = isTreeStillGrowing();
				if(!isStillGrowing){
					Logger.Log("Tree is Not Growing anymore");
					hasGameEnded = true;
				}
			}
		} else {
			Logger.Log("Game is Over");
		}
  }

  public void AddBranch(BranchComponent branch)
  {
    branches.Add(branch);
  }

	public bool IsGameOver(){
		return hasGameEnded;
	}

	public void ResetScene(){
		//TODO: RESET IT ALL!!
	}

	private bool isTreeStillGrowing(){
		int growingBranchCount = branches.Where((b) => b.isGrowing).Count();

		// If we are growing any branches, or any branch points are still active
		//	the game is still alive. Also if the first branch hasn't been added
		//	yet, we are still alive.
		return branches.Count == 0 || growingBranchCount > 0 || FindObjectsOfType(typeof(SelectionPointInteractions)).Length > 0;
	}
	private bool checkForCollisions(){
		bool shouldEndGame = false;
    // For every Branch that is growing, we need to check if it's crossing other branches.
    foreach (BranchComponent branch in branches.Where((b) => b.isGrowing && b.LineSegments.Count > 0))
    {
      LineSegment latestSegment = branch.WorldLineSegments.Last();
      foreach (BranchComponent other in branches)
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
              Logger.Log("LINES CROSS AT: ", latestSegment, line);
							bool areCrossing = false;
							Vector2 point = LineUtils.GetIntersectionPointCoordinates(latestSegment.StartPoint, latestSegment.EndPoint, line.StartPoint, line.EndPoint, out areCrossing);
							GameObject prefab = Resources.Load("Prefabs/Animated Leaf") as GameObject;
							GameObject go = Instantiate(prefab, new Vector3(point.x, point.y, 0f), Quaternion.identity);
							go.transform.localScale *= 4f;
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