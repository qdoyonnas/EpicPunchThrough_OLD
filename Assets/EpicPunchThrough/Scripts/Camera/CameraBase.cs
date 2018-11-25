using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBase : MonoBehaviour
{
	private Camera _camera;
	public Camera Camera {
		get { return _camera; }
	}
	private Vector2 cameraSize {
		get {
			return new Vector2(_camera.orthographicSize * _camera.aspect, _camera.orthographicSize);
		}
	}
	private float cameraLeft {
		get { return transform.position.x - cameraSize.x; }
	}
	private float cameraRight {
		get { return transform.position.x + cameraSize.x; }
	}
	private float cameraTop {
		get { return transform.position.y + cameraSize.y; }
	}
	private float cameraBottom {
		get { return transform.position.y - cameraSize.y; }
	}

	float shakeRadius = 0;
	float shakeDuration = 0;

	public delegate void MoveAction(Vector3 translation);
	public event MoveAction Moved;
	protected virtual void OnMove(Vector3 translation)
	{
		// Copy event to avoid race scenarios
		MoveAction handler = Moved;
		if( handler != null ) {
			handler(translation);
		}
	}

	public delegate void ZoomAction(float zoomInput);
	public event ZoomAction Zoomed;
	protected virtual void OnZoom(float zoomInput)
	{
		// Copy event to avoid race scenarios
		ZoomAction handler = Zoomed;
		if( handler != null ) {
			handler(zoomInput);
		}
	}

	void Awake()
	{
		_camera = gameObject.GetComponentInChildren<Camera>();
	}

	void FixedUpdate()
	{
		if( shakeDuration > 0 ) {
			float xVal = Random.Range(-shakeRadius, shakeRadius);
			float yVal = Random.Range(-shakeRadius, shakeRadius);
			_camera.transform.localPosition = new Vector3(xVal, yVal, 0);

			shakeRadius -= shakeRadius * (Time.deltaTime/shakeDuration);
			shakeDuration -= Time.deltaTime;
		} else {
			_camera.transform.localPosition = Vector3.zero;
		}
	}

	public void StartScreenShake(float radius, float duration)
	{
		if( radius >= shakeRadius ) {
			shakeRadius = radius;
			shakeDuration = duration;
		}
	}

	protected void CheckWorldBounds()
	{
		if( GameManager.Instance.gameOptions.worldBounds.leftBound ) {
			if( cameraLeft < GameManager.Instance.gameOptions.worldBounds.minX ) {
				transform.position = new Vector3(GameManager.Instance.gameOptions.worldBounds.minX + cameraSize.x,
												transform.position.y,
												transform.position.z);
			}
		}
		if( GameManager.Instance.gameOptions.worldBounds.rightBound ) {
			if( cameraRight > GameManager.Instance.gameOptions.worldBounds.maxX ) {
				transform.position = new Vector3(GameManager.Instance.gameOptions.worldBounds.maxX - cameraSize.x,
												transform.position.y,
												transform.position.z);
			}
		}

		if( GameManager.Instance.gameOptions.worldBounds.topBound ) {
			if( cameraTop > GameManager.Instance.gameOptions.worldBounds.maxY ) {
				transform.position = new Vector3(transform.position.x,
												GameManager.Instance.gameOptions.worldBounds.maxY - cameraSize.y,
												transform.position.z);
			}
		}
		if( GameManager.Instance.gameOptions.worldBounds.bottombound ) {
			if( cameraBottom < GameManager.Instance.gameOptions.worldBounds.minY ) {
				transform.position = new Vector3(transform.position.x,
												GameManager.Instance.gameOptions.worldBounds.minY + cameraSize.y,
												transform.position.z);
			}
		}
	}
}
