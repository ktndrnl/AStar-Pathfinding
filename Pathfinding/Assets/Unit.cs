using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
	public Transform target;

	public float speed = 20f;
	public float turnDst = 5f;

	private Path path;
	
	private void Start()
	{
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
	}

	public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
	{
		if (pathSuccessful)
		{
			path = new Path(waypoints, transform.position, turnDst);
			StopCoroutine(FollowPath());
			StartCoroutine(FollowPath());
		}
	}

	private IEnumerator FollowPath()
	{
		while (true)
		{
			yield return null;
		}
	}

	private void OnDrawGizmos()
	{
		path?.DrawWithGizmos();
	}
}
