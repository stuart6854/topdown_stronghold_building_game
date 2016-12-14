using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim {

	private AnimFrame[] Frames;
	private int CurrentFrame = 0;

	public Anim(string[] spriteNames, float[] frameDurations) {
		InitFrames(spriteNames, frameDurations);
	}

	private void InitFrames(string[] spriteNames, float[] frameDurations) {
		Frames = new AnimFrame[spriteNames.Length];
		for(int i = 0; i < spriteNames.Length; i++) {
			AnimFrame frame = new AnimFrame(spriteNames[i], frameDurations[i]);
			Frames[i] = frame;
		}
	}

	public void NextFrame() {
		CurrentFrame++;
		if(CurrentFrame >= Frames.Length)
			CurrentFrame = 0;
	}

	public int GetFrameIndex() {
		return CurrentFrame;
	}

	public string GetFrameSpriteName() {
		return Frames[CurrentFrame].SpriteName;
	}

	public float GetFrameDuration() {
		return Frames[CurrentFrame].Duration;
	}

	private class AnimFrame {

		public string SpriteName { get; private set; }
		public float Duration { get; private set; } // In Seconds

		public AnimFrame(string spriteName, float duration) {
			this.SpriteName = spriteName;
			this.Duration = duration;
		}

	}

}
