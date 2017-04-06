using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

	public GameObject m_camera;

	// Use this for initialization
	void Start () {
		//m_camera = GameObject.Find("Camera").GetComponent<Camera>();
	}

	public void ResetCamera()
	{
		transform.position = new Vector3(50.0f, -10.0f, 50.0f);
		transform.rotation = Quaternion.identity;
		m_camera.transform.localPosition = new Vector3(0.0f, 90.0f, -90.0f);
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
		if (Input.GetKey("w"))
		{
			transform.position += new Vector3(m_camera.transform.forward.x * 15.0f * Time.deltaTime, 0.0f, m_camera.transform.forward.z * 15.0f * Time.deltaTime);
		}
		if (Input.GetKey("s"))
		{
			transform.position -= new Vector3(m_camera.transform.forward.x * 15.0f * Time.deltaTime, 0.0f, m_camera.transform.forward.z * 15.0f * Time.deltaTime);
		}
		if (Input.GetKey("a"))
		{
			transform.position -= new Vector3(m_camera.transform.right.x * 15.0f * Time.deltaTime, 0.0f, m_camera.transform.right.z * 15.0f * Time.deltaTime);
		}
		if (Input.GetKey("d"))
		{
			transform.position += new Vector3(m_camera.transform.right.x * 15.0f * Time.deltaTime, 0.0f, m_camera.transform.right.z * 15.0f * Time.deltaTime);
		}
		m_camera.transform.position = Vector3.MoveTowards(m_camera.transform.position, transform.position, 1000.0f * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime);
	}
}
