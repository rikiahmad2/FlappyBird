using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour {

	private const float Camera_Ortho_Size = 50f;
	private const float Pipe_Width = 25f;
	private const float Collider_Width = 8f;
	private const float pipe_Move_Speed = 30f;
	private List<Pipe> pipeList;
	private const float BatasDestroy = -165f;
	private const float pipe_Spawn_X = +100f;
	private const float Bird_X_Position = 0f;
	private static Level instance;
	private State state;


	
	public static Level GetInstance(){
		return instance;

	}

	private float pipeSpawnTimer;
	private int pipesSpawned;
	private float pipeSpawnTimerMax;
	private float gapSize;
	private int pipesPassedCount;

	private enum State{
		WaitingToStart,
		Playing,
		BirdDead,
	}

	public enum Difficulty{
		Easy,
		Medium,
		Hard,
		Impossible,
	}

	private void Awake() {
		instance = this;
		pipeList = new List<Pipe>();
		pipeSpawnTimerMax = 1.5f;
		SetDifficulty(Difficulty.Easy);
		state = State.WaitingToStart;
	}

	private void Start() {

		Bird.GetInstance().OnDied += Bird_OnDied;
		Bird.GetInstance().OnStartedPlaying += Bird_OnStartedPlaying;

	}

	private void Bird_OnStartedPlaying(object sender, System.EventArgs e){
		state = State.Playing;
	}

	private void Bird_OnDied(object sender, System.EventArgs e){
		CMDebug.TextPopupMouse("DEAD!");
		state = State.BirdDead;
	}

	private void Update() {

		if (state == State.Playing)
		{
			HandlePipeMovement();
			HandlePipeSpawning();
		}
	}

	
	private void HandlePipeSpawning() {

		pipeSpawnTimer -= Time.deltaTime;
		if(pipeSpawnTimer < 0) {

			// Time to spawn another pipe
			pipeSpawnTimer += pipeSpawnTimerMax;

			float heightEdgeLimit = 10f;
			float minHeight = gapSize * .5f + heightEdgeLimit;
			float totalHeight = Camera_Ortho_Size * 2f;
			float maxHeight = totalHeight - gapSize * .5f - heightEdgeLimit;

			float height = Random.Range(minHeight, maxHeight);
			CreateGapPipes(height, gapSize, pipe_Spawn_X);

		}
	}

	private void HandlePipeMovement() {

		for (int i=0; i<pipeList.Count; i++) {
			Pipe pipa = pipeList[i];
			//Menggerakan pipa
			bool isToTheRightOfBird = pipa.GetXPosition() > Bird_X_Position;
			pipa.Move();
			if (isToTheRightOfBird && pipa.GetXPosition() <= Bird_X_Position && pipa.IsBottom())
			{
				//Pipe Melewati Burung
				pipesPassedCount++;
			}
			//jika melebihi posisi yang ditentukan pipe di destroy

			if(pipa.GetXPosition() < BatasDestroy)
			{
				pipa.DestroySelf();
				pipeList.Remove(pipa);
				i--;
			}

		}

	}

	private void SetDifficulty(Difficulty difficulty){
		switch (difficulty)
		{
			case Difficulty.Easy:
				gapSize = 50f;
				pipeSpawnTimerMax = 1.5f;
				break;
			case Difficulty.Medium:
				gapSize = 40f;
				pipeSpawnTimerMax = 1.4f;
				break;
			case Difficulty.Hard:
				gapSize = 33f;
				pipeSpawnTimerMax = 1.3f;
				break;
			case Difficulty.Impossible:
				gapSize = 25f;
				pipeSpawnTimerMax = 1.1f;
				break;
		}
	}

	private Difficulty getDifficulty(){
		if (pipesSpawned >= 30) return Difficulty.Impossible;
		if (pipesSpawned >= 20) return Difficulty.Hard;
		if (pipesSpawned >= 10) return Difficulty.Medium;
		return Difficulty.Easy;
	}

	private void CreateGapPipes(float gapY, float gapSize, float xPosition) {

		// Jalankan Fungsi Pipe bawah
		CreatePipe(xPosition, gapY - gapSize * .5f,true);
		// Jalankan Fungsi Pipe atas
		CreatePipe2( xPosition, Camera_Ortho_Size * 2f - gapY - gapSize * .5f);
		pipesSpawned++;

		SetDifficulty(getDifficulty());

	}

	private void CreatePipe(float x, float y, bool createBottom) {

		// Set posisi Pipe Bawah
		Transform pipeFix = Instantiate(GameAssets.GetInstance().pfPipe);
		pipeFix.position = new Vector3(x, -Camera_Ortho_Size, 0f);


		SpriteRenderer pipeFixSpriteRenderer = pipeFix.GetComponent<SpriteRenderer>();
		pipeFixSpriteRenderer.size = new Vector2(Pipe_Width, y);

		BoxCollider2D pipeFixBoxCollider = pipeFix.GetComponent<BoxCollider2D>();
		pipeFixBoxCollider.size	  = new Vector2(Collider_Width, y);
		pipeFixBoxCollider.offset = new Vector2(0f, y * .5f);

		Pipe pipe = new Pipe(pipeFix, createBottom);
		pipeList.Add(pipe);

	}

	private void CreatePipe2(float x, float y) {
		// Set posisi Pipe Atas
		Transform pipeFix2 = Instantiate(GameAssets.GetInstance().pfPipe2);
		pipeFix2.position = new Vector3(x, Camera_Ortho_Size, 0f);


		SpriteRenderer pipeFix2SpriteRenderer = pipeFix2.GetComponent<SpriteRenderer>();
		pipeFix2SpriteRenderer.size = new Vector2(Pipe_Width, y);

		BoxCollider2D pipeFix2BoxCollider = pipeFix2.GetComponent<BoxCollider2D>();
		pipeFix2BoxCollider.size = new Vector2(Collider_Width, y);
		pipeFix2BoxCollider.offset = new Vector2(0f, y * .5f);

		Pipe pipe = new Pipe(pipeFix2,false);
		pipeList.Add(pipe);
	}

	public int GetPipesSpawned(){

		return pipesSpawned;
	}

	public int GetPipesPassedCount(){
		return pipesPassedCount;
	}

	// menunjukan satu pipe
	private class Pipe {

		private Transform pipeFixTransform;
		private bool isBottom;

		public Pipe(Transform pipeFixTransform, bool isBottom) {

			this.pipeFixTransform  = pipeFixTransform;
			this.isBottom = isBottom;
		}

		public void Move() {

			pipeFixTransform.position += new Vector3(-1, 0, 0) * pipe_Move_Speed * Time.deltaTime;
		}

		public float GetXPosition()
		{
			return pipeFixTransform.position.x;
		}

		public bool IsBottom()
		{
			return isBottom;
		}

		public void DestroySelf()
		{
			Destroy(pipeFixTransform.gameObject);
		}
	}
}
