using UnityEngine;
using System.Collections;

public class WindSource : MonoBehaviour {

	[SerializeField]
	public Vector3 m_direction;
	public Vector3 m_mapPos;
	public float m_strength;
	public GameObject m_particles;
	private WindManager m_manager;
	private Camera m_camera;
	private bool m_moving;
	private int m_ID;
	//private float m_distanceToCentre;

	// Use this for initialization
	void Start () {
		//m_particles = transform.FindChild("Particles")/*.GetComponent<GameObject>()*/;
		m_camera = GameObject.Find("Camera").GetComponent<Camera>();
		//m_distanceToCentre = transform.FindChild("Particles").localPosition.z;
	}

	public void InitialiseSource (int _i, float _strength, WindManager _Manager)
	{
		m_ID = _i;
		m_manager = _Manager;
		transform.Rotate(new Vector3(0.0f, 72.0f * _i, 0.0f));
		m_strength = _strength;
		m_particles.GetComponentInChildren<ParticleSystem>().startSize -= m_particles.GetComponentInChildren<ParticleSystem>().startSize * (1 - m_strength) / 1.5f;
		UpdateDirection();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if ((Physics.Raycast(ray, out hit)) && (hit.collider.GetComponent<WindSource>()))
			{
				if (hit.collider.GetComponent<WindSource>().m_ID == m_ID)
				{
					m_moving = true;
				}
			}
		}
		if (m_moving)
		{
			if (Input.GetMouseButton(0))
			{
				if (Input.GetAxis("Mouse X") > 0)
				{
					transform.Rotate(new Vector3(0.0f, 90.0f * Time.deltaTime, 0.0f));
				}
				if (Input.GetAxis("Mouse X") < 0)
				{
					transform.Rotate(new Vector3(0.0f, -90.0f * Time.deltaTime, 0.0f));
				}
				UpdateDirection();
			}
			else
			{
				m_moving = false;
				//m_manager.RefreshAverageDirection();
			}
		}
		//transform.Rotate(new Vector3(0.0f, 5.0f * Time.deltaTime, 0.0f));
	}

	void UpdateDirection()
	{
		m_direction = Vector3.Normalize(transform.position - m_particles.transform.position);
		m_mapPos = m_particles.transform.position;
		//Debug.Log(m_direction.ToString());
		//Debug.Log(m_particles.transform.position.ToString());
	}
}
