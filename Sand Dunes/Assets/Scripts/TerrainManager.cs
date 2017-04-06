using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TerrainManager : MonoBehaviour {

	public Button m_resetButton;
	public Dropdown m_dropdown;
	public List<Sprite> m_dropdownSprites;
	public Texture2D[] m_heightMapTextures;
    private Terrain m_terrain;
	[SerializeField]
	private WindManager m_wind;
	public ObstacleManager m_obstacles;
	private float m_simulationMultiplier;
    private int m_width;
    private int m_height;
	private float m_terrainSize;
    private float[,] m_heightMap;
	private float[,] m_posHeightMap;
	private float m_distanceBetweenPoints;
	private float m_maxDistanceToTransfer;
    private Texture2D m_heightMapTexture;
	//public GameObject m_windManager;

	// Use this for initialization
	void Start () {
		PopulateDropdownList();
		m_simulationMultiplier = 1.0f;
		m_heightMapTexture = m_heightMapTextures[0];
        m_terrain = Terrain.activeTerrain;
        //m_terrain.terrainData = new TerrainData();
        m_width = m_heightMapTexture.width;
        m_height = m_heightMapTexture.height;
		m_distanceBetweenPoints = m_terrain.terrainData.size.x / m_width;
		m_maxDistanceToTransfer = Mathf.Abs(2 * m_distanceBetweenPoints * Mathf.Sin(90.0f / 2));
		Debug.Log("m_maxDistanceToTransfer = " + m_maxDistanceToTransfer.ToString());
		m_terrainSize = m_terrain.terrainData.size.y;
		Debug.Log("m_terrainSize = " + m_terrainSize.ToString());
        m_heightMap = new float[m_width, m_height];
		m_posHeightMap = new float[m_width, m_height];
		ResetMap();

		m_obstacles.InitialiseMap(m_width, m_height, m_distanceBetweenPoints);
		//m_wind = FindObjectOfType<WindManager>();
		//m_wind = m_windManager.GetComponent<WindManager>();
		m_wind.InitialiseWindMap(m_width, m_height, m_distanceBetweenPoints, m_posHeightMap);
		//m_wind = new WindManager(m_width, m_height, (m_terrain.terrainData.size.x / m_width), m_heightMap);
    }
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetKey("space")) && (m_wind.getNoOfSources() > 0))
		{
			AdjustHeights();
			m_wind.SetNewHeights(m_posHeightMap);
		}
		else if (Input.GetKeyDown("r"))
		{
			ResetMap();
		}
	}

	public void ChangeMultiplier(Single _multiplier)
	{
		m_simulationMultiplier = _multiplier;
	}

	void PopulateDropdownList()
	{
		m_dropdown.AddOptions(m_dropdownSprites);
	}

	public void DropdownMenuChanged(int _index)
	{
		m_heightMapTexture = m_heightMapTextures[_index];
		m_resetButton.GetComponent<Image>().color = Color.green;
	}

	public void ResetMap()
	{
		Color[] map = m_heightMapTexture.GetPixels();
		for (int i = 0; i < m_width; i++)
		{
			for (int j = 0; j < m_height; j++)
			{
				m_heightMap[i, j] = map[i * m_width + j].grayscale;
				m_posHeightMap[i, j] = m_heightMap[i, j] * m_terrainSize;
			}
		}
		m_terrain.terrainData.SetHeights(0, 0, m_heightMap);
		m_resetButton.GetComponent<Image>().color = Color.white;
	}

    void AdjustHeights()
    {
		//get heights of terrain
		//set the new height values of the terrain
		//Vector3 WindVector = new Vector3();
		//Vector3 SandVector = new Vector3();
		float[,] NewMap = m_heightMap;
		float[,] NewPosMap = m_posHeightMap;
		Vector3 WindVector;
		Vector3 SandVector;
		Vector3 ReceivingSandVector;
		float TransferAmount;
		float WindDistanceToPoint;
		for (int i = 0; i < m_width; i++)
        {
            for (int j = 0; j < m_height; j++)
            {
				WindVector = m_wind.getWindVector(i, j)/* * m_distanceBetweenPoints*/;
				SandVector = FindGridPosNoHeight(i, j);
				//m_heightMap[i, j] = m_heightMap[i + 1, j];
				for (int k = -1; k < 2; k++)                        //Evaluate all 9 points around it (8, excluding itself)
				{
					for (int l = -1; l < 2; l++)
					{ 
						if ((i + k >= 0) && (j + l >= 0) && (i + k <= m_width - 1) && (j + l <= m_height - 1))      //check it's not past an edge
						{
							if (((k != 0) || (l != 0)) && (!m_obstacles.CheckCoordinate(i, j)) && (!m_obstacles.CheckCoordinate(i + k, j + l)))                    // and it isnt evaluating itself
							{
								//if ((i == 120) && (j == 120))
								//{
								//	Debug.Log("hit i = " + i.ToString() + " j = " + j.ToString() + " k = " + k.ToString() + " l = " + l.ToString());
								//}
								ReceivingSandVector = FindGridPosNoHeight(i + k, j + l);
								WindDistanceToPoint = Vector3.Distance(SandVector + WindVector, ReceivingSandVector);
								TransferAmount = CalculateTransferAmount(i, j, WindDistanceToPoint, m_heightMap[i, j], m_heightMap[i + k, j + l], ReceivingSandVector, SandVector, WindVector);
								if (TransferAmount > 0.0f)
								{
									TransferAmount = Mathf.Clamp(TransferAmount, 0.0f, 0.03f * m_simulationMultiplier);
									NewMap[i, j] -= TransferAmount;
									NewMap[i + k, j + l] += TransferAmount;
									NewPosMap[i, j] -= TransferAmount * m_terrainSize;
									NewPosMap[i + k, j + l] += TransferAmount * m_terrainSize;
								}
								if (Mathf.Abs(NewMap[i, j] - NewMap[i + k, j + l]) > 0.02f)
								{
									float Diff = NewMap[i, j] - NewMap[i + k, j + l];
									NewMap[i, j] -= Diff * 0.25f;
									NewMap[i + k, j + l] += Diff * 0.25f;
									NewPosMap[i, j] -= Diff * 0.25f * m_terrainSize;
									NewPosMap[i + k, j + l] += Diff * 0.25f * m_terrainSize;
								}
							}
						}
						else
						{
							//Vector3 WindVector = m_wind.getWindVector(i, j);
							//Vector3 SandVector = FindGridPos(i, j);
							TransferAmount = CalculateTransferAmount(i, j, 0.3f, m_heightMap[i, j], m_heightMap[i, j], SandVector, SandVector, WindVector);
							if (TransferAmount > 0.0f)
							{
								TransferAmount = Mathf.Clamp(TransferAmount, 0.01f, 0.03f * m_simulationMultiplier);
								NewMap[i, j] -= TransferAmount;
								NewPosMap[i, j] -= TransferAmount * m_terrainSize;
							}
						}
					}
				}
            }
        }
		m_heightMap = NewMap;
		m_posHeightMap = NewPosMap;
        m_terrain.terrainData.SetHeightsDelayLOD(0, 0, m_heightMap);
		//m_terrain.Flush();
    }

	float CalculateTransferAmount(int _x, int _y, float _WindDistanceToPoint, float _height1, float _height2, Vector3 _ReceivingVector, Vector3 _SandVector, Vector3 _WindVector)
	{
		//take in wind strength, wind vector,
		//height1 = sending, height2 = receiving
		if (_WindDistanceToPoint > m_maxDistanceToTransfer)
		{
			return 0.0f;
		}
		float TransferAmount;
		//float HeightDifference = Mathf.Abs(_height1 - _height2);
		float HeightDifference = _height1 - _height2;
		if (HeightDifference > 0.0f)	//downhill
		{
			TransferAmount = (m_heightMap[_x, _y] * 2) * (((m_wind.getWindStrength(_x, _y) * 2) / (_WindDistanceToPoint * 2)) * (1 + HeightDifference) * 3) * Time.deltaTime;
		}
		else							//uphill / flat
		{
			TransferAmount = (m_heightMap[_x, _y] * 3) * (((m_wind.getWindStrength(_x, _y) * 2) / (_WindDistanceToPoint * 2)) * (1 / (1.0f + Mathf.Abs(HeightDifference)))) * Time.deltaTime;
		}
		if (TransferAmount > _height1)
		{
			TransferAmount = _height1;
		}
		//Debug.Log("/ x = " + _x.ToString() + "/ y = " + _y.ToString() + "/ x2 = " + _x2.ToString() + "/ y2 = " + _y2.ToString() + "/ transfer amount = " + TransferAmount.ToString());
		return TransferAmount * m_simulationMultiplier;
	}

	public void ForceSetCoord (int _x, int _y, float _val)
	{
		m_heightMap[_x, _y] = _val;
		m_posHeightMap[_x, _y] = _val * m_terrainSize;
	}

	Vector3 FindGridPos(int _x, int _y)
	{
		Vector3 GridPos = new Vector3(m_distanceBetweenPoints * _y, m_posHeightMap[_x, _y], m_distanceBetweenPoints * _x) + transform.position;
		return GridPos;
	}

	Vector3 FindGridPosNoHeight(int _x, int _y)
	{
		Vector3 GridPos = new Vector3(m_distanceBetweenPoints * _y, 0.0f, m_distanceBetweenPoints * _x) + transform.position;
		return GridPos;
	}
}
