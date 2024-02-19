using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoadTileManagement : MonoBehaviour
{
    public List<GameObject> onHold = new List<GameObject>();
    public bool canCheck;
    int counter;
    public Vector2 gridSize;
    public VectorRemovalTest vrt;
    public int once;
    bool trig;
    public bool collectedEmpty = true;

    public int blockCount;
    int countNeeded;
    Vector3[] directions = { new Vector3(5, 0, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, -5), new Vector3(0, 0, 5) };

    public List<GameObject> connectedRoads = new List<GameObject>();
    public Dictionary<Vector3, GameObject> allRoads = new Dictionary<Vector3, GameObject>();
    Dictionary<Vector3, int> checkDir = new Dictionary<Vector3, int>(0);

    public List<GameObject> cluster = new List<GameObject>();
    List<List<GameObject>> fullClusters = new List<List<GameObject>>();
    int clusterNr;

    Queue<Vector3> toCheckPos = new Queue<Vector3>();


    public void EntropySelector()
    {
        VectorRemovalTest[] test1 = null;
        test1 = FindObjectsOfType<VectorRemovalTest>();
        List<VectorRemovalTest> possibleTiles = new List<VectorRemovalTest>();
        possibleTiles = test1.ToList();

        int lowestEntropy = 100;
        VectorRemovalTest currentSelected = null;
        if (possibleTiles.Count > 0)
        {
            lowestEntropy = possibleTiles[0].entropy;
            currentSelected = possibleTiles[0];
        }
        foreach (var item in possibleTiles)
        {
            if (item.entropy < lowestEntropy)
            {
                lowestEntropy = item.entropy;
                currentSelected = item;
            }
        }

        if (currentSelected != null)
        {
            currentSelected.GenerateRoadTile();
        }
        counter++;
    }
    public void Selector()
    {

        GameObject selectCurrent = null;
        int entropyLowest = 100;
        if (onHold.Count > 0)
        {
            foreach (var item in onHold)
            {
                if (item == null) continue;
                VectorRemovalTest vt = item.GetComponent<VectorRemovalTest>();
                if (vt.entropy < entropyLowest)
                {
                    entropyLowest = vt.entropy;
                    selectCurrent = vt.gameObject;

                }
            }
        }
        if (selectCurrent != null)
        {
            onHold.Remove(selectCurrent);
            selectCurrent.GetComponent<VectorRemovalTest>().GenerateRoadTile();
        }

    }

    void CheckRoads()
    {
        while (toCheckPos.Count > 0)
        {
            if (toCheckPos.Count <= 0) break;
            GameObject tileOfInterest = allRoads[toCheckPos.Dequeue()];
            Vector3 toiPos = tileOfInterest.transform.position;

            for (int i = 0; i < 4; i++)
            {
                if (allRoads.ContainsKey(toiPos + directions[i]) && !toCheckPos.Contains(toiPos + directions[i]))
                {
                    TileProperties tp = tileOfInterest.GetComponent<TileProperties>();
                    TileProperties tpNext = allRoads[toiPos + directions[i]].GetComponent<TileProperties>();

                    if (tp.sideKeys[i] == 1 && tpNext.sideKeys[checkDir[directions[i]]] == 1)
                    {
                        toCheckPos.Enqueue(toiPos + directions[i]);
                    }
                }
            }
            cluster.Add(allRoads[toiPos]);
            fullClusters[clusterNr].Add(allRoads[toiPos]);
            connectedRoads.Remove(allRoads[toiPos]);
            allRoads.Remove(toiPos);
        }
        if (connectedRoads.Count > 0 && toCheckPos.Count == 0)
        {
            toCheckPos.Enqueue(connectedRoads[0].transform.position);
            fullClusters.Add(new List<GameObject>());
            clusterNr++;
            CheckRoads();
        }
    }

    void Start()
    {
        countNeeded = ((int)gridSize.x * 2) * ((int)gridSize.y * 2);
        for (int i = 0; i < 4; i++)
        {
            int temp;
            if (i == 0) temp = 1;
            else if (i == 1) temp = 0;
            else if (i == 2) temp = 3;
            else temp = 2;
            checkDir.Add(directions[i], temp);
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            CityGenerator cg = FindObjectOfType<CityGenerator>();
            cg.ContinuousBuildings();
        }
        else if (Input.GetKey(KeyCode.O))
        {
            foreach (var item in cluster)
            {
                item.gameObject.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            for (int i = 0; i < fullClusters.Count; i++)
            {
                Debug.Log("Length of Cluster: " + i + " = " + fullClusters[i].Count);
            }
        }
        else if (onHold.Count > 0)
        {
            Selector();
        }
        else if (blockCount == countNeeded && !trig)
        {

            trig = true;

            toCheckPos.Enqueue(connectedRoads[0].transform.position);
            fullClusters.Add(new List<GameObject>());

            CheckRoads();
            ClusterConnect cc = FindObjectOfType<ClusterConnect>();
            cc.clusters = fullClusters;

            cc.CheckClusters();

        }
        else if (!collectedEmpty && fullClusters.Count ==1)
        {

            CityGenerator cg = FindObjectOfType<CityGenerator>();
            cg.ContinuousBuildings();
            collectedEmpty = true;
        }


        

    }


}
