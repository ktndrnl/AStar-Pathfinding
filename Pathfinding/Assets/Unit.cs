using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	private const float pathUpdateMoveThreshhold = 0.5f;
	private const float minPathUpdateTime = 0.2f;
	
	public Transform target;

	public float speed = 20f;
	public float turnSpeed = 3f;
	public float turnDst = 5f;
	public float stoppingDst = 10f;

	private Path path;
	
	private void Start()
	{
		StartCoroutine(UpdatePath());
	}

	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
	{
		if (pathSuccessful)
		{
			path = new Path(waypoints, transform.position, turnDst, stoppingDst);
			StopCoroutine(FollowPath());
			StartCoroutine(FollowPath());
		}
	}

	IEnumerator UpdatePath()
	{
		if (Time.timeSinceLevelLoad < 0.3f)
		{
			yield return new WaitForSeconds(0.3f);
		}
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);

		float sqrMoveThreshold = pathUpdateMoveThreshhold * pathUpdateMoveThreshhold;
		Vector3 targetPosOld = target.position;
		
		while (true)
		{
			yield return new WaitForSeconds(minPathUpdateTime);
			if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
			{
				PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
				targetPosOld = target.position;
			}
		}
	}

	private IEnumerator FollowPath()
	{
		bool followingPath = true;
		int pathIndex = 0;
		transform.LookAt(path.lookPoints[0]);

		float speedPercent = 1f;
		
		while (followingPath)
		{
			Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
			while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
			{
				if (pathIndex == path.finishLineIndex)
				{
					followingPath = false;
					break;
				}
				else
				{
					pathIndex++;
				}
			}

			if (followingPath)
			{
				if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
				{
					speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
					if (speedPercent < 0.01f)
					{
						followingPath = false;
					}
				}

				Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
				transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
			}
			yield return null;
		}
	}

	private void OnDrawGizmos()
	{
		path?.DrawWithGizmos();
	}
}
