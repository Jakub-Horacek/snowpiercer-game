using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundExtender : MonoBehaviour
{

    public float tileLength = 1000.0f;
    public int tilesOnEachSide = 2; // number of tiles to extend existing tile on each side
    public GameObject tilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        ExtendInitialTile();
    }

    GameObject FindInitialTile()
    {
        return GameObject.FindGameObjectsWithTag("Ground Tile")[0];
    }

    void ExtendInitialTile()
    {
        GameObject initialTile = FindInitialTile();
        Vector3 lastTilePosition = initialTile.transform.position;

        // add tilesOnEachSide count of tiles on each side of the last tile position.z
        for (int i = 1; i <= tilesOnEachSide; i++)
        {
            // add tile in front
            Vector3 rightTilePosition = new Vector3(lastTilePosition.x, lastTilePosition.y, lastTilePosition.z + tileLength * i);
            Instantiate(tilePrefab, rightTilePosition, Quaternion.identity);

            // add tile behind
            Vector3 leftTilePosition = new Vector3(lastTilePosition.x, lastTilePosition.y, lastTilePosition.z - tileLength * i);
            Instantiate(tilePrefab, leftTilePosition, Quaternion.identity);
        }
    }
}
