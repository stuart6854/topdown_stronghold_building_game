using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AnimHandler {

	public static Dictionary<string, Anim> Animations = new Dictionary<string, Anim>();

	private WorldObject ToAnimate; //Object which is being animated

	private Anim Animation;

	private float TimeThisFrame; // In Seconds

	public AnimHandler(string animName, WorldObject toAnimate) {
		this.ToAnimate = toAnimate;
		this.Animation = AnimHandler.GetAnim(animName);

		ToAnimate.IsAnimated = true;
	}

	public void TickAnim(float deltaTime) {
		if(Animation == null) {
			ToAnimate.IsAnimated = false;
			return;
		}

		TimeThisFrame += deltaTime;

		if(TimeThisFrame >= Animation.GetFrameDuration()) {
			Animation.NextFrame();
			string spriteName = Animation.GetFrameSpriteName();
			SpriteController.Instance.SetSprite(spriteName, ToAnimate);
			TimeThisFrame = 0;
		}
	}

	public static void AddAnim(string name, Anim anim) {
		if(Animations.ContainsKey(name)) {
			Debug.LogError("AnimHandler::AddAnim -> Already have animation with name: " + name);
			return;
		}

		Animations.Add(name, anim);
	}

	public static Anim GetAnim(string animName) {
		if(!Animations.ContainsKey(animName)) {
			Debug.LogError("static AnimHandler::GetAnim -> No Animation with name: " + animName);
			return null;
		}

		return Animations[animName];
	}
	
}
