using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullbackAnimator : MonoBehaviour
{
	[Header("Constraints")]
	public Vector2 maxPullPos, targetReleasePos = Vector2.zero;

	[Header("Other stuff")]
	public float rotationScale = 0;
	[SerializeField] private float lerp = 0.2f;
	[HideInInspector] public Vector2 restPos;

	private Vector2 targetPos;

	private bool atTarget = true;

	private Vector2 PositionRange
	{
		get
		{
			return maxPullPos - targetReleasePos;
		}
	}

	public Vector2 PositionRangeGlobal
	{
		get
		{
			return transform.parent.TransformPoint(maxPullPos) - transform.parent.TransformPoint(targetReleasePos);
		}
	}

	//percentage being a 0-1 range of vectors between maxPullPos and targetReleasePos,
	//0 being maxPullPos, and 1 being targetReleasePos
	public float CurrentPercentage
	{
		get
		{
			return PositionToPercentage(transform.localPosition, false);
		}
		set
		{
			SetPositionPercentage(value);
		}
	}

	public float MaxPercentage
	{
		get
		{
			return PositionToPercentage(targetReleasePos, false);
		}
	}

	public float MinPercentage
	{
		get
		{
			return PositionToPercentage(maxPullPos, false);
		}
	}

	public float RestPercentage
	{
		get
		{
			return PositionToPercentage(restPos, false);
		}
	}

	private void Start()
	{
		restPos = transform.localPosition;
	}

	private void Update()
	{
		//if we're not at the target, lerp to it.
		if (!atTarget)
		{
			transform.localPosition = Vector2.Lerp(transform.localPosition, targetPos, lerp);
			if((Vector2)transform.localPosition == targetPos)
			{
				atTarget = true;
			}
		}
	}

	//set the position using a 0-1 range from restPos to maxPullPos
	public void SetPull(float pull, bool lerp = true)
	{
		Vector2 rangedOffset = (restPos - maxPullPos) * -pull;
		if (lerp)
		{
			targetPos = restPos + rangedOffset;
			atTarget = false;
		}
		else
		{
			transform.localPosition = targetPos = restPos + rangedOffset;
			atTarget = true;
		}
	}

	public void SetPositionPercentage(float percent)
	{
		transform.localPosition = targetPos = maxPullPos - PositionRange * percent;
		atTarget = true;
	}


	public float PositionToPercentage(Vector2 position, bool worldSpace)
	{
		//if given a world space position, convert it to local space
		if (worldSpace)
		{
			position = transform.InverseTransformPoint(position);
		}

		//PositionRange is a vector that points from maxPullPos to targetReleasePos
		//The following conversion converts 'position' to a vector that points from maxPullPos to 'position'
		position -= maxPullPos;

		//project position onto the line between min and max positions
		//position = PositionRange * Vector2.Dot(position, PositionRange.normalized);
		position *= Mathf.Cos(Mathf.Deg2Rad * Vector2.Angle(PositionRange, position));

		return position.magnitude / PositionRange.magnitude;

	}

}
