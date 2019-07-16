using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	GameManager manager;

	public float shakeIntensity;
	public float shakeLimit = 0.1f;

	[SerializeField] private Material shaker;

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

	CollisionBorder border;

	// Use this for initialization
	void Start()
	{
		cam = GetComponent<Camera>();
		manager = GameObject.Find("Game Manager").GetComponent<GameManager>();

		presets.Add("Menu", new CamPreset(transform.position, cam.orthographicSize));
		presets.Add("Game", new CamPreset(new Vector3(0,21.8f,-100), 8));
		presets.Add("Loadout", new CamPreset(new Vector3(0, 11, -100), 8));

		border = GetComponent<CollisionBorder>();
	}

	void Update()
	{
		if (moving)
		{
			transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed);
			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, transitionSpeed);

			if (Mathf.Approximately((transform.position - targetPosition).sqrMagnitude, 0))
			{
				moving = false;
				cam.orthographicSize = targetSize;
				OnDestinationReached();
			}
		}

		if (shakeIntensity > shakeLimit) shakeIntensity = shakeLimit;

		shakeIntensity = Mathf.Lerp(shakeIntensity, 0, 0.2f);

	}

	void OnDestinationReached()
	{
		Debug.Log("Camera Reached Destination");
	}

	public void SetDestination(string name)
	{
		CamPreset target = presets[name];
		SetDestination(target.position, target.size);
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
	}

	public void SetDestination(Vector3 position, float size, float speed)
	{
		moving = true;

		targetPosition = position;
		targetSize = size;

		transitionSpeed = speed;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		shaker.SetFloat("_mag", shakeIntensity);
		Graphics.Blit(source, destination, shaker);
	}

}
