using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreater : MonoBehaviour
{
    public int Width = 12;
    public int Height = 12;
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
        for(int i=0; i<Width; i++)
        {
            for(int j=0; j<Height; j++)
            {
                GameObject newTile = Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity, GameObject.Find("Map").transform);
                newTile.gameObject.GetComponent<Tile>().SetTileIndex((i+1) + Height * j);
            }
        }
        setPlayer();
	}
	
    private void setPlayer()
    {
        Instantiate(player, new Vector3(Random.Range(0, Width), 1.5f, Random.Range(0, Height)), Quaternion.identity);
        Instantiate(enemy, new Vector3(Random.Range(0, Width), 1.5f, Random.Range(0, Height)), Quaternion.identity);
    }
}
