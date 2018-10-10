using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreater : MonoBehaviour
{
    [SerializeField]
    private int width = 12;
    [SerializeField]
    private int height = 12;
    [SerializeField]
    private GameObject tile;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject enemy;

    private int randomX;
    private int randomZ;
	// Use this for initialization
	void Start ()
    {
        for(int i=0; i<width; i++)
        {
            for(int j=0; j<height; j++)
            {
                Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity, GameObject.Find("Map").transform);
            }
        }
        setPlayer();
	}
	
    private void setPlayer()
    {
        Instantiate(player, new Vector3(Random.Range(0, width), 1.5f, Random.Range(0, height)), Quaternion.identity);
        Instantiate(enemy, new Vector3(Random.Range(0, width), 1.5f, Random.Range(0, height)), Quaternion.identity);
    }
}
