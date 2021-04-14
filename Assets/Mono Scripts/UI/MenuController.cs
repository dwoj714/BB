using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public static MenuController current;

    //re-enable "Fallback" menu when *this* menu is closed (pressing back button, resuming paused game, etc)
    protected MenuController FallbackMenu { get; private set; }



    public virtual void Enter()
	{
        gameObject.SetActive(true);
        FallbackMenu?.gameObject.SetActive(false);
	}

    public virtual void Enter(MenuController fallback)
    {
        FallbackMenu = fallback;
        Enter();
    }

    public virtual void Exit()
	{
        gameObject.SetActive(false);
        FallbackMenu.gameObject.SetActive(true);
	}
}
