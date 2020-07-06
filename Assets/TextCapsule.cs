using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(ContentSizeFitter))]
public class TextCapsule : MonoBehaviour
{

	[Header("Behaviour")]
	[SerializeField] private Vector2 padding = Vector2.zero;


	[Header("References")]
	[SerializeField] private RectTransform rect;

	private TextMeshProUGUI text;
	private ContentSizeFitter csf;

	// Start is called before the first frame update
	void Start()
	{
		text = GetComponent<TextMeshProUGUI>();
		csf = GetComponent<ContentSizeFitter>();
	}

	private void OnRectTransformDimensionsChange()
	{

		//return immediately if neither text nor csf are assigned to avoid missing reference exceptions
		if (!text || !csf) return;

		float x, y;

		if (csf.horizontalFit == ContentSizeFitter.FitMode.PreferredSize)
		{
			x = ((RectTransform)transform).sizeDelta.x + padding.x;
		}
		else
		{
			x = rect.sizeDelta.x;
		}

		if (csf.verticalFit == ContentSizeFitter.FitMode.PreferredSize)
		{
			y = ((RectTransform)transform).sizeDelta.y + padding.y;
		}
		else
		{
			y = rect.sizeDelta.y;
		}

		rect.sizeDelta = new Vector2(x, y);
	}
}
