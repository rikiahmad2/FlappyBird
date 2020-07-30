using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour
{
	[SerializeField]
	private GameObject OverWindow;
	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private Button_UI retryBtn;

	private void Awake()
	{
		retryBtn.ClickFunc = () => { Loader.load(Loader.Scene.GameScene); };
		Hide();
	}

	private void Start(){
		Bird.GetInstance().OnDied += Bird_OnDied;
	}

	private void Bird_OnDied(object sender, System.EventArgs e)
	{
		scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
		Show();
	}

	private void Hide()
	{
		OverWindow.SetActive(false);
	}

	private void Show()
	{
		OverWindow.SetActive(true);
	}
}