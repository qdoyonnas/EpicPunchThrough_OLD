using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Static

    private static GameManager instance;
    public static GameManager Instance
    {
        get {
            return instance;
        }
    }

    #endregion

    public CameraBase activeCamera;

    #region Game Settings

    public GameOptions gameOptions = new GameOptions();

	[SerializeField] float gravity = 26f;
	[SerializeField] float maxSpeed = 3000f;
	[SerializeField] float vF2NRatio = 0.01f;
	[SerializeField] bool leftBound;
	[SerializeField] bool rightBound;
	[SerializeField] bool topBound;
	[SerializeField] bool bottombound = true;
	[SerializeField] float minX;
	[SerializeField] float maxX;
	[SerializeField] float minY = 3f;
	[SerializeField] float maxY;

    #endregion

	float timeScaleDuration = 0;

	// Use this for initialization
	void Awake()
	{
		if( instance == null ) {
			instance = this;
		} else {
			//XXX: hack singleton
			Debug.LogError("Two instances of GameManager. Destroying.");
            Destroy(gameObject);
		}

		WorldBounds bounds = new WorldBounds();
		bounds.leftBound = leftBound;
		bounds.rightBound = rightBound;
		bounds.topBound = topBound;
		bounds.bottombound = bottombound;
		bounds.minX = minX;
		bounds.minY = minY;
		bounds.maxX = maxX;
		bounds.maxY = maxY;
		
		gameOptions.worldBounds = bounds;
		gameOptions.gravity = gravity;
		gameOptions.maximumSpeed = maxSpeed;
		gameOptions.vF2NRatio = vF2NRatio;

        if( activeCamera == null ) {
            activeCamera = GameObject.Find( "CameraBase" ).GetComponent<CameraBase>();
        }
	}

	void OnGUI()
	{
	}

	void FixedUpdate()
	{
		if( timeScaleDuration > 0 ) {
			timeScaleDuration -= Time.fixedUnscaledDeltaTime;
		}
	}

	public void StartSlowMotion(float timeScale, float duration)
	{
		timeScaleDuration = duration;
	}
}