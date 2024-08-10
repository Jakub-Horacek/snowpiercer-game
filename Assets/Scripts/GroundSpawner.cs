using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public List<GameObject> groundTiles;
    private float tilesLength = 1000.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (groundTiles != null && groundTiles.Count > 0)
        {
            groundTiles = groundTiles.OrderBy(r => r.transform.position.z).ToList();
        }
    }

    public void MoveGround()
    {
        GameObject moveGround = groundTiles[0];
        groundTiles.Remove(moveGround);
        float newZ = groundTiles[groundTiles.Count - 1].transform.position.z + tilesLength;
        moveGround.transform.position = new Vector3(-500, 0, newZ);
        groundTiles.Add(moveGround);
    }
}
