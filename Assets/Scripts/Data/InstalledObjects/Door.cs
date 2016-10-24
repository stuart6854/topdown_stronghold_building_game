using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : WorldObjectMethod {

	//TODO: Implement Door

    private const float DoorSpeed = 3.0f;

	public override void OnCreated(WorldObject worldObject) {
	    worldObject.SetParameter("IsOpening", false);
	    worldObject.SetParameter("Openness", 0f);
	}

	public override void OnUpdate(WorldObject worldObject) {
	    bool IsOpening = (bool)worldObject.GetParameter("IsOpening");
	    float Openness = (float)worldObject.GetParameter("Openness");

	    if(IsOpening) {
	        Openness += Time.deltaTime * DoorSpeed;
	        if(Openness >= 1.0f) {
	            worldObject.SetParameter("IsOpening", false);
	        }
	    } else {
	        Openness -= Time.deltaTime * DoorSpeed;
	    }

	    Openness = Mathf.Clamp01(Openness);
	    worldObject.SetParameter("Openness", Openness);

        ChangeSprite(Openness, worldObject);
	}

	public override void OnDestroyed(WorldObject worldObject) {
		throw new System.NotImplementedException();
	}

	public override Enterabilty GetEnterabilty(WorldObject worldObject) {
	    bool IsOpening = (bool)worldObject.GetParameter("IsOpening");
	    float Openness = (float)worldObject.GetParameter("Openness");

	    worldObject.SetParameter("IsOpening", true);

	    if(Openness >= 1f)
	        return Enterabilty.Enterable;

		return Enterabilty.Soon;
	}

    private void ChangeSprite(float openness, WorldObject worldObject) {
        if(openness < 0.1f) {
            SpriteController.Instance.SetSprite("door_0", worldObject);
        }else if(openness < 0.5f) {
            SpriteController.Instance.SetSprite("door_1", worldObject);
        }else if(openness < 0.9f){
            SpriteController.Instance.SetSprite("door_2", worldObject);
        }else{
            SpriteController.Instance.SetSprite("door_3", worldObject);
        }
    }

    public override BuildMethod GetBuildMethod() {
        return BuildMethod.Single;
    }

    public override Dictionary<string, int> GetConstructionRequirements() {
        return new Dictionary<string, int>() {
            { "wood", 2 }
        };
    }

}
