using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {

	//Game
	[Header("Game")]

	//Graphics
	[Header("Graphics")]
	public OptionSelector ResolutionSelector;
	public OptionSelector WindowModeSelector;

	//Sound
	[Header("Sound")]
	public Slider Main;
	public Slider Game;
	public Slider Music;

	//PlayerPref Keys
	private static readonly string DISPLAY_WIDTH = "display_width";
	private static readonly string DISPLAY_HEIGHT = "display_height";
	private static readonly string DISPLAY_FULLSCREEN = "display_fullscreen";

	private static readonly string SOUND_MAIN = "sound_main";
	private static readonly string SOUND_GAME = "sound_game";
	private static readonly string SOUND_MUSIC = "sound_music";
	

	void Start () {
		//Resolution Options
		Resolution[] supportedResolutions = Screen.resolutions;
		foreach(Resolution resolution in supportedResolutions) {
			string option = resolution.width + "x" + resolution.height;
			if(ResolutionSelector.Contains(option))
				continue;

			ResolutionSelector.AddOption(option);
		}

		ResetOptionsMenu();
	}

	public void LoadSettings() {
		int width = PlayerPrefs.GetInt(DISPLAY_WIDTH);
		int height = PlayerPrefs.GetInt(DISPLAY_HEIGHT);
		bool isFullscreen = (PlayerPrefs.GetInt(DISPLAY_FULLSCREEN) == 1 ? true : false);
		Screen.SetResolution(width, height, isFullscreen);
	}

	public void ApplySettings() {
		//Resolution
		string res = ResolutionSelector.GetValue();
		int width = int.Parse(res.Substring(0, res.IndexOf("x", StringComparison.Ordinal)));
		int height = int.Parse(res.Substring(res.IndexOf("x", StringComparison.Ordinal)));
		PlayerPrefs.SetInt(DISPLAY_WIDTH, width);
		PlayerPrefs.SetInt(DISPLAY_HEIGHT, height);

		//Window Mode
		int isFullscreen = (WindowModeSelector.GetValue() != "Windowed") ? 1 : 0;
		PlayerPrefs.SetInt(DISPLAY_FULLSCREEN, isFullscreen);

		//Main Volume
		float mainVolume = Main.value;
		PlayerPrefs.SetFloat(SOUND_MAIN, mainVolume);
		//Game Volume
		float gameVolume = Game.value;
		PlayerPrefs.SetFloat(SOUND_GAME, gameVolume);
		//Music Volume
		float musicVolume = Music.value;
		PlayerPrefs.SetFloat(SOUND_MUSIC, musicVolume);

		//Do this Last!
		PlayerPrefs.Save();
	}

	public void ResetOptionsMenu() {
		ResetResolutionSelector();
		ResetWindowModeSelector();
	}

	private void ResetResolutionSelector() {
		Resolution res = Screen.currentResolution;
		string option = res.width + "x" + res.height;
		ResolutionSelector.SetSelectedOption(option);
	}

	private void ResetWindowModeSelector() {
		bool isFullscreen = Screen.fullScreen;
		WindowModeSelector.SetSelectedIndex((isFullscreen ? 1 : 0)); // Options should be 0="Windowed" then 1="Borderless Windowed"
	}

}
