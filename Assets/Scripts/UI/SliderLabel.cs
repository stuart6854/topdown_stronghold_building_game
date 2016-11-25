using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderLabel : MonoBehaviour {
	
	public Text Label;

	private Slider Slider;

	private void Start() {
		this.Slider = GetComponent<Slider>();
		this.Slider.onValueChanged.AddListener(delegate { UpdateLabel(); });
		
		UpdateLabel();
	}

	private void UpdateLabel() {
		Label.text = Slider.value.ToString();
	}

}
