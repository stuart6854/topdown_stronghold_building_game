using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public OptionsMenu OptionsMenu;

	public GameObject MainMenuPanel;
	public GameObject OptionsMenuPanel;

	public Transform ModListHolder;
	public GameObject ModListItemPrefab;

	private Animator MainMenuAnimator;
	private Animator OptionsAnimator;

	void Awake() {
		OptionsMenu.LoadSettings();
	}

	public void Start() {
		//Transitions
		MainMenuAnimator = MainMenuPanel.GetComponent<Animator>();
		OptionsAnimator = OptionsMenuPanel.GetComponent<Animator>();
		MainMenuAnimator.enabled = false;
		OptionsAnimator.enabled = false;

		//Setup
		RefreshMods();
	}

	#region Main Menu

	public void NewGame() {
		//TODO: Implement New Game Menu
		AsyncOperation SceneLoading = SceneManager.LoadSceneAsync("Game");
	}

	public void LoadGame() {
		NewGame(); //TEMPORARY

		//TODO: Implement Load Menu
	}

	#endregion

	#region Settings

	public void Options(bool In) {
		OptionsMenu.ResetOptionsMenu();

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

	#endregion

	public void ExitGame() {
		Application.Quit();
	}

}
