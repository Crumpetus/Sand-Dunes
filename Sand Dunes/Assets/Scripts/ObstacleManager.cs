using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour {

	public TerrainManager m_tManager;
	private int m_width;
	private int m_height;
	private bool[,] m_obstacleMap;
	public bool m_readyToUpdate = false;
	[SerializeField]
	private List<Obstacle> m_obstacles;
	private float m_distanceBetweenPoints;
	public Obstacle CubeObstacle;
	public Obstacle CylinderObstacle;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown("o"))
		{
			CreateCylinder();
		}

		if (Input.GetKeyDown("c"))
		{
			CreateCuboid();
		}
		if (Input.GetKeyDown("r"))
		{
			ResetMap();
		}
		if ((Input.GetKeyDown("space")) && (m_readyToUpdate))
		{
			RefreshObstacleMap();
			m_readyToUpdate = false;
		}
	}

	public void ResetMap()
	{
		foreach (Obstacle it in m_obstacles)
		{
			Destroy(it.gameObject);
		}
		m_obstacles.Clear();
		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				m_obstacleMap[i, j] = false;
			}
		}
	}

	public void CreateCylinder()
	{
		Vector3 SpawnPos = new Vector3(Random.Range(0.0f, m_width * m_distanceBetweenPoints), 17.0f, Random.Range(0.0f, m_height * m_distanceBetweenPoints));
		Obstacle NewObstacle = Instantiate(CylinderObstacle);
		NewObstacle.InitialiseObstacle(m_obstacles.Count, SpawnPos, this);
		m_obstacles.Add(NewObstacle);
		//RefreshObstacleMap();
		m_readyToUpdate = true;
	}

	public void CreateCuboid()
	{
		Vector3 SpawnPos = new Vector3(Random.Range(0.0f, m_width * m_distanceBetweenPoints), 17.0f, Random.Range(0.0f, m_height * m_distanceBetweenPoints));
		Obstacle NewObstacle = Instantiate(CubeObstacle);
		NewObstacle.InitialiseObstacle(m_obstacles.Count, SpawnPos, this);
		m_obstacles.Add(NewObstacle);
		//RefreshObstacleMap();
		m_readyToUpdate = true;
	}

	public bool CheckCoordinate(int x, int y)
	{
		return m_obstacleMap[y, x];
	}

	public void InitialiseMap(int _width, int _height, float _distanceBetweenPoints)
	{
		m_width = _width;
		m_height = _height;
		m_distanceBetweenPoints = _distanceBetweenPoints;
		m_obstacleMap = new bool[m_width, m_height];
		m_obstacles = new List<Obstacle>();
		RefreshObstacleMap();
	}

	public void RefreshObstacleMap()
	{
		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				Vector3 CastPos = new Vector3(i * m_distanceBetweenPoints, -1.5f, j * m_distanceBetweenPoints);
				if (Physics.Raycast(CastPos, Vector3.up, 8.0f))
				{
					m_obstacleMap[i, j] = true;
					//Debug.Log("found one!");
					m_tManager.ForceSetCoord(j, i, 0.0f);
				}
				else
				{
					m_obstacleMap[i, j] = false;
				}
			}
		}
	}
}
