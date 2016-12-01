using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour {

	public Button NextButton;
	public Button BackButton;
	public Text Label;

	public List<string> Options;

	private int SelectedIndex = 0;

	void Start () {
		NextButton.onClick.AddListener(NextOption);
		BackButton.onClick.AddListener(BackOption);

		UpdateControl();
	}

	public void NextOption() {
		if(SelectedIndex + 1 >= Options.Count)
			return;

		SetSelectedIndex(SelectedIndex + 1);
	}

	public void BackOption() {
		if(SelectedIndex - 1 < 0)
			return;

		SetSelectedIndex(SelectedIndex - 1);
	}

	public void SetSelectedIndex(int index) {
		this.SelectedIndex = index;

		UpdateControl();
	}

	public void SetSelectedOption(string option) {
		if(!Options.Contains(option))
			return;

		SetSelectedIndex(Options.IndexOf(option));
	}

	private void UpdateControl() {
		if(Options.Count > 0)
			Label.text = Options[SelectedIndex];

		if(SelectedIndex <= 0)
			BackButton.interactable = false;
		else
			BackButton.interactable = true;
		
		if(SelectedIndex >= Options.Count - 1)
			NextButton.interactable = false;
		else
			NextButton.interactable = true;
	}

	public void AddOption(string option) {
		Options.Add(option);
	}

	public bool Contains(string option) {
		return Options.Contains(option);
	}

	public string GetValue() {
		return Options[SelectedIndex];
	}

}
