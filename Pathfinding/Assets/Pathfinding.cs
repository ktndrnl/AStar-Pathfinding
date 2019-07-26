using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour
{
	public Transform seeker, target;
	
	private Grid grid;

	private void Awake()
	{
		grid = GetComponent<Grid>();
	}

	private void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			FindPath(seeker.position, target.position);
		}
	}

	private void FindPath(Vector3 startPos, Vector3 tartgetPos)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(tartgetPos);

		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node currentNode = openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if (currentNode == targetNode)
			{
				sw.Stop();
				print("Path found: " + sw.ElapsedMilliseconds + " ms");
				RetracePath(startNode, targetNode);
				return;
			}

			foreach (Node neighbor in grid.GetNeighbors(currentNode))
			{
				if (!neighbor.walkable || closedSet.Contains(neighbor))
				{
					continue;
				}

				int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
				if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
				{
					neighbor.gCost = newMovementCostToNeighbor;
					neighbor.hCost = GetDistance(neighbor, targetNode);
					neighbor.parent = currentNode;
					if (!openSet.Contains(neighbor))
					{
						openSet.Add(neighbor);
					}
				}
			}
		}
	}

	private void RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		
		path.Reverse();

		grid.path = path;
	}

	private int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY);
		}

		return 14 * dstX + 10 * (dstY - dstX);
	}
}
