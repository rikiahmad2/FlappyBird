using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreWindow : MonoBehaviour {

	private Text scoreText;


	// Use this for initialization
	private void Awake()
	{
		scoreText = transform.Find("scoreText").GetComponent<Text>();
	}

	private void Update(){
		scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
	}
}
