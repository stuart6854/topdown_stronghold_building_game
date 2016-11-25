using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public GameObject MainMenuPanel;
	public GameObject OptionsMenuPanel;

	public Transform ModListHolder;
	public GameObject ModListItemPrefab;

	private Animator MainMenuAnimator;
	private Animator OptionsAnimator;

	public void Start() {
		//Transitions
		MainMenuAnimator = MainMenuPanel.GetComponent<Animator>();
		OptionsAnimator = OptionsMenuPanel.GetComponent<Animator>();
		MainMenuAnimator.enabled = false;
		OptionsAnimator.enabled = false;

		//Setup
		RefreshMods();
	}

	#region Settings

	public void Options(bool In) {
		MainMenuAnimator.enabled = true;
		OptionsAnimator.enabled = true;

		if(In) {
			MainMenuAnimator.Play("MainMenuSlideOut");
			OptionsAnimator.Play("OptionsMenuSlideIn");
		} else {
			MainMenuAnimator.Play("MainMenuSlideIn");
			OptionsAnimator.Play("OptionsMenuSlideOut");
		}
	}

	#region Sound

	public void UpdateVolumeLabel(Text lbl, Slider slider) {
		
	}

	#endregion

	#region Mods

	public void RefreshMods() {
		//Remove Mods from list
		foreach(Transform child in ModListHolder) {
			Destroy(child.gameObject);
		}
		//Reload mods
		ModManager.LoadMods();
		//Create ListItem for each mod
		foreach(Mod mod in ModManager.Mods) {
			GameObject listItem = Instantiate(ModListItemPrefab);
			listItem.name = mod.AboutInfo.Name + "_ModLstItm";
			listItem.transform.SetParent(ModListHolder);

			Text text = listItem.GetComponentInChildren<Text>();
			text.text = mod.AboutInfo.Name;
		}
	}

	public void OpenModFolder() {
		string path = Application.streamingAssetsPath + "/Mods/";
		path = path.Replace(@"/", @"\");
		System.Diagnostics.Process.Start("explorer.exe", "" + path);
	}

	#endregion

	public void ApplySettings() {
		
	}

	#endregion

	public void ExitGame() {
		Application.Quit();
	}

}
