using System.Collections;
using UnityEngine;

public class AnimationDispatcher : MonoBehaviour
{
	[Header("Animation Components")]
	[SerializeField] private int holderIDX;
	[SerializeField] private Vector2 shotOffset = Vector2.zero;
	[SerializeField] private PullbackAnimator[] subTargets;
	[SerializeField] private float releaseDuration, retractPause, releaseResetDuration = 0.1f;

	[Header("Launcher sprite rotation")]
	[SerializeField] private float targetRotateSpeed = 0.5f;
	[SerializeField] private float rotationResetDelay = 0.6f;
	[SerializeField] private Transform target;

	[Header("Animation Cancelling")]
	[SerializeField] private bool allowFullCancel = false;

	//for launching the shot if an AcceleratedRelease is animation canceled
	private Launchable shot;
	private Vector2 shotDirection;
	private float shotPower;

	public bool AnimateShot
	{
		get
		{
			return holderIDX < 0;
		}
	}

	public PullbackAnimator MainAnim
	{
		get
		{
			if (holderIDX < 0) return null;
			return subTargets[holderIDX];
		}
	}

	private LauncherController launcher;
	private Vector2 direction;
	private float resetTimer = 0;

	private void Start()
	{
		launcher = GetComponent<LauncherController>();
	}

	private void Update()
	{
		resetTimer += Time.deltaTime;
		if (launcher.Armed)
		{
			direction = Vector2.Lerp(direction, -launcher.Pull.normalized * LauncherController.InvertFactor, targetRotateSpeed);
			target.localEulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);

			//send pull info to each animated component
			foreach (PullbackAnimator t in subTargets)
			{
				t.SetPull(launcher.PullPercentage);
			}
			resetTimer = 0;
		}

		if(resetTimer > rotationResetDelay)
		{
			direction = Vector2.Lerp(direction, Vector2.up, targetRotateSpeed / 5);
			target.localEulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, direction);
		}
	}

	//animate the components at linear velovity
	private IEnumerator LinearRelease()
	{
		// a list of the starting position percentages before being released
		float[] startPercentages = new float [subTargets.Length];
		for(int i = 0; i < startPercentages.Length; i++)
			startPercentages[i] = subTargets[i].PositionToPercentage(subTargets[i].transform.localPosition, false);

		// a list of the changes in each position percentage throughout the release
		float[] deltaPercentages = new float[startPercentages.Length];
		for (int i = 0; i < deltaPercentages.Length; i++)
			deltaPercentages[i] = 1 - startPercentages[i];

		//within the releaseDuration time frame, move the component from current position to max release position
		float timer = 0;
		while (timer < releaseDuration)
		{
			for (int i = 0; i < subTargets.Length; i++)
			{
				//make sure timer doesn't exceed the time limit
				timer = Mathf.Clamp(timer + Time.deltaTime, 0, releaseDuration);

				float newPercentage = startPercentages[i] + deltaPercentages[i] * (timer / releaseDuration);
				//Debug.Log(anim.name + " set percentage to " + newPercentage);
				subTargets[i].SetPositionPercentage(newPercentage);
			}
						
			yield return null;
		}

		//wait a bit
		yield return new WaitForSeconds(retractPause);

		yield return ReturnToRestPos();

	}

	//animate the components at accelerating velocity, moving the shot along with the MainAnim
	public IEnumerator AcceleratedRelease(Launchable shot, Vector2 direction, float power)
	{
		//hold onto the shot reference in case the animation is interrupted
		this.shot = shot;
		this.shotDirection = direction;
		this.shotPower = power;

		//world space start and end positions for calculating acceleration
		Vector2 globalStartPos = MainAnim.transform.position;
		Vector2 globalEndPos = MainAnim.transform.parent.TransformPoint(MainAnim.targetReleasePos);

		//the world space distance to be travelled while accelerating
		float travelDistance = (globalEndPos - globalStartPos).magnitude;
		float maxTravelDistance = MainAnim.PositionRangeGlobal.magnitude;

		//Debug.Log(travelDistance + " --Max-> " + maxTravelDistance);

		//the final speed when ending the acceleration phase
		float speed = shot.LaunchSpeed(power);

		// a = v^2 / (2 * (xf - xi))
		float acc = (speed * speed) / (2 * travelDistance);

		//Debug.Log("From (" + globalStartPos.x + ", " + globalStartPos.y + ") to (" + globalEndPos.x + ", " + globalEndPos.y + ") - Distance: " + travelDistance + ". Acc: " + acc);

		float progress = maxTravelDistance - travelDistance;
		float velocity = 0;

		//Debug.Log(progress);
		while (progress < maxTravelDistance)
		{
			yield return null;

			velocity += acc * Time.deltaTime;
			progress += velocity * Time.deltaTime;

			if (progress > maxTravelDistance)
			{
				progress = maxTravelDistance;
			}

			for (int i = 0; i < subTargets.Length; i++)
			{
				subTargets[i].SetPositionPercentage(progress / maxTravelDistance);
			}

			shot.transform.position = ShotPosition;
		}

		//Launch the shot, and acknowledge that it's been launched
		shot.Launch(direction, power);
		this.shot = null;

		yield return new WaitForSeconds(retractPause);

		yield return ReturnToRestPos();
	}

	//if we're in the middle of launching a shot, don't allow animation cancelling, and return false.
	//othewise (if the main component is returning to restPos), allow for calcels.
	public bool CancelAnimation()
	{
		if (shot && !allowFullCancel)
		{
			return false;
		}
		else if (shot)
		{
			shot.Launch(shotDirection, shotPower);
		}
		StopAllCoroutines();

		return true;
	}

	private IEnumerator ReturnToRestPos()
	{

		float[] startPercentages = new float[subTargets.Length];
		float[] deltaPercentages = new float[subTargets.Length];

		//set motion parameters
		for (int i = 0; i < startPercentages.Length; i++)
		{
			startPercentages[i] = subTargets[i].CurrentPercentage;
		}
		for (int i = 0; i < deltaPercentages.Length; i++)
		{
			deltaPercentages[i] = subTargets[i].RestPercentage - startPercentages[i];
		}

		float timer = 0;
		//within the releaseResetDuration time frame, move the components back to resting position
		while (timer < releaseResetDuration)
		{
			for (int i = 0; i < subTargets.Length; i++)
			{
				//make sure timer doesn't exceed the time limit
				timer = Mathf.Clamp(timer + Time.deltaTime, 0, releaseResetDuration);

				float newPercentage = startPercentages[i] + deltaPercentages[i] * (timer / releaseResetDuration);
				subTargets[i].SetPositionPercentage(newPercentage);
			}

			yield return null;
		}
	}

	public void OnShotLaunched()
	{
		StartCoroutine(LinearRelease());
	}

	public void OnShotLaunched(Launchable shot, Vector2 direction, float power)
	{
		StartCoroutine(AcceleratedRelease(shot, direction, power));
	}

	public Vector2 ShotPosition
	{
		get
		{
			if (!MainAnim) return transform.position;
			return MainAnim.transform.position + MainAnim.transform.right * shotOffset.x + MainAnim.transform.up * shotOffset.y;
		}
	}

}
