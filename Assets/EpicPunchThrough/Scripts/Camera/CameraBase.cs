﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CameraBase : MonoBehaviour
{
    public float minimumZoom = 4;
	public float maximumZoom = 8;

    public Image screenFade;

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
    
    float cameraZDistance = -10;

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

    public void Move(Vector2 vector, bool relative = false)
    {
        Vector3 oldPosition = transform.position;

        if( relative ) {
            transform.Translate(vector);
        } else {
            transform.position = new Vector3(vector.x, vector.y, cameraZDistance);
        }
        CheckWorldBounds();

        Vector2 translation = oldPosition - transform.position;
        OnMove(translation);
    }
    protected void CheckWorldBounds()
	{
		if( GameManager.Instance.settings.worldBounds.leftBound ) {
			if( cameraLeft < GameManager.Instance.settings.worldBounds.minX ) {
				transform.position = new Vector3(GameManager.Instance.settings.worldBounds.minX + cameraSize.x,
												transform.position.y,
												transform.position.z);
			}
		}
		if( GameManager.Instance.settings.worldBounds.rightBound ) {
			if( cameraRight > GameManager.Instance.settings.worldBounds.maxX ) {
				transform.position = new Vector3(GameManager.Instance.settings.worldBounds.maxX - cameraSize.x,
												transform.position.y,
												transform.position.z);
			}
		}

		if( GameManager.Instance.settings.worldBounds.topBound ) {
			if( cameraTop > GameManager.Instance.settings.worldBounds.maxY ) {
				transform.position = new Vector3(transform.position.x,
												GameManager.Instance.settings.worldBounds.maxY - cameraSize.y,
												transform.position.z);
			}
		}
		if( GameManager.Instance.settings.worldBounds.bottombound ) {
			if( cameraBottom < GameManager.Instance.settings.worldBounds.minY ) {
				transform.position = new Vector3(transform.position.x,
												GameManager.Instance.settings.worldBounds.minY + cameraSize.y,
												transform.position.z);
			}
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

    public void Zoom(float value, bool relative = false)
    {
        float oldSize = Camera.orthographicSize;

        if( relative ) {
            Camera.orthographicSize += value;
        }
        else {
            Camera.orthographicSize = value;
        }
        ConfineZoom();

        float change = Camera.orthographicSize - oldSize;
        OnZoom(change);
    }
    void ConfineZoom()
    {
        if( Camera.orthographicSize < minimumZoom ) {
            Camera.orthographicSize = minimumZoom;
        } else if( Camera.orthographicSize > maximumZoom ) {
            Camera.orthographicSize = maximumZoom;
        }
    }

    public delegate void FadeAction(float value);
    public event FadeAction StartedFade;
    public event FadeAction UpdatedFade;
    public event FadeAction EndedFade;
    public void Fade(float value, float duration) // value: 0-1. 0 fade in, 1 fade to black
    {
        if( screenFade == null ) { return; }

        if( duration <= 0 ) {
            Color c = screenFade.color;
            c.a = value;
            screenFade.color = c;
        } else {
            screenFade.DOFade(value, duration)
                .OnStart(() => { StartedFade(screenFade.color.a); })
                .OnUpdate(() => { UpdatedFade(screenFade.color.a); })
                .OnComplete(() => { EndedFade(screenFade.color.a); });
        }
    }

	void Awake()
	{
		_camera = gameObject.GetComponentInChildren<Camera>();

        if( screenFade == null ) {
            GameObject.Find("ScreenFade").GetComponent<Image>();
        }

        GameManager.Instance.fixedUpdated += DoFixedUpdate;
	}
    void DoFixedUpdate(GameManager.UpdateData data)
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
}
