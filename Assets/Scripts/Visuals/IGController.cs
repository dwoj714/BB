using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The purpose of this is to control graphics feedback to the user regarding what their pointer is doing
public class IGController : MonoBehaviour {

	public LauncherController launcher;

	public GameObject field;
	public GameObject point;

	private SpriteRenderer fSprite, pSprite;

	private bool armedLastFrame = false;
	private Vector3 lastPos;

	private void Start()
	{
		fSprite = field.GetComponent<SpriteRenderer>();
		pSprite = point.GetComponent<SpriteRenderer>();
		field.transform.localScale = (Vector3.right + Vector3.up) * launcher.maxDragLength * 2;

		SetVisible(false);
	}

	private void Update ()
	{
		if(!armedLastFrame && launcher.Armed())
		{
			SetVisible(true);

			Vector3 newPos = (Vector2)Camera.main.ScreenToWorldPoint(lastPos);

			transform.position = newPos;

		}

		if (launcher.Armed())
		{
			point.transform.localPosition = launcher.mouseDrag;
			fSprite.material.SetFloat("_charge", launcher.ChargePercentage());
			fSprite.material.SetFloat("_drag", launcher.mouseDrag.magnitude / launcher.maxDragLength);
		}

		if (armedLastFrame && !launcher.Armed())
		{
			SetVisible(false);
		}

		armedLastFrame = launcher.Armed();
		lastPos = Input.mousePosition;
	}

	private void SetVisible(bool visible)
	{
		fSprite.enabled = pSprite.enabled = visible;
	}
}
