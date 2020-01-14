using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuseVisualizer : MonoBehaviour
{
	[SerializeField] private VisibleAt visibleAt = VisibleAt.FuseSpark;
	private Detonator det;
	private SpriteRenderer spr;
	private bool visible = false;
	private Vector3 scale;

	private int meterID = Shader.PropertyToID("_charge");

	// Start is called before the first frame update
    void Start()
    {
		det = GetComponent<Detonator>();
		if (!det) det = transform.parent.GetComponent<Detonator>();

		spr = GetComponent<SpriteRenderer>();

		scale = transform.localScale;

		if (visibleAt == VisibleAt.FuseSpark)
		{
			transform.localScale = Vector3.zero;
		}
		
    }

    // Update is called once per frame
    void Update()
    {
        if(!visible && det.sparked)
		{
			StartCoroutine(MakeVisible());
			visible = true;
		}
		if (visible)
		{
			spr.material.SetFloat(meterID, det.FusePercent);
		}

    }

	private IEnumerator MakeVisible()
	{
		bool done = false;
		while (!done)
		{
			transform.localScale = Vector3.Lerp(transform.localScale, scale, 0.2f);
			if(Mathf.Approximately(transform.localScale.x, scale.x))
			{
				done = true;
			}
			yield return null;
		}
	}

}

enum VisibleAt { Start, FuseSpark};
