using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool IsWalkable = true; // 걸을 수 있는 타일인가?
    public bool IsCurrent = false;
    public bool IsTarget = false;
    public bool IsSelectable = false;

    public List<Tile> AdjacencyList = new List<Tile>(); //Adjacency List = 인접 리스트

    //BFS (Breath First Search)
    public bool IsVisited = false;
    public Tile ParentTile = null;
    public int Distance = 0;

    //A star
    public float f = 0.0f; //g+h cost
    public float g = 0.0f; //부모 타일로부터 현재 타일까지의 비용
    public float h = 0.0f; //현재 타일부터에서 목적지까지의 비용

    public void Reset() //변수들을 초기 상태로 되돌리는 함수
    {
        IsWalkable = true;
        IsCurrent = false;
        IsTarget = false;
        IsSelectable = false;

        AdjacencyList.Clear();

        IsVisited = false;
        ParentTile = null;
        Distance = 0;

        f = g = h = 0;
    }

    public void FindNeighbors(Tile targetTile)
    {
        Reset();
        CheckTile(Vector3.forward, targetTile);
        CheckTile(Vector3.back, targetTile);
        CheckTile(Vector3.left, targetTile);
        CheckTile(Vector3.right, targetTile);
    }

    public void CheckTile(Vector3 direction, Tile targetTile) //매개변수로 들어온 방향의 타일을 체크
    {
        Vector3 halfExtents = new Vector3(0.25f, 0.5f, 0.25f); //타일의 중간부분을 알아내야 함 (Extent = 범위)
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents); //모든 접촉한 콜라이더를 반환 (center of box, half of the size ot the box)

        foreach(Collider c in colliders)
        {
            Tile tile = c.GetComponent<Tile>();
            if(tile != null && tile.IsWalkable) //tile이 아닌 것과 걸을 수 없는 타일은 무시
            {
                RaycastHit hit;
                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1) || tile == targetTile) //타일 위에 무언가 있으면 갈 수 없으므로 Raycast로 현재 타일의 1범위만큼을 확인(확인해서 부딪힌게 없으면 인접 리스트에 추가) 
                {
                    AdjacencyList.Add(tile);
                }
            }
        }
    }

}
