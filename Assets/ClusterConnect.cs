using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterConnect : MonoBehaviour
{

    public List<List<GameObject>> clusters = new List<List<GameObject>>();
    int smallestClusterSize;
    int clusterOfInterest;
    RoadTileManagement rtm;
    Vector2 boundries;
    Vector3[] directions = { new Vector3(5, 0, 0), new Vector3(-5, 0, 0), new Vector3(0, 0, -5), new Vector3(0, 0, 5) };
    public GameObject[] tTiles;
    public GameObject[] connectingTiles;

    public GameObject test;
    int clusterElementIndex = 0;
    int breaks;

    Dictionary<Vector3, int> checkDir = new Dictionary<Vector3, int>(0);
    Dictionary<string, int> sequenceMap = new Dictionary<string, int>
{
    { "1011", 0 },
    { "0111", 1 },
    { "1110", 2 },
    { "1101", 3 },
    { "1111", 4 }
};


    public void ClusterFix()
    {
        if (clusters.Count <= 1) return;

        int countFor = 0;
        smallestClusterSize = clusters[0].Count;
        clusterOfInterest = 0;
        for (int i = 0; i < clusters.Count; i++)
        {
            if (clusters[i].Count < smallestClusterSize)
            {
                smallestClusterSize = clusters[i].Count;
                clusterOfInterest = i;
            }
        }
        GameObject clusterElement = clusters[clusterOfInterest][clusterElementIndex];
        TileProperties tp = clusterElement.GetComponent<TileProperties>();
        clusterElementIndex++;

        List<Vector3> strLine = new List<Vector3>();
        List<GameObject> lineObj = new List<GameObject>();
        GameObject first, last;
        first = clusterElement;
        for (int i = 0; i < 4; i++)
        {
            if (first.GetComponent<TileProperties>().sideKeys[i] == 1) continue;
            while (true)
            {
                Collider[] tempCols = Physics.OverlapSphere(clusterElement.transform.position + (directions[i] * countFor), 1f);
                if (tempCols.Length > 0)
                {
                    foreach (var item in tempCols)
                    {
                        if (item.gameObject.TryGetComponent<TileProperties>(out TileProperties tempProp))
                        {
                            strLine.Add(item.gameObject.transform.position);
                            lineObj.Add(item.gameObject);
                            for (int j = 0; j < clusters.Count; j++)
                            {
                                if (clusters[j].Contains(item.gameObject) && clusterOfInterest != j)
                                {
                                    last = item.gameObject;
                                    TileProperties firstTP = first.GetComponent<TileProperties>();
                                    TileProperties lastTP = tempProp;
                                    firstTP.sideKeys[i] = 1;
                                    lastTP.sideKeys[checkDir[directions[i]]] = 1;
                                    string firstSeq = firstTP.sideKeys[0].ToString() + firstTP.sideKeys[1].ToString() + firstTP.sideKeys[2].ToString() + firstTP.sideKeys[3].ToString();
                                    string lastSeq = lastTP.sideKeys[0].ToString() + lastTP.sideKeys[1].ToString() + lastTP.sideKeys[2].ToString() + lastTP.sideKeys[3].ToString();
                                    Collider[] toDel = Physics.OverlapSphere(strLine[0],1f);
                                    foreach (var item2 in toDel)
                                    {
                                        Debug.Log("DESTROYING FIRST AT : " + item2.gameObject.transform.position);
                                        Destroy(item2.gameObject);
                                    }
                                    Instantiate(tTiles[sequenceMap[lastSeq]], strLine[0], Quaternion.identity);
                                    //Destroy(lineObj[0].gameObject);
                                    strLine.RemoveAt(0);
                                    Collider[] toDel2 = Physics.OverlapSphere(item.gameObject.transform.position, 1f);
                                    foreach (var item2 in toDel2)
                                    {
                                        Debug.Log("DESTROYING Second AT : " + item2.gameObject.transform.position);
                                        Destroy(item2.gameObject);
                                    }
                                    Instantiate(tTiles[sequenceMap[firstSeq]], item.gameObject.transform.position, Quaternion.identity);
                                    Destroy(item.gameObject);
                                }
                            }
                        }
                    }
                }
                else
                    break;
                countFor++;
            }
        }
    }

    public void CheckClusters()
    {

        while (clusters.Count > 1)
        {
            breaks++;
            if (breaks > 15)
            {
                return;
            }
            smallestClusterSize = clusters[0].Count;
            clusterOfInterest = 0;
            for (int i = 0; i < clusters.Count; i++)
            {
                if (clusters[i].Count < smallestClusterSize)
                {
                    smallestClusterSize = clusters[i].Count;
                    clusterOfInterest = i;
                }
            }
            int randomClusterElementIndex = Random.Range(0, smallestClusterSize);
            GameObject clusterElement = clusters[clusterOfInterest][clusterElementIndex];
            TileProperties tp = clusterElement.GetComponent<TileProperties>();
            clusterElementIndex++;
            //if (clusterElement.tag.Equals("TCross")) continue;

            int countFor = 0;
            for (int i = 0; i < 4; i++)
            {
                if (tp.sideKeys[i] == 0)
                {
                    List<Vector3> tempDirCheck = new List<Vector3>();
                    Vector3 tileCheck = clusterElement.transform.position;
                    tempDirCheck.Add(tileCheck);

                    while ((tileCheck.x < boundries.x * 5) &&
                        (tileCheck.x > -(boundries.x + 1) * 5) &&
                        (tileCheck.z > -boundries.y * 5) &&
                        (tileCheck.z < (boundries.y + 1) * 5))
                    {

                        countFor++;
                        if (countFor >= 135)
                        {
                            //Instantiate(tTiles[sequenceMap[personalSeq]], clusterElement.transform.position, Quaternion.identity);
                            break;
                        }
                        tileCheck = clusterElement.transform.position + (directions[i] * countFor);
                        tempDirCheck.Add(tileCheck);

                        Collider[] cols = Physics.OverlapSphere(tileCheck, 1f);
                        foreach (var col in cols)
                        {
                            for (int j = 0; j < clusters.Count; j++)
                            {
                                if (clusters[j].Contains(col.gameObject))
                                {
                                    if (clusterOfInterest != j)
                                    {
                                        if (clusters.Count == 1)
                                        {
                                            rtm.collectedEmpty = false;
                                            return;
                                        }

                                        countFor = 0;
                                        List<int> otherSeqTemp = col.gameObject.GetComponent<TileProperties>().sideKeys;
                                        otherSeqTemp[checkDir[directions[i]]] = 1;
                                        string otherSeq = otherSeqTemp[0].ToString() + otherSeqTemp[1].ToString() + otherSeqTemp[2].ToString() + otherSeqTemp[3].ToString();
                                        tp = clusterElement.GetComponent<TileProperties>();
                                        List<int> seqTemp = tp.sideKeys;
                                        seqTemp[i] = 1;
                                        string personalSeq = seqTemp[0].ToString() + seqTemp[1].ToString() + seqTemp[2].ToString() + seqTemp[3].ToString();

                                        Vector3 newPositionOfRoad = clusterElement.transform.position;
                                        Destroy(clusterElement.gameObject);
                                        Instantiate(tTiles[sequenceMap[personalSeq]], newPositionOfRoad, Quaternion.identity);

                                        if (!sequenceMap.ContainsKey(otherSeq))
                                        {
                                            break;
                                        }
                                        if (col.gameObject.tag.Equals("TCross"))
                                        {
                                            Instantiate(test, tileCheck, Quaternion.identity);
                                            Instantiate(tTiles[4], tileCheck, Quaternion.identity);
                                        }
                                        else if (tp.gameObject.tag.Equals("TCross"))
                                        {
                                            Instantiate(test, clusterElement.transform.position, Quaternion.identity);
                                            Instantiate(tTiles[4], clusterElement.transform.position, Quaternion.identity);
                                        }
                                        else
                                        {
                                            Collider[] toDel = Physics.OverlapSphere(tileCheck, 1f);
                                            foreach (var item in toDel)
                                            {
                                                Debug.Log(item.gameObject.name + " " + item.transform.position);
                                                Destroy(item.gameObject);
                                            }
                                            Instantiate(tTiles[sequenceMap[otherSeq]], tileCheck, Quaternion.identity);
                                            // Instantiate(test, tileCheck, Quaternion.identity);
                                        }
                                        int horizOrVertIndex;
                                        if (i < 2)
                                        {
                                            horizOrVertIndex = 0;
                                        }
                                        else
                                        {
                                            horizOrVertIndex = 1;
                                        }


                                        //for (int k = 0; k < tempDirCheck.Count; k++)
                                        //{
                                        //    Collider[] tileCols = Physics.OverlapSphere(tempDirCheck[k], 1f);
                                        //    foreach (var item in tileCols)
                                        //    {
                                        //        if (!item.tag.Equals("TCross"))
                                        //            Destroy(item.gameObject);
                                        //    }
                                        //}
                                        for (int k = 1; k < tempDirCheck.Count - 1; k++)
                                        {
                                            Collider[] colsSphere = Physics.OverlapSphere(tempDirCheck[k], 1f);
                                            foreach (var item in colsSphere)
                                            {
                                                Destroy(item.gameObject);
                                            }
                                            GameObject g = Instantiate(connectingTiles[horizOrVertIndex], tempDirCheck[k], Quaternion.identity);
                                            clusters[j].Add(g);
                                        }
                                        Debug.Log(clusters.Count);
                                        Debug.Log(clusters[clusterOfInterest].Count); ///////
                                        Debug.Log(clusters[j].Count);
                                        for (int k = 0; k < clusters[clusterOfInterest].Count; k++)
                                        {
                                            clusters[j].Add(clusters[clusterOfInterest][k]);
                                            //Destroy(clusters[clusterOfInterest][k]);
                                        }
                                        clusters.RemoveAt(clusterOfInterest);
                                        clusterElementIndex = 0;
                                        CheckClusters();
                                    }
                                    else break;
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            int temp;
            if (i == 0) temp = 1;
            else if (i == 1) temp = 0;
            else if (i == 2) temp = 3;
            else temp = 2;
            checkDir.Add(directions[i], temp);
        }
        rtm = FindObjectOfType<RoadTileManagement>();
        boundries = rtm.gridSize;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
