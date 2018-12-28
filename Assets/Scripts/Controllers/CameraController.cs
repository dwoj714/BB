using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	GameManager manager;

	public struct CamPreset
	{
		public float size;
		public Vector3 position;

		public CamPreset(Vector3 pos, float s)
		{
			position = pos;
			size = s;
		}
	};

	public Dictionary<string, CamPreset> presets = new Dictionary<string, CamPreset>();

	Camera cam;
	bool moving;

	Vector3 targetPosition;
	float targetSize;

	public float transitionSpeed = 1;
	private float defaultTransitionSpeed;

	int framecount = 0;

	CollisionBorder border;

	// Use this for initialization
	void Start()
	{
		cam = GetComponent<Camera>();
		manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
		defaultTransitionSpeed = transitionSpeed;

		presets.Add("Menu", new CamPreset(transform.position, cam.orthographicSize));
		presets.Add("Game", new CamPreset(new Vector3(0,22,-100), 8));

		border = GetComponent<CollisionBorder>();

	}

	void Update()
	{
		if (moving)
		{
			framecount++;
			transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed);
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, transitionSpeed);

			if (Mathf.Approximately(cam.orthographicSize, targetSize))
			{
				moving = false;
				cam.orthographicSize = targetSize;
				OnDestinationReached();
			}
		}
	}

	void OnDestinationReached()
	{
		Debug.Log("Camera Reached Destination");
	}

	public void SetDestination(string name)
	{
		CamPreset target = presets[name];
		SetDestination(target.position, target.size);

		if(name == "Game")
		{
			manager.StartGame();
		}

	}

	public void SetDestination(Vector3 position, float size)
	{
		//Move the camera to its old destination and size, set the colliders for that destination,
		//and revert to what it was before transitioning to the new position
		Vector3 oldPos = transform.position;
		float oldSize = cam.orthographicSize;

		transform.position = position;
		cam.orthographicSize = size;
		border.Reposition();

		transform.position = oldPos;
		cam.orthographicSize = oldSize;


		moving = true;

		targetPosition = position;
		targetSize = size;

		framecount = 0;

		//transitionSpeed = defaultTransitionSpeed;
	}

	public void SetDestination(Vector3 position, float size, float speed)
	{
		moving = true;

		targetPosition = position;
		targetSize = size;

		transitionSpeed = speed;
	}
}
