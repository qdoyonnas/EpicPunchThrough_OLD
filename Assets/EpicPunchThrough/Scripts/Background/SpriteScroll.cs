using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScroll : MonoBehaviour
{
    #region Parameters

    // External
    [Header("Sprite")]
    [Tooltip("First in array will be used for size calculations")]
	public Sprite[] sprites;
    [Tooltip("Size of sprite, leave 0 for automatic")]
	public Vector2 size = Vector2.zero;

    [Header("Scroll Settings")]
	public bool scrollX = true;
	public bool scrollY = true;
	public float parallaxRatio = 1;
	public string sortingLayerName;
	public int sortingOrder;

    // Internal
	private GameObject[] tiles;
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

    private bool didInit = false;

    #endregion

    #region Fields

    public float inverseRatio
    {
        get {
            return 1 - parallaxRatio;
        }
    }

    #endregion


    void Awake ()
	{
        // Guard
        if( sprites == null || sprites.Length <= 0 ) { Debug.LogError("SpriteScroll not provided a sprite"); return; }

		// Track camera movement by event
		GameManager.Instance.activeCamera.Moved += CameraMoved;
		GameManager.Instance.activeCamera.Zoomed += CameraZoomed;

		ConfigureSpriteScaling(sprites[0]);
        CreateTileObjects();
        didInit = true;

        CalcFieldSize();
		TileField();
	}

    void ConfigureSpriteScaling(Sprite sprite)
    {
        float xScale = 1;
		float yScale = 1;
		if( size.x > 0 ) {
			xScale = size.x / (sprite.rect.width / sprite.pixelsPerUnit);
		}
		if( size.y > 0 ) {
			yScale = size.y / (sprite.rect.height / sprite.pixelsPerUnit);
		}
		spriteScale = new Vector2(xScale, yScale);
        spriteSize = new Vector2(sprite.rect.width / sprite.pixelsPerUnit,
								sprite.rect.height / sprite.pixelsPerUnit);
    }
    void CreateTileObjects()
    {

        tiles = new GameObject[sprites.Length];

        for( int i = 0; i < sprites.Length; i++ ) {
            // Optimize by not making duplicate objects for the same sprite
            foreach( GameObject tile in tiles ) {
                if( tile == null ) { break; }

                if( tile.name == sprites[i].name + "Tile") {
                    tiles[i] = tile;
                    break;
                }
            }
            if( tiles[i] != null ) { continue; }

            // If tile does not exist, create new tile objects for instantiation
		    tiles[i] = new GameObject(sprites[i].name + "Tile");
		    tiles[i].SetActive(false);
		    SpriteRenderer renderer = tiles[i].AddComponent<SpriteRenderer>();
		    renderer.sprite = sprites[i];
		    renderer.sortingLayerName = sortingLayerName;
		    renderer.sortingOrder = sortingOrder;
        }
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
            if( sections.GetLength(1) == numWidth
                    && sections.GetLength(0) == numHeight ) {
                return false;
            }

            ClearSections();
		}

		sections = new GameObject[numHeight, numWidth];
        return true;
	}
    void ClearSections()
    {
        for( int y = sections.GetLength(0)-1; y >= 0; y-- ) {
			for( int x = sections.GetLength(1)-1; x >= 0; x-- ) {
				Destroy(sections[y,x]);
			}
		}
    }
	void TileField()
	{
        if( sections == null ) { return; }

        // Clear previous collection if present
        if( sections[0,0] != null ) {
            ClearSections();
        }

		for( int y=0; y < sections.GetLength(0); y++ ) {
			for( int x=0; x < sections.GetLength(1); x++ ) {
				CreateTile(x, y);
			}
		}
	}
    void CreateTile(int x, int y)
    {
        // Offset so that the first tiles will be in negative quadrants of screen then proceed evenly in positive quadrants
        Vector2 tileOffset = new Vector2(spriteSize.x * (sections.GetLength(1) - 1) / 2f,
										spriteSize.y * (sections.GetLength(0) - 1) / 2f);

        Vector3 pos = new Vector3( (spriteSize.x * x) - tileOffset.x, (spriteSize.y * y) - tileOffset.y );
		sections[y,x] = Instantiate<GameObject>(RandomTile(transform.position + pos), transform.position + pos, Quaternion.identity, transform);
		sections[y,x].transform.localScale = spriteScale;
		sections[y,x].SetActive(true);
    }
    GameObject RandomTile(Vector3 pos)
    {
        if( !didInit || tiles == null || tiles.Length <= 0 ) { return null; }

        if( tiles.Length == 1 ) { return tiles[0]; }

        // Use position to pick tile in order to attempt (not guarenteed) to get the same tile in the same location)
        float value = Math.Abs( ((pos.x + pos.y) + (pos.y * pos.x)) / 3 ); // XXX: Complete garbage
        int index = (int)Mathf.Floor(value % tiles.Length);

        return tiles[index];
    }

    enum ShiftDirection {
        up = 0, right, down, left
    }
    void TileShift(ShiftDirection direction)
    {
        switch( direction ) {
            case ShiftDirection.up:
                // Clear bottom most row
                for( int x=0; x < sections.GetLength(1); x++ ) {
                    Destroy(sections[sections.GetLength(0)-1, x]);
                }

                // Shift all rows down
                for( int y=sections.GetLength(0)-2; y >= 0; y-- ) {
			        for( int x=0; x < sections.GetLength(1); x++ ) {
                        sections[y,x].transform.Translate(0, -spriteSize.y, 0, Space.Self);
                        sections[y+1,x] = sections[y,x];
                    }
                }

                // Create new tiles in top row
                for( int x=0; x < sections.GetLength(1); x++ ) {
                    CreateTile(x, 0);
                }

                break;
            case ShiftDirection.right:
                // Clear left most column
                for( int y=0; y < sections.GetLength(0); y++ ) {
                    Destroy(sections[y, 0]);
                }

                // Shift columns left
                for( int y=0; y < sections.GetLength(0); y++ ) {
			        for( int x=1; x < sections.GetLength(1); x++ ) {
                        sections[y,x].transform.Translate(-spriteSize.x, 0, 0, Space.Self);
                        sections[y,x-1] = sections[y,x];
                    }
                }

                // Create new right most column
                for( int y=0; y < sections.GetLength(0); y++ ) {
                    CreateTile(sections.GetLength(1)-1, y);
                }

                break;
            case ShiftDirection.down:
                // Clear top most row
                for( int x=0; x < sections.GetLength(1); x++ ) {
                    Destroy(sections[0, x]);
                }

                // Shift rows up
                for( int y=1; y < sections.GetLength(0); y++ ) {
                    for( int x=0; x < sections.GetLength(1); x++ ) {
                        sections[y,x].transform.Translate(0, spriteSize.y, 0, Space.Self);
                        sections[y-1,x] = sections[y,x];
                    }
                }

                // Create new bottom row
                for( int x=0; x < sections.GetLength(1); x++ ) {
                    CreateTile(x, sections.GetLength(0)-1);
                }

                break;
            case ShiftDirection.left:
                // Clear right most column
                for( int y=0; y < sections.GetLength(0); y++ ) {
                    Destroy(sections[y, sections.GetLength(1)]);
                }

                // Shift columns right
                for( int y=0; y < sections.GetLength(0); y++ ) {
                    for( int x=sections.GetLength(1)-2; x >= 0; x-- ) {
                        sections[y,x].transform.Translate(spriteSize.x, 0, 0, Space.Self);
                        sections[y,x+1] = sections[y,x];
                    }
                }

                // Create new left most column
                for( int y=0; y < sections.GetLength(0); y++ ) {
                    CreateTile(0, y);
                }

                break;
        }
    }

	void CameraMoved(Vector3 translation)
	{
		transform.Translate(-(translation) * inverseRatio);
        WrapField();

	}
	void CameraZoomed(float zoomDelta)
	{
		if( CalcFieldSize() ) {
            TileField();
        }
	}

    void WrapField()
    {
        // Translate spritefield based on offset from camera

		Vector2 offsetFromCamera = GameManager.Instance.activeCamera.transform.position - transform.position;

		if( scrollX ) {
			if( offsetFromCamera.x > spriteSize.x/2 ) {
				transform.Translate(spriteSize.x, 0, 0);
                if( sprites.Length > 1 ) {  TileShift(ShiftDirection.right); }
			} else if( offsetFromCamera.x < -spriteSize.x/2 ) {
				transform.Translate(-spriteSize.x, 0, 0);
                if( sprites.Length > 1 ) {  TileShift(ShiftDirection.right); }
			}
		}
		if( scrollY ) {
			if( offsetFromCamera.y > spriteSize.y/2 ) {
				transform.Translate(0, spriteSize.y, 0);
                if( sprites.Length > 1 ) {  TileShift(ShiftDirection.right); }
			} else if( offsetFromCamera.y < -spriteSize.y/2 ) {
				transform.Translate(0, -spriteSize.y, 0);
                if( sprites.Length > 1 ) {  TileShift(ShiftDirection.right); }
			}
		}
    }
}