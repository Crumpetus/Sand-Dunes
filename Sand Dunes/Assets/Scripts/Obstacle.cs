using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	private ObstacleManager m_manager;
	private Camera m_camera;
	private bool m_moving;
	private int m_ID;

	// Use this for initialization
	void Start () {
		m_camera = GameObject.Find("Camera").GetComponent<Camera>();
	}

	public void InitialiseObstacle (int _i, Vector3 _position, ObstacleManager _manager)
	{
		m_ID = _i;
		transform.position = _position;
		m_manager = _manager;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if ((Physics.Raycast(ray, out hit)) && (hit.collider.GetComponent<Obstacle>().m_ID == m_ID))
			{
				m_moving = true;
			}
		}
		if (m_moving)
		{
			if (Input.GetMouseButton(0))
			{
				transform.position += (m_camera.transform.forward * Input.GetAxis("Mouse Y") * 100 * Time.deltaTime) + (m_camera.transform.right * Input.GetAxis("Mouse X") * 100 * Time.deltaTime);
				transform.position = new Vector3(transform.position.x, 17.0f, transform.position.z);
			}
			else
			{
				m_moving = false;
				m_manager.RefreshObstacleMap();
			}
		}
	}
}
