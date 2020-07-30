using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using System;

public class Bird : MonoBehaviour {

	private const float JUMP_AMOUNT = 100f;
	private Rigidbody2D birdRigidBody2D;

	private static Bird instance;
	float time;
	float pause;
	public EventHandler OnDied;
	public EventHandler OnStartedPlaying;
	private State state;
	public AudioSource audiofx;
	public Animator bird_anim;
	public static Bird GetInstance(){
		return instance;
	}

	private enum State{
		WaitingToStart,
		Playing,
		Dead
	}

	private void Awake()
	{
		instance = this;
		birdRigidBody2D = GetComponent<Rigidbody2D>();
		birdRigidBody2D.bodyType = RigidbodyType2D.Static;
		state = State.WaitingToStart;
	}

	private void Update()
	{
		switch (state){
			default:
			case State.WaitingToStart:
				if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){
					state = State.Playing;
					birdRigidBody2D.bodyType = RigidbodyType2D.Dynamic;
					audiofx.Play();
					Jump();
					Invoke("close", 0.5f);
					if (OnStartedPlaying != null) OnStartedPlaying(this, EventArgs.Empty);
				}
				break;
			case State.Playing:
				if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)){
					audiofx.Play();
					Jump();

					Invoke("close", 0.5f);
				}
				break;
			case State.Dead:
				break;
		}
	}

	private void Jump()
	{
		birdRigidBody2D.velocity = Vector2.up * JUMP_AMOUNT;
		bird_anim.SetBool("jumped", true);
	}

	private void close()
	{
		Debug.Log("This is Close !");
		bird_anim.SetBool("jumped", false);
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		Debug.Log("test");
		birdRigidBody2D.bodyType = RigidbodyType2D.Static;
		if (OnDied != null) OnDied(this, EventArgs.Empty);
	}
}
