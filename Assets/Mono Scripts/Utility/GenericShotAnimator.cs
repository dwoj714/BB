using System.Collections;
using System;
using UnityEngine;

public class GenericShotAnimator : MonoBehaviour
{

    LauncherController launcher;
    Launchable shot;

    // Start is called before the first frame update
    void Start()
    {
        launcher = GetComponent<LauncherController>();

        launcher.ShotReadied += OnShotReadied;
        launcher.ShotHeld += OnShotHeld;
        launcher.ShotReleased += OnShotReleased;
    }

    public void OnShotReadied(object o, ShotEventArgs args)
	{
        shot = args.shot;
        shot.rb.MovePosition((Vector2)transform.position + GetOffset(args.direction, args.power));
	}

    private void OnShotHeld(object o, ShotEventArgs args)
    {
        shot.rb.MovePosition((Vector2)transform.position + GetOffset(args.direction, args.power));
    }

    private void OnShotReleased(object o, ShotEventArgs args)
    {
        StartCoroutine(AcceleratedRelease(args.shot, args.direction, args.power));
    }

    private Vector2 GetOffset(Vector2 dir, float power)
	{
        if (dir == Vector2.zero) dir = Vector2.up;

        return -dir * (power - 0.5f) * 2 * (LauncherRadius() - ShotRadius());
	}

    private float ShotRadius()
	{
        return shot.transform.lossyScale.x / 2;
	}

    private float LauncherRadius()
	{
        return launcher.transform.lossyScale.x / 2;
	}

    private IEnumerator AcceleratedRelease(Launchable shot, Vector2 direction, float power)
	{
        Vector2 startPos = shot.rb.position;
        Vector2 releasePos = (Vector2)transform.position + direction * (LauncherRadius() - ShotRadius());

        //the distance the shot will travel during the release animation at current charge
        float travelDistance = (startPos - releasePos).magnitude;

        //the distance the shot will travel during the release animation at maximum charge
        float maxTravelDistance = GetOffset(direction, 1).magnitude * 2;

        //the final speed when ending the acceleration phase
        float speed = shot.LaunchSpeed(power);

        // a = v^2 / (2 * (xf - xi))
        float acc = (speed * speed) / (2 * travelDistance);

        float progress = maxTravelDistance - travelDistance;
        float velocity = 0;

        while (progress < maxTravelDistance)
        {
            yield return null;

            velocity += acc * Time.deltaTime;
            progress += velocity * Time.deltaTime;

            if (progress > maxTravelDistance)
            {
                progress = maxTravelDistance;
            }

            shot.rb.MovePosition((Vector2)transform.position + GetOffset(direction, 1 - progress / maxTravelDistance));

        }

        shot.Launch(direction, power);
    }

}
