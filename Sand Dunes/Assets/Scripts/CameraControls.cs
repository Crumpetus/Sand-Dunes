using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

	public GameObject m_camera;

	// Use this for initialization
	void Start () {
		//m_camera = GameObject.Find("Camera").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1))
		{
			transform.Rotate(new Vector3(0.0f, 90.0f * Time.deltaTime * Input.GetAxis("Mouse X"), 0.0f));
			transform.position += new Vector3(0.0f, 15.0f * Time.deltaTime * Input.GetAxis("Mouse Y"), 0.0f);
		}
		//m_camera.transform.position += new Vector3(0.0f, 0.0f, 500.0f * Time.deltaTime * Input.GetAxis("Mouse ScrollWheel"));
		//m_camera.transform.moveto
		m_camera.transform.position = Vector3.MoveTowards(m_camera.transform.position, transform.position, 1000.0f * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime);
	}
}
