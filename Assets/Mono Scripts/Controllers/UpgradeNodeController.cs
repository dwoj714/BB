using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeNodeController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Button upgradeButton;
	[SerializeField] private Text priceText;
	[SerializeField] private Image upgradeProgress;
	[SerializeField] private RectTransform descriptionRect;
	[SerializeField] private RectTransform barRect;    //not the same as the upgradeProgress image's rectTransform
	public static UpgradeMenuController upgradeMenu;
	
	[Header("Upgrade Properties")]
	[SerializeField] private int upgradeIdx;
	[SerializeField] private int baseCost = 1;
	[SerializeField] private int costIncrement = 1;

	public LauncherController targetLauncher;


	private int currentLevel = 0;
	private int segID = Shader.PropertyToID("_seg");
	private int fillID = Shader.PropertyToID("_fill");
	private bool expanded = false;
	private Vector2 barSizeExpanded, barPosExpanded, nodeSizeExpanded, nodePosExpanded;
	private Vector2 barSizeCollapsed, barPosCollapsed, nodeSizeCollapsed, nodePosCollapsed;
	private bool initResize = true;
	private Material copyMaterial;
	private RectTransform nodeRect { get { return (RectTransform)transform; } }


	private const float animationDuration = 0.25f;
	
	private void Start()
	{
		priceText.text = Cost.ToString();
		upgradeButton.interactable = false;

		upgradeProgress.material = new Material(upgradeProgress.material);

		upgradeProgress.material.SetFloat(segID, MaxLevel());
		upgradeProgress.material.SetFloat(fillID, 0);

		Refresh();
	}

	private void OnEnable()
	{
		Refresh();
	}

	private void OnDisable()
	{
		if (expanded) ToggleExpanded();
	}

	public float ExpandedHeight
	{
		get
		{
			return nodeSizeExpanded.y;
		}
	}

	public float CollapsedHeight
	{
		get
		{
			return nodeSizeCollapsed.y;
		}
	}

	public float CurrentHeight
	{
		get
		{
			return nodeRect.sizeDelta.y;
		}
	}

	//Using the values set in the inspector, a button press will attempt to
	//purchase the given upgrade, and send it to the UpgradeTransmitter, which
	//causes the indicated UpgradeApplicator to increment the indicated upgrade (upgradeNum)
	public void PurchaseUpgrade()
	{
		if (CanUpgrade() && GameManager.main.SpendPoints(Cost))
		{
			IncrementUpgrade();
		}
	}

	private void OnHitMaxLevel()
	{
		priceText.text = "MAX";
		upgradeButton.interactable = false;
		ColorBlock colors = upgradeButton.colors;
		colors.disabledColor = new Color(0, 1, 0, 0.5f);
		upgradeButton.colors = colors;
	}

	private int Cost
	{
		get
		{
			return baseCost + currentLevel * costIncrement;
		}
	}

	private void CheckResize()
	{
		if (initResize)
		{
			initResize = false;

			nodeSizeCollapsed = nodeRect.sizeDelta;
			nodePosCollapsed = nodeRect.anchoredPosition;
			barSizeCollapsed = barRect.sizeDelta;
			barPosCollapsed = barRect.anchoredPosition;

			nodeSizeExpanded = nodeRect.sizeDelta + Vector2.up * descriptionRect.sizeDelta.y;
			nodePosExpanded = nodeRect.anchoredPosition;// + Vector2.down * descriptionRect.sizeDelta.y / 2;
			barSizeExpanded = barRect.sizeDelta - Vector2.right * ((RectTransform)upgradeButton.transform).sizeDelta.x;
			barPosExpanded = barRect.anchoredPosition - Vector2.right * ((RectTransform)upgradeButton.transform).sizeDelta.x / 2;

			descriptionRect.anchoredPosition = Vector2.zero;
		}
	}

	public void ToggleExpanded()
	{
		expanded = !expanded;

		CheckResize();
		
		if (expanded)
		{
			nodeRect.sizeDelta = nodeSizeExpanded;
			nodeRect.anchoredPosition = nodePosExpanded;
			barRect.sizeDelta = barSizeExpanded;
			barRect.anchoredPosition = barPosExpanded;
		}
		else
		{
			nodeRect.sizeDelta = nodeSizeCollapsed;
			nodeRect.anchoredPosition = nodePosCollapsed;
			barRect.sizeDelta = barSizeCollapsed;
			barRect.anchoredPosition = barPosCollapsed;
		}
	}

	public void AnimatedToggle()
	{
		CheckResize();
		StopAllCoroutines();
		StartCoroutine(ToggleExpandAnimated());
	}

	public IEnumerator ToggleExpandAnimated()
	{
		float timer = 0;
		float progress;
		
		//the max amount to change rect sizes and positions by
		Vector2 nodeDeltaSize, barDeltaSize;
		Vector2 nodeDeltaPos, barDeltaPos;

		//start rect sizes and positions
		Vector2 nodeStartSize = nodeRect.sizeDelta, barStartSize = barRect.sizeDelta;
		Vector2 nodeStartPos = nodeRect.anchoredPosition, barStartPos = barRect.anchoredPosition;

		//define rect size deltas
		if (expanded)
		{
			nodeDeltaSize = nodeSizeCollapsed - nodeRect.sizeDelta;
			barDeltaSize = barSizeCollapsed - barRect.sizeDelta;

			nodeDeltaPos = nodePosCollapsed - nodeRect.anchoredPosition;
			barDeltaPos = barPosCollapsed - barRect.anchoredPosition;
		}
		else
		{
			nodeDeltaSize = nodeSizeExpanded - nodeRect.sizeDelta;
			barDeltaSize = barSizeExpanded - barRect.sizeDelta;

			nodeDeltaPos = nodePosExpanded - nodeRect.anchoredPosition;
			barDeltaPos = barPosExpanded - barRect.anchoredPosition;
		}

		expanded = !expanded;

		//expand/collapse relevant rects over time
		while ((timer += Time.unscaledDeltaTime) < animationDuration)
		{
			progress = Mathf.Sqrt (Mathf.Pow( timer / animationDuration , 4));

			nodeRect.sizeDelta = nodeStartSize + nodeDeltaSize * progress;
			barRect.sizeDelta = barStartSize + barDeltaSize * progress;

			nodeRect.anchoredPosition = nodeStartPos + nodeDeltaPos * progress;
			barRect.anchoredPosition = barStartPos + barDeltaPos * progress;
			yield return null;
		}

		//Set the rects to the appropriate end sizes
		if (expanded)
		{
			nodeRect.sizeDelta = nodeSizeExpanded;
			barRect.sizeDelta = barSizeExpanded;

			nodeRect.anchoredPosition = nodePosExpanded;
			barRect.anchoredPosition = barPosExpanded;
		}
		else
		{
			nodeRect.sizeDelta = nodeSizeCollapsed;
			barRect.sizeDelta = barSizeCollapsed;

			nodeRect.anchoredPosition = nodePosCollapsed;
			barRect.anchoredPosition = barPosCollapsed;
		}
	}

	private bool CanUpgrade()
	{
		try
		{
			if (targetLauncher.UpgradeLevels[upgradeIdx] < targetLauncher.UpgradeLimits[upgradeIdx])
			{
				return true;
			}
			return false;
		}
		catch (System.IndexOutOfRangeException)
		{
			Debug.LogWarning(name + ": Attempting to get out of bounds upgrade index " + upgradeIdx);
			return false;
		}
	}

	private int MaxLevel()
	{
		return targetLauncher.UpgradeLimits[upgradeIdx];
	}

	private void IncrementUpgrade()
	{
		currentLevel++;
		priceText.text = Cost.ToString();

		//increment the launcher upgrade level
		targetLauncher.UpgradeLevels[upgradeIdx]++;
		upgradeProgress.material.SetFloat(fillID, currentLevel);

		Refresh();
		RefreshDownward();
		RefreshUpward();

		//if the upgrade puts us at max, disable the upgrade button, have it indicate max value
		if (!CanUpgrade())
		{
			OnHitMaxLevel();
		}
	}

	public void SetLauncherDownward(LauncherController launcher)
	{
		targetLauncher = launcher;

		foreach (UpgradeNodeController node in GetComponentsInChildren<UpgradeNodeController>())
		{
			if (node != this)
			{
				node.targetLauncher = launcher;
			}
		}

	}

	public void Refresh()
	{
		if (Cost <= GameManager.Points && CanUpgrade())
		{
			upgradeButton.interactable = true;
		}
		else
		{
			upgradeButton.interactable = false;
		}

		upgradeProgress.material.SetFloat(fillID, currentLevel);

		upgradeProgress.gameObject.SetActive(false);
		upgradeProgress.gameObject.SetActive(true);
	}

	public void RefreshDownward()
	{
		foreach (UpgradeNodeController node in GetComponentsInChildren<UpgradeNodeController>())
		{
			if(node != this)
			{
				node.Refresh();
				node.RefreshDownward();
			}
		}
	}

	public void RefreshUpward()
	{
		foreach (UpgradeNodeController node in GetComponentsInParent<UpgradeNodeController>())
		{
			if (node != this)
			{
				node.Refresh();
				node.RefreshUpward();
			}
		}
	}

	public void CollapseDownward()
	{
		if (expanded) ToggleExpanded();

		foreach (UpgradeNodeController node in GetComponentsInChildren<UpgradeNodeController>())
		{
			if (node != this && node.expanded)
			{
				node.ToggleExpanded();
			}
		}
	}
}
/*	UPGRADE LIST
*	
*	0-10: General-Use Levels
*	
*		0-2: LauncherController
*			0: Charge Speed
*			1: Recharge Speed
*			2: Energy Capacity
*		
*		3-5: ProjectileController (all projectiles get these)
*			3: Mass Increase
*			4: Launch speed increase
*			5: Life span
*		
*		6-9: ExplosiveProjecile:
*			6: Detonator damage
*			7: Detonator impulse
*			8: Detonator radius
*			9: Detonator status effect level (not yet implemented, but may be useful)
*		
*		10: StatusProjectile Effect level
*	
*	11+: Projectile-Specific levels
*	
*		Splitslug:
*			11: Explosive
*			12: Pellet/Bounce Count
*			13: Spread Tightness
*		
*		Flare:
*			10: DPS Level (StatusProjectile effect level)
*			11: Burn radius (BurnZone)
*			12: Radial damage (BurnZone)
*			
*		Sticky:
*			11: Auto detonate
*			12: Detonator power
*		
*		Grav:
*			11: Radius
*			12: Strength
*			
*		TimeBomb:
*			11:	Fuse Speed
*			12: Detonator power
*			13: Contact Detonation
*			
*		Pulsar:
*			11: Explosion count
*		
*/


