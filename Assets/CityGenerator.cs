using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{

    GameObject[] buildings1x1, buildings1x2, buildings2x1, buildings2x2;
    public List<GameObject> noRoadList = new List<GameObject>();
    Vector3[] directions = { new Vector3(-5, 0, 0), new Vector3(0, 0, 5), new Vector3(-5, 0, 5) };
    List<string> possibleShapes = new List<string> {"1x1","1x2","2x1","2x2"};

    void Start()
    {
        buildings1x1 = Resources.LoadAll("Prefabs/1x1", typeof(GameObject)).Cast<GameObject>().ToArray();
        buildings1x2 = Resources.LoadAll("Prefabs/1x2", typeof(GameObject)).Cast<GameObject>().ToArray();
        buildings2x1 = Resources.LoadAll("Prefabs/2x1", typeof(GameObject)).Cast<GameObject>().ToArray();
        buildings2x2 = Resources.LoadAll("Prefabs/2x2", typeof(GameObject)).Cast<GameObject>().ToArray();
    }

    public void ContinuousBuildings()
    {
        int index = 0;
        foreach (var item in GameObject.FindGameObjectsWithTag("BuildingSpot"))
        {
            //Debug.Log(item.name + " " + item.transform.position);
            //Debug.Log(index);
            index++;
            noRoadList.Add(item);
        }
        /*
        while (noRoadList.Count>0)
        {
            GenerateBuildings();
        }
        */
    }

    void GenerateBuildings()
    {
        bool validFound = false;
        GameObject target = null;
        while (!validFound)
        {
            target = noRoadList[0];
            noRoadList.RemoveAt(0);
            Collider col = target.transform.GetComponent<Collider>();


            if(col.tag.Equals("BuildingSpot"))
            {
                validFound = true;
            }
        }
        List<string> possibles = new List<string>(possibleShapes);
        for (int i = 0; i < directions.Length; i++)
        {
            Collider[] colliders = Physics.OverlapSphere((target.transform.position + new Vector3(0, 3, 0)) + directions[i],0.5f);
            if (i == 0)
            {
                if (colliders.Length > 0)
                {
                    foreach (var item in colliders)
                    {
                        if (!item.tag.Equals("BuildingSpot"))
                        {
                            possibles.Remove(possibleShapes[1].ToString());
                            possibles.Remove(possibleShapes[3].ToString());
                        }
                    }
                }
                else
                {
                    possibles.Remove(possibleShapes[1].ToString());
                    possibles.Remove(possibleShapes[3].ToString());
                }
            }else if (i == 1)
            {
                if (colliders.Length > 0)
                {
                    foreach (var item in colliders)
                    {
                        if (!item.tag.Equals("BuildingSpot"))
                        {
                            possibles.Remove(possibleShapes[2].ToString());
                            possibles.Remove(possibleShapes[3].ToString());
                        }
                    }
                }
                else
                {
                    possibles.Remove(possibleShapes[2].ToString());
                    possibles.Remove(possibleShapes[3].ToString());
                }
            }
            else if(i == 2)
            {
                if (colliders.Length > 0)
                {
                    foreach (var item in colliders)
                    {
                        if (!item.tag.Equals("BuildingSpot"))
                        {
                            possibles.Remove(possibleShapes[3].ToString());
                        }
                    }
                }
                else
                {
                    possibles.Remove(possibleShapes[3].ToString());
                }
            }
        }
        string randomChoice = possibles[Random.Range(0,possibles.Count)];
        if (randomChoice.Equals("1x1"))
        {
            int randomStructure = Random.Range(0, buildings1x1.Length);
            Instantiate(buildings1x1[randomStructure], target.transform.position+new Vector3(0, buildings1x1[randomStructure].transform.localScale.y/2,0),Quaternion.identity);
            Destroy(target);
        }else if (randomChoice.Equals("1x2"))
        {
            int randomStructure = Random.Range(0, buildings1x2.Length);
            Collider[] cols = Physics.OverlapSphere((target.transform.position + new Vector3(0,3,0))+directions[0],0.5f);
            if (cols.Length > 0)
            {
                foreach (var item in cols)
                {
                    if (item.tag.Equals("BuildingSpot"))
                    {
                        noRoadList.Remove(item.gameObject);
                        Destroy(item.gameObject);
                    }
                }
            }
            Instantiate(buildings1x2[randomStructure], target.transform.position + new Vector3(-2.5f, buildings1x2[randomStructure].transform.localScale.y / 2, 0), Quaternion.identity);
            Destroy(target);
        }else if (randomChoice.Equals("2x1"))
        {
            int randomStructure = Random.Range(0, buildings2x1.Length);
            Collider[] cols = Physics.OverlapSphere((target.transform.position + new Vector3(0, 3, 0)) + directions[1], 1f);
            //Debug.Log(cols.Length);
            if (cols.Length > 0)
            {
                foreach (var item in cols)
                {
                    if (item.gameObject.tag.Equals("BuildingSpot"))
                    {
                        noRoadList.Remove(item.gameObject);
                        Destroy(item.gameObject);
                    }
                }
            }
            Instantiate(buildings2x1[randomStructure], target.transform.position + new Vector3(0, buildings2x1[randomStructure].transform.localScale.y / 2, 2.5f), Quaternion.identity);
            Destroy(target);
        }
        else if (randomChoice.Equals("2x2"))
        {
            int randomStructure = Random.Range(0, buildings2x2.Length);
            for (int i = 0; i < 3; i++)
            {
                Collider[] cols = Physics.OverlapSphere((target.transform.position + new Vector3(0, 3, 0)) + directions[i], 0.5f);
                if (cols.Length > 0)
                {
                    foreach (var item in cols)
                    {
                        if (item.tag.Equals("BuildingSpot"))
                        {
                            noRoadList.Remove(item.gameObject);
                            Destroy(item.gameObject);
                        }
                    }
                }
            }
            
            Instantiate(buildings2x2[randomStructure], target.transform.position + new Vector3(-2.5f, buildings2x2[randomStructure].transform.localScale.y / 2, 2.5f), Quaternion.identity);
            Destroy(target);
        }
        

    }
}
