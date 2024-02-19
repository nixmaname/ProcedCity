using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class VectorRemovalTest : MonoBehaviour
{
    public GameObject[] roadTiles;
    public GameObject tile;
    List<List<int>> vectorListTest = new List<List<int>>();
    public int entropy;
    public List<int> axisRules = new List<int>();

    Vector3[] directions = { new Vector3(5, 0, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, -5), new Vector3(0, 0, 5) };

    RoadTileManagement rTM;
    Vector2 gridBoundries;

    public List<Vector4> totalOptions = new List<Vector4>();
    CityGenerator cityGenerator;

    Dictionary<string, int> sequenceMap = new Dictionary<string, int>
{
    { "0000", 0 },
    { "0011", 1 },
    { "1100", 2 },
    { "1111", 3 },
    { "0110", 4 },
    { "0101", 5 },
    { "1001", 6 },
    { "1010", 7 }
};

    void Start()
    {
        cityGenerator = FindObjectOfType<CityGenerator>();
        rTM = FindObjectOfType<RoadTileManagement>();
        gridBoundries = rTM.gridSize;
        PopulateList();
        ExecuteSearch();
    }

    public void ExecuteSearch()
    {
        rTM.onHold.Add(gameObject);
        for (int i = 0; i < 4; i++)
        {
            CheckAxis(i);
        }
        rTM.once++;

        if (!rTM.canCheck)
        {
            rTM.canCheck = true;
        }

    }

    void CheckAxis(int axis)
    {
        if (axisRules[axis] == 1)
        {
            for (int i = vectorListTest.Count - 1; i > -1; i--)
            {
                if (!vectorListTest[i][axis].Equals(1))
                {
                    vectorListTest.RemoveAt(i);
                }
            }
        }
        else if (axisRules[axis] == 0)
        {
            for (int i = vectorListTest.Count - 1; i > -1; i--)
            {
                if (!vectorListTest[i][axis].Equals(0))
                {
                    vectorListTest.RemoveAt(i);
                }
            }
        }
        entropy = vectorListTest.Count;
    }



    void PopulateList()
    {
        for (int i = 0; i < totalOptions.Count; i++)
        {
            vectorListTest.Add(new List<int> { 0, 0, 0, 0 });

            for (int j = 0; j < 4; j++)
            {
                if (j == 0)
                    vectorListTest[i][j] = (int)totalOptions[i].x;
                else if (j == 1)
                    vectorListTest[i][j] = (int)totalOptions[i].y;
                else if (j == 2)
                    vectorListTest[i][j] = (int)totalOptions[i].z;
                else if (j == 3)
                    vectorListTest[i][j] = (int)totalOptions[i].w;
            }
        }
    }

    public void GenerateRoadTile()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10);
        bool checkForRoadCross = false;

        if (vectorListTest.Count > 4 && colliders.Length > 0)
        {
            foreach (var item in colliders)
            {
                if (item.gameObject.tag.Equals("Cross"))
                {
                    checkForRoadCross = true;
                }
            }
            if (checkForRoadCross)
            {
                for (int i = vectorListTest.Count - 1; i > -1; i--)
                {
                    string secondSeq = vectorListTest[i][0].ToString() + vectorListTest[i][1].ToString() +
            vectorListTest[i][2].ToString() + vectorListTest[i][3].ToString();
                    if (secondSeq.Equals("1111"))
                    {
                        vectorListTest.RemoveAt(i);
                    }
                }
            }
        }

        int randomBlock = Random.Range(0, vectorListTest.Count);
        string seq = vectorListTest[randomBlock][0].ToString() + vectorListTest[randomBlock][1].ToString() +
            vectorListTest[randomBlock][2].ToString() + vectorListTest[randomBlock][3].ToString();

        if (sequenceMap.ContainsKey(seq))
        {
            GameObject tile = Instantiate(roadTiles[sequenceMap[seq]], transform.position, Quaternion.identity);
            if (seq.Equals("0000"))
            {
                //cityGenerator.noRoadList.Add(tile.transform.GetChild(0).gameObject);
            }
            else
            {
                rTM.allRoads.Add(tile.transform.position, tile);
                rTM.connectedRoads.Add(tile);
            }
            rTM.blockCount++;
        }


        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                if (transform.position.x + directions[i].x < gridBoundries.x * 5)
                {
                    Collider[] colCheck = Physics.OverlapSphere(transform.position + directions[i], 0.2f);

                    if (colCheck.Length == 0)
                    {
                        GameObject newTile = Instantiate(tile, transform.position + directions[i], Quaternion.identity);
                        newTile.GetComponent<VectorRemovalTest>().axisRules = new List<int> { 2, 2, 2, 2 };
                        newTile.GetComponent<VectorRemovalTest>().axisRules[1] = int.Parse(seq[0].ToString());

                    }
                    else if (colCheck[0].tag.Equals("Tile"))
                    {
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().axisRules[1] = int.Parse(seq[0].ToString());
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().ExecuteSearch();
                    }
                }
            }
            else if (i == 1)
            {
                if (transform.position.x + directions[i].x > -(gridBoundries.x + 1) * 5)
                {

                    Collider[] colCheck = Physics.OverlapSphere(transform.position + directions[i], 0.2f);

                    if (colCheck.Length == 0)
                    {
                        GameObject newTile = Instantiate(tile, transform.position + directions[i], Quaternion.identity);
                        newTile.GetComponent<VectorRemovalTest>().axisRules = new List<int> { 2, 2, 2, 2 };
                        newTile.GetComponent<VectorRemovalTest>().axisRules[0] = int.Parse(seq[1].ToString());
                    }
                    else if (colCheck[0].tag.Equals("Tile"))
                    {
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().axisRules[0] = int.Parse(seq[1].ToString());
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().ExecuteSearch();
                    }
                }
            }
            else if (i == 2)
            {
                if (transform.position.z + directions[i].z > -gridBoundries.y * 5)
                {

                    Collider[] colCheck = Physics.OverlapSphere(transform.position + directions[i], 0.2f);

                    if (colCheck.Length == 0)
                    {
                        GameObject newTile = Instantiate(tile, transform.position + directions[i], Quaternion.identity);
                        newTile.GetComponent<VectorRemovalTest>().axisRules = new List<int> { 2, 2, 2, 2 };
                        newTile.GetComponent<VectorRemovalTest>().axisRules[3] = int.Parse(seq[2].ToString());
                    }
                    else if (colCheck[0].tag.Equals("Tile"))
                    {
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().axisRules[3] = int.Parse(seq[2].ToString());
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().ExecuteSearch();
                    }
                }
            }
            else if (i == 3)
            {
                if (transform.position.z + directions[i].z < (gridBoundries.y + 1) * 5)
                {

                    Collider[] colCheck = Physics.OverlapSphere(transform.position + directions[i], 0.2f);

                    if (colCheck.Length == 0)
                    {
                        GameObject newTile = Instantiate(tile, transform.position + directions[i], Quaternion.identity);
                        newTile.GetComponent<VectorRemovalTest>().axisRules = new List<int> { 2, 2, 2, 2 };
                        newTile.GetComponent<VectorRemovalTest>().axisRules[2] = int.Parse(seq[3].ToString());
                    }
                    else if (colCheck[0].tag.Equals("Tile"))
                    {
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().axisRules[2] = int.Parse(seq[3].ToString());
                        colCheck[0].gameObject.GetComponent<VectorRemovalTest>().ExecuteSearch();
                    }
                }
            }

        }
        rTM.onHold.Remove(gameObject);
        Destroy(gameObject);
    }
    void Update()
    {

    }
}
