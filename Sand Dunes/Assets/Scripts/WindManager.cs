using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WindManager : MonoBehaviour {

	[SerializeField]
	protected float[,] m_windHeights;                   //the height of the wind at each point
	[SerializeField]
	protected Vector3[,] m_windMap;                     //the directional vectors of the wind at each point
	[SerializeField]
	protected float[,] m_strengthMap;					//the strength of the wind at each point
	protected int m_width;								//the y size of the terrain
	protected int m_height;                             //the y size of the terrain
	[SerializeField]
	protected float m_distanceBetweenPoints;            //the distance in world units between 2 points
	private float m_maxDistance;						//the maximum distance a point can be from a diagonal point
	public WindSource WindSource;                       //the windsource prefab
	public float m_strength;
	private List<WindSource> m_sources;

	//public WindManager(int _width, int _height, float _terrainDistance, float[,] _terrainHeights)
	//{
	//	InitialiseWindMap(_width, _height, _terrainDistance, _terrainHeights);
	//}

	public Vector3 getWindVector(int _x, int _y)
	{
		return m_windMap[_x, _y];
	}

	public float getWindStrength(int _x, int _y)
	{
		return m_strengthMap[_x, _y];
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("n"))
		{
			AddNewWindSource();
			//RefreshAverageDirection();
		}
		if (Input.GetKey("space"))
		{
			UpdateWindMap();
		}

		if (Input.GetKeyDown("r"))
		{
			ResetMap();
		}
	}

	public void InitialiseWindMap (int _width, int _height, float _terrainDistance, float[,] _terrainHeights)
	{
		//initialise vector map and wind strength map
		m_width = _width;
		m_height = _height;
		Debug.Log("m_width = " + m_width.ToString() + "m_height = " + m_height.ToString());
		m_distanceBetweenPoints = _terrainDistance;
		Debug.Log("m_distanceBetweenPoints = " + m_distanceBetweenPoints.ToString());
		m_maxDistance = Mathf.Sqrt(m_distanceBetweenPoints * m_distanceBetweenPoints * 2);
		Debug.Log("m_maxDistance = " + m_maxDistance.ToString());
		m_windHeights = new float[_width, _height];
		m_windHeights = _terrainHeights;
		Debug.Log("m_windHeights Length = " + m_windHeights.Length.ToString() + "FirstInstance = " + m_windHeights[125, 125].ToString());
		m_sources = new List<WindSource>();
		AddNewWindSource();
		m_windMap = new Vector3[_width, _height];
		m_strengthMap = new float[_width, _height];
		RefreshAverageDirection();
		Debug.Log("m_windMap Length = " + m_windMap.Length.ToString() + "FirstInstance = " + m_windMap[125, 125].ToString());
		Debug.Log("m_strengthMap Length = " + m_strengthMap.Length.ToString() + "FirstInstance = " + m_strengthMap[125, 125].ToString());
		//UpdateWindMap();
		//for (int i = 0; i < width; i++)
		//{
		//	for (int j = 0; j < height; j++)
		//	{

		//	}
		//}
	}

	public void ResetMap()
	{

		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				m_windMap[i, j] = Vector3.zero;
				m_strengthMap[i, j] = 0.0f;
			}
		}
		foreach (WindSource it in m_sources)
		{
			Destroy(it.gameObject);
		}
		m_sources.Clear();
		AddNewWindSource();
	}

	void UpdateWindMap ()
	{
		//create new map to update and replace the new one
		//Vector3[,] NewMap = m_windMap;
		//NewMap = m_windMap;
		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				ApplyWindSources(/*ref NewMap, */i, j);
				if (m_strengthMap[i, j] > 0.0f)
				{
					ApplyWindVector(/*ref NewMap, */i, j);
				}
			}
		}
		//m_windMap = NewMap;
		Debug.Log("pos 20, 20 = " + m_windMap[20, 20].ToString());
		//Debug.Log("pos 20, 20 = " + m_strengthMap[20, 20].ToString());
		//FindAverageMapStrength();
	}

	void ApplyWindSources (/*ref Vector3[,] _NewMap, */int _x, int _y)
	{
		//bool Affected = false;
		for (int i = 0; i < m_sources.Count; i++)
		{
			float windDistance = Vector3.Distance(FindGridPosNoHeight(_x, _y), m_sources[i].m_mapPos);
			if (windDistance < 140.0f)
			{
				//Affected = true;
				float windDistanceFactor = (140.0f - windDistance) / 140.0f;
				//m_windMap[_x, _y] = Vector3.ClampMagnitude(m_sources[i].m_direction * windDistanceFactor * Time.deltaTime + m_windMap[_x, _y], m_distanceBetweenPoints);
				//m_strengthMap[_x, _y] += m_sources[i].m_strength * windDistanceFactor * Time.deltaTime;

				//_NewMap[_x, _y] = Vector3.ClampMagnitude(m_sources[i].m_direction * windDistanceFactor * Time.deltaTime + _NewMap[_x, _y], m_distanceBetweenPoints);
				m_windMap[_x, _y] = Vector3.Normalize(m_sources[i].m_direction * windDistanceFactor * Time.deltaTime + m_windMap[_x, _y]) * 1.0f;
				m_strengthMap[_x, _y] += m_sources[i].m_strength * windDistanceFactor * Time.deltaTime;
			}
		}
		//if (!Affected)
		//{
		//	m_strengthMap[_x, _y] = Mathf.Clamp(m_strengthMap[_x, _y] - 0.1f * Time.deltaTime, 0.0f, 1.0f);
		//}
		//else
		//{
		//	m_strengthMap[_x, _y] = Mathf.Clamp(m_strengthMap[_x, _y], 0.0f, 1.0f);
		//}
		m_strengthMap[_x, _y] = Mathf.Clamp(m_strengthMap[_x, _y] - 0.1f * Time.deltaTime, 0.0f, 1.0f);
		}

	void ApplyWindVector (/*ref Vector3[,] _NewMap, */int _x, int _y)
	{
		Vector3 WindVector = m_windMap[_x, _y];         //the vector being applied
		Vector3 WindVectorPos = FindGridPosNoHeight(_x, _y);		//The position in space of the current windpoint
		for (int i = -1; i <2; i++)						//Evaluate all 9 points around it (8, excluding itself)
		{
			for (int j = -1; j < 2; j++)
			{
				if (((i != 0) || (j != 0)) && (_x + i > 0) && (_y + j > 0) && (_x + i <= m_width - 1) && (_y + j <= m_height - 1))		//check it isnt evaluating itself and it's not past an edge
				{
					Vector3 AffectedPos = FindGridPosNoHeight(_x + i, _y + j);
					float WindDistanceToPoint = Vector3.Distance(WindVectorPos +  WindVector, AffectedPos);
					//_NewMap[_x + i, _y + j] = Vector3.ClampMagnitude((_NewMap[_x + i, _y + j] * m_strengthMap[_x + i, _y + j] + (WindVector * m_strengthMap[_x, _y]) / WindDistanceToPoint * Time.deltaTime), 1.0f);
					m_windMap[_x + i, _y + j] = Vector3.Normalize((m_windMap[_x + i, _y + j] * m_strengthMap[_x + i, _y + j] + (WindVector * m_strengthMap[_x, _y]) / WindDistanceToPoint * Time.deltaTime)) * 1.0f;
					m_strengthMap[_x + i, _y + j] = Mathf.Clamp(((m_strengthMap[_x + i, _y + j] + (m_strengthMap[_x, _y] / WindDistanceToPoint)) * Time.deltaTime), 0.0f, 1.0f);
				}
			}
		}
	}

	public void RefreshAverageDirection()
	{
		Vector3 AveDirection = FindAverageWindFromSources();
		//float AveStrength = FindAverageWindStrengthFromSources();
		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				//m_strengthMap[i, j] = AveStrength;
				m_windMap[i, j] = AveDirection;
			}
		}
	}

	public void SetNewHeights(float[,] _TerrainHeights)
	{
		m_windHeights = _TerrainHeights;
	}

	Vector3 FindGridPos(int _x, int _y)
	{
		Vector3 GridPos = new Vector3(m_distanceBetweenPoints * _y, m_windHeights[_x, _y] + 1.0f, m_distanceBetweenPoints * _x) + transform.position;
		return GridPos;
	}

	Vector3 FindGridPosNoHeight(int _x, int _y)
	{
		Vector3 GridPos = new Vector3(m_distanceBetweenPoints * _y, 0.0f, m_distanceBetweenPoints * _x) + transform.position;
		return GridPos;
	}

	//void AddNewWindSource ()
	//{
	//	Vector3 NewSource = new Vector3(0, 0, 0);
	//	m_windSources.Add(NewSource);
	//}

	//void AddNewWindSource (Vector3 _Direction)
	//{
	//	m_windSources.Add(_Direction);
	//}

	public void AddNewWindSource ()
	{
		WindSource NewSource = Instantiate(WindSource);
		//NewSource.transform.position += new Vector3(0.0f, 0.0f, 0.0f);
		NewSource.InitialiseSource(m_sources.Count, m_strength, this);
		m_sources.Add(NewSource);
	}

	Vector3 FindAverageWindFromSources()
	{
		Vector3 AveDirection = Vector3.zero;
		foreach (WindSource Source in m_sources)
		{
			AveDirection += Source.m_direction * Source.m_strength;
		}
		AveDirection = Vector3.ClampMagnitude(AveDirection / m_sources.Count, m_distanceBetweenPoints);
		Debug.Log("AveDirection = " + AveDirection.ToString() + " point 125, 125 = " + m_windMap[125, 125].ToString());
		return AveDirection;
	}

	float FindAverageWindStrengthFromSources()
	{
		float AveStrength = 0.0f;
		foreach (WindSource Source in m_sources)
		{
			AveStrength += Source.m_strength;
		}
		AveStrength = AveStrength / m_sources.Count;
		return AveStrength;
	}

	void FindAverageMapStrength()
	{
		int counter = 0;
		float total = 0.0f;
		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				total += m_strengthMap[i, j];
				counter++;
			}
		}
		total = total / counter;
		Debug.Log("ave strength = " + total.ToString());
	}
}
