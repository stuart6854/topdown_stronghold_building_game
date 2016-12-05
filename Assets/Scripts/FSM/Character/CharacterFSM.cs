using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterFSM : FSM {

    private IEnumerator State;

    private Character Character;

    public CharacterFSM(Character character) : base() {
        this.Character = character;
    }

    public override IEnumerator run() {
        this.State = Idle();

        while(base.isRunning) {
            yield return WorldController.Instance.StartCoroutine(State);

	        yield return null;
        }

        Debug.LogError("Character Behaviour has Stopped!");
    }

    private IEnumerator Idle() {
//        Debug.Log("Going Idle.");

        float JobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
        float lastTime = 0;

        bool ChangedState = false;
        while(!ChangedState) {
            float passedTime = Time.time - lastTime;
            lastTime = Time.time;
            JobSearchCooldown -= passedTime;

            //TODO: Lots of checks to determine our next move/state, eg. Hungry = GetFoodState or CanGetJob = DoJobState, etc.

            if(Character.GetCurrentJob() == null && JobController.Instance.HasJob()) {
                if(JobSearchCooldown <= 0) {
                    this.State = GetJob();
                    ChangedState = true;
                }
            } else if(Character.GetCurrentJob() != null) {
                this.State = Job();
                ChangedState = true;
            }

			yield return null;
		}

        //TODO: Random Movement

        //We have changed state, so have broken out the loop
//        Debug.Log("I am no longer Idle.");
    }

    private IEnumerator GetJob() {
        yield return null;

//        Debug.Log("Trying to get a Job.");

        if(Character.GetJob()) {
            State = Job();
            Debug.Log("Got a Job. Going to work.");
        } else {
            State = Idle();
            Debug.Log("No Jobs. Going to Idle.");
        }
    }

    private IEnumerator Job() {
        yield return null;

//        Debug.Log("Starting my Job.");

        //TODO: Do Job. Conditional Loop may be required

        Job currentJob = Character.GetCurrentJob();
		
        float lastTime = 0;
        while(Character.GetCurrentJob() != null) {

			Tile currTile = Character.GetCurrentTile();
			if(!Character.HasJobRequirements()) {
				if(Character.GetCurrentPath() == null) {
					KeyValuePair<string, int> requirement = Character.GetUnfulfilledJobRequirement();
//					Debug.Log("a");

					List<Tile> path = PathfindingController.Instance.RequestPathToObject(currTile, requirement.Key);
					if(path == null) {
						//We could find one of the requirements so this job is unable to be completed just now.
						//So lets abandon and requeue the job
						Character.AbandonJob();
						State = Idle();
//						Debug.Log("Couldn't find requirement: " + requirement.Value + " " + requirement.Key);
						yield break;
					}
					Character.SetCurrentJobRequirement(requirement.Key);
					Character.SetPath(path);
				} else {
					if(!PathfindingController.Instance.PathStillValid(Character.GetCurrentPath())) {
//						Debug.Log("b");
						Character.SetPath(null);
					} else if(currTile == Character.GetDestinationTile()) {
//						Debug.Log("c");
						//We have reached a requirements location, hopefully
						string requirment = Character.GetCurrentJobRequirement();
						LooseItem item = currTile.GetLooseItem();
						if(item == null) {
//							Debug.Log("-1");
//							Debug.LogError("This requirement has been taken!");
							Character.SetCurrentJobRequirement(null);
							Character.SetPath(null); // Reset
							continue;
						}

						int amnt = item.GetStackSize();
						if(amnt < Character.GetJobRequirements()[requirment]) {
							//Not enough of what we need. We'll pick it up anyway and look for more
//							Debug.Log("d");
							Character.GetInventory().Add(new LooseItem(requirment, amnt));
							currTile.GetLooseItem().RemoveFromStack(amnt);
							Character.GetJobRequirements()[requirment] -= amnt;
							Character.SetCurrentJobRequirement(null);
							Character.SetPath(null); // Resets us to try find more of requirement as we have not fullfilled the required amount
						} else {
							//We can fullfil the whole remaining requirment here
//							Debug.Log("e");
							Character.GetInventory().Add(new LooseItem(requirment, Character.GetJobRequirements()[requirment]));
							currTile.GetLooseItem().RemoveFromStack(Character.GetJobRequirements()[requirment]);
							Character.GetJobRequirements().Remove(requirment); // We dont require any more of this type
							Character.SetCurrentJobRequirement(null);
							Character.SetPath(null);
						}
					}
				}
	        } else {
                Tile destTile = currentJob.GetTile();
                Character.SetDestination(currentJob.GetTile());

				if(currTile == currentJob.GetTile() || currTile.GetNeighbourTiles().Contains(currentJob.GetTile())) {
//					Debug.Log("f");
					currentJob.DoJob(Time.deltaTime);
				} else {
//				    Debug.Log("g");
					if(Character.GetCurrentPath() == null) {
						List<Tile> path = PathfindingController.Instance.RequestPath(currTile, destTile);
//					    Debug.Log("h");
						if(path == null) {
							Character.AbandonJob();
							State = Idle();
//						    Debug.Log("Could find Path to job!");
							yield break;
						}

						path.RemoveAt(path.Count - 1);
						Character.SetPath(path);
					} else if(!PathfindingController.Instance.PathStillValid(Character.GetCurrentPath())) {
						Character.SetPath(null);
//					    Debug.Log("j");
					}
				}
			}

            yield return null;
        }

        this.State = Idle();
//        Debug.Log("Completed my Job. Going to Idle.");

    }

}
