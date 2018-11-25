using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroll : MonoBehaviour
{
    #region Parameters

    // External
    [Header("Sprite")]
	public Sprite sprite;
    [Tooltip("Size of sprite, leave 0 for automatic")]
	public Vector2 size = Vector2.zero;

    [Header("Scroll Settings")]
	public bool scrollX = true;
	public bool scrollY = true;
	public float parallaxRatio = 1;
	public string sortingLayerName;
	public int sortingOrder;

    // Internal
	private GameObject tile;
	private Vector2 spriteScale = Vector2.one;
	private Vector2 _spriteSize;
	public Vector2 spriteSize {
		get {
			return new Vector2(_spriteSize.x * spriteScale.x,
								_spriteSize.y * spriteScale.y);
		}
		set {
			_spriteSize = value;
		}
	}
	private GameObject[,] sections;

    #endregion

    #region Fields

    public float inverseRatio
    {
        get {
            return 1 - parallaxRatio;
        }
    }

    #endregion


    void Start ()
	{
		// Track camera movement by event
		GameManager.Instance.activeCamera.Moved += CameraMoved;
		GameManager.Instance.activeCamera.Zoomed += CameraZoomed;

		// Configure sprite scaling
		float xScale = 1;
		float yScale = 1;
		if( size.x > 0 ) {
			xScale = size.x / (sprite.rect.width / sprite.pixelsPerUnit);
		}
		if( size.y > 0 ) {
			yScale = size.y / (sprite.rect.height / sprite.pixelsPerUnit);
		}
		spriteScale = new Vector2(xScale, yScale);

		// Create tile object
		tile = new GameObject(sprite.name + "Tile");
		tile.SetActive(false);
		SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
		renderer.sprite = sprite;
		renderer.sortingLayerName = sortingLayerName;
		renderer.sortingOrder = sortingOrder;
		spriteSize = new Vector2(sprite.rect.width / sprite.pixelsPerUnit,
								sprite.rect.height / sprite.pixelsPerUnit);
		
		TileField();
	}

	bool CalcFieldSize()
	{
		Vector2 cameraSize = new Vector2(GameManager.Instance.activeCamera.Camera.orthographicSize * GameManager.Instance.activeCamera.Camera.aspect * 2,
										GameManager.Instance.activeCamera.Camera.orthographicSize * 2);

        // Relative size of screen to sprites
		float widthRatio = cameraSize.x / spriteSize.x;
		float heightRatio = cameraSize.y / spriteSize.y;

        // Only calc for directions this background tiles in // + 2 adds covers past screen size to cover movement
		int numWidth = ( scrollX ? Mathf.CeilToInt(widthRatio) + 2 : 1 );
		int numHeight = ( scrollY ? Mathf.CeilToInt(heightRatio) + 2 : 1 );

        // Guarentee uneven to prevent skipping on zooms
        if( numWidth != 1 && numWidth % 2 == 0 ) {
            numWidth++;
        }
        if( numHeight != 1 && numHeight % 2 == 0  ) {
            numHeight++;
        }

		if( sections != null ) {
            // No retiling if number of tiles hasn't changed
            if( sections.GetLength(0) == numWidth
                    && sections.GetLength(1) == numHeight ) {
                return false;
            }

            // Clear previous collection
            for( int y = sections.GetLength(1)-1; y >= 0; y-- ) {
				for( int x = sections.GetLength(0)-1; x >= 0; x-- ) {
					Destroy(sections[x,y]);
				}
			}
		}

		sections = new GameObject[numWidth, numHeight];
        return true;
	}
	
	void TileField()
	{
		bool retile = CalcFieldSize();
        if( !retile ) { return; }

        // Offset so that the first tiles will be in negative quadrants of screen then proceed evenly in positive quadrants
		Vector2 tileOffset = new Vector2(spriteSize.x * (sections.GetLength(0) - 1) / 2f,
										spriteSize.y * (sections.GetLength(1) - 1) / 2f);
		for( int y=0; y < sections.GetLength(1); y++ ) {
			for( int x=0; x < sections.GetLength(0); x++ ) {
				Vector3 pos = new Vector3( (spriteSize.x * x) - tileOffset.x, (spriteSize.y * y) - tileOffset.y );
				sections[x,y] = Instantiate<GameObject>(tile, transform.position + pos, Quaternion.identity, transform);
				sections[x,y].transform.localScale = spriteScale;
				sections[x,y].SetActive(true);
			}
		}
	}

	void CameraMoved(Vector3 translation)
	{
		transform.Translate(-(translation) * inverseRatio);
	}
	void CameraZoomed(float zoomDelta)
	{
		TileField();
	}

	void Update()
	{
        WrapField();
	}

    void WrapField()
    {
        // Translate spritefield based on offset from camera

		Vector2 offsetFromCamera = GameManager.Instance.activeCamera.transform.position - transform.position;

		if( scrollX ) {
			if( offsetFromCamera.x > spriteSize.x/2 ) {
				transform.Translate(spriteSize.x, 0, 0);
				offsetFromCamera = GameManager.Instance.activeCamera.transform.position - transform.position;
			}
			if( offsetFromCamera.x < -spriteSize.x/2 ) {
				transform.Translate(-spriteSize.x, 0, 0);
				offsetFromCamera = GameManager.Instance.activeCamera.transform.position - transform.position;
			}
		}
		if( scrollY ) {
			if( offsetFromCamera.y > spriteSize.y/2 ) {
				transform.Translate(0, spriteSize.y, 0);
				offsetFromCamera = GameManager.Instance.activeCamera.transform.position - transform.position;
			}
			if( offsetFromCamera.y < -spriteSize.y/2 ) {
				transform.Translate(0, -spriteSize.y, 0);
				offsetFromCamera = GameManager.Instance.activeCamera.transform.position - transform.position;
			}
		}
    }
}