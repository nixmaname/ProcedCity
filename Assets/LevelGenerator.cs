using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject tileHolder;
    public GameObject[] tileset;
    public List<GameObject> tiles = new List<GameObject>();
    public Vector2 bounds;
    Vector2 currentSize;
    Vector3[] directions = { new Vector3(5,0,0),new Vector3(-5,0,0),new Vector3(0,0,-5),new Vector3(0,0,5)};
    

    // Start is called before the first frame update
    void Start()
    {
        int randomI= Random.Range(0,12);
        Instantiate(tileset[randomI], Vector3.zero, Quaternion.identity);

        for (int i = 0; i < 4; i++)
        {
            GameObject tempTileHolder = Instantiate(tileHolder, directions[i], Quaternion.identity);
            tempTileHolder.GetComponent<TileHoldScript>().parentVec.Add(tileset[randomI].GetComponent<TileProperties>().sides);
           
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
