using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{
	[Header("Visuals")]
	public float maxLength = 2;
	public float minLength = 0;
	public float maxWidth, minWidth = 0.2f;
	public float speed = 0.5f;
	public bool colorTransition = false;
	public float transitionPoint = 0.875f;
	public Color mainColor, altColor;
	
	public int OrderInLayer
	{
		set
		{
			line.sortingOrder = value;
			tip.sortingOrder = value;
		}
	}

	[Header("References")]
	public GameObject lineObj;
	public GameObject tipObj;
	public LauncherController launcher;

	private LineRenderer line;
	private SpriteRenderer tip;
	private Vector3 baseScale;	//Initial scale of the tip sprite

	private bool armedLastFrame = false;

	private void Start()
	{
		line = lineObj.GetComponent<LineRenderer>();
		tip = tipObj.GetComponent<SpriteRenderer>();
		baseScale = tip.transform.localScale;
		SetColor(mainColor);
		SetVisible(false);
	}

	private void Update()
	{
		if (!armedLastFrame && launcher.Armed)
		{
			SetVisible(true);
		}

		if (launcher.Armed)
		{
			Aim(-launcher.Pull, launcher.PullPercentage);
		}

		if (armedLastFrame && !launcher.Armed)
		{
			SetVisible(false);
			line.SetPosition(0, Vector3.zero);
			tipObj.transform.localPosition = Vector2.zero;
		}
		armedLastFrame = launcher.Armed;
	}

	//Power should be between 0 and 1
	public void Aim(Vector2 direction, float power)
	{
		SetVisible(power > 0);

		float length = (maxLength - minLength) * power + minLength;

		Vector2 pos = direction.normalized * length;
		tip.transform.localPosition = Vector2.Lerp(tip.transform.localPosition, pos, speed);
		line.SetPosition(0, tip.transform.localPosition);
		tip.transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, tip.transform.localPosition));

		line.endWidth = line.startWidth = (maxWidth - minWidth) * (1 - power) + minWidth;

		Vector3 warp = Vector3.up * ((maxWidth - minWidth) * power + minWidth) + Vector3.right * ((maxWidth - minWidth) * (1 - power) + minWidth);
		tip.transform.localScale = baseScale.normalized/4 + warp.normalized/4;

		if(colorTransition)
			if(power >= transitionPoint)
			{
				SetColor(altColor);
			}
			else
			{
				SetColor(mainColor);
			}

	}

	public void SetColor(Color col)
	{
		line.endColor = line.startColor = col;
		tip.color = col;
	}

	public void SetVisible(bool visible)
	{
		line.enabled = tip.enabled = visible;
	}

}
