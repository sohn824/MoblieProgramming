using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMove : MonoBehaviour
{
    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;
    Stack<Tile> path = new Stack<Tile>();
    [HideInInspector]
    public Tile CurrentTile;

    private int moveRange = 20; //한 번에 움직일 수 있는 타일 수
    private float moveSpeed = 2.0f;
    private float halfHeight = 0;
    [HideInInspector]
    public bool IsMoving = false; //움직이는 중에는 알고리즘을 실행하지 않기 위해 체크할 bool변수

    Vector3 velocity = new Vector3();
    Vector3 headingDirection = new Vector3();
    [HideInInspector]
    public Tile actualTargetTile; //실제 타겟 타일

    protected void Initialize()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfHeight = GetComponent<Collider>().bounds.extents.y; //중간 위치 맞추기
    }

    public void GetCurrentTile()
    {
        CurrentTile = GetTargetTile(gameObject); //현재 이 스크립트를 상속받은 스크립트가 붙어있는 게임 객체를 인자로 넘김
        CurrentTile.IsCurrent = true;
    }

    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;
        if(Physics.Raycast(target.transform.position, Vector3.down, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>(); //Raycast로 닿은 타일정보를 가져옴
        }
        return tile;
    }

    public void ComputeAdjacencyList(Tile targetTile) //인접 리스트 계산
    {
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbors(targetTile); //주변의 Walkable Tile 조사          
        }
    }

    public void FindSelectableTiles() //BFS 알고리즘
    {
        ComputeAdjacencyList(null); //인접 리스트 계산
        GetCurrentTile(); //현재 타일 정보

        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(CurrentTile); //먼저 현재 타일을 큐에 넣는다
        CurrentTile.IsVisited = true; //다음 IsVisted 플래그를 true로 바꿔주고 (다시 돌아오지 않기 위해)

        while(process.Count > 0) //process queue에 타일이 남아있을 때 까지
        {
            Tile t = process.Dequeue(); //process queue에서 타일을 꺼내서
            selectableTiles.Add(t); //selectableTiles 리스트에 넣어준다
            t.IsSelectable = true; //그 후 selectableTiles에 넣은 타일의 IsSelectable 플래그를 true로 바꿔준다

            if (t.Distance < moveRange) //moveRange만큼만 BFS 알고리즘이 동작하도록
            {
                foreach (Tile tile in t.AdjacencyList) //selectableTiles에 넣은 타일의 인접 리스트에 있는 타일들
                {
                    if (!tile.IsVisited) //방문하지 않은 타일이라면
                    {
                        tile.ParentTile = t; //그 타일의 부모 타일을 selectableTiles에 넣은 타일로 설정
                        tile.IsVisited = true; //다시 방문하지 않도록 IsVisited 플래그 true로
                        tile.Distance = 1 + t.Distance;
                        process.Enqueue(tile); //selectableTiles에 넣은 타일의 인접 리스트에 있는 타일들을 process queue에 넣는다
                    }
                }
            }
        }
    }

    public void MoveToTile(Tile tile)
    {
        //매개변수 tile => checkMouse() 함수에서 사용자가 클릭한 마우스 위치의 tile
        path.Clear(); //경로 스택 초기화
        tile.IsTarget = true;
        IsMoving = true;

        Tile nextTile = tile;
        while(nextTile != null)
        {
            path.Push(nextTile); //경로에 다음 타일 넣고
            nextTile = nextTile.ParentTile; //다음 타일 최신화
            //경로는 스택에 역순으로 저장되게 된다
        }
    }

    public void Move()
    {
        if(path.Count > 0) //path 이동중..
        {
            Tile t = path.Peek(); //캐릭터가 도달할때까지 삭제하면 안되므로 path스택에서 Peek()으로 정보만 가져온다
            Vector3 target = t.transform.position;

            //타켓 타일의 위에 있는 유닛의 포지션 계산
            target.y += halfHeight + t.GetComponent<Collider>().bounds.extents.y; //Move하는 유닛의 halfHeight + 현재 타일의 halfHeight

            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                calculateHeading(target);
                setHorizontalVelocity();

                transform.forward = headingDirection;
                transform.position += velocity * Time.deltaTime;
            }
            else //움직이는 유닛과 타겟 타일의 거리가 충분히 가깝다면 위치를 중간으로 자연스럽게 보정해줌
            {
                //Tile center reached
                transform.position = target;
                path.Pop(); //path 스택에서 도달한 타겟 타일을 제거
            }

        }
        else //path이동 종료 (path 스택에서 더 이상 Pop할것이 없으므로 이동 종료를 알 수 있다)
        {
            RemoveSelectableTiles();
            IsMoving = false;
        }
    }

    protected void RemoveSelectableTiles()
    {
        if(CurrentTile != null)
        {
            CurrentTile.IsCurrent = false;
            CurrentTile = null;
        }
        foreach(Tile tile in selectableTiles)
        {
            tile.Reset();

        }
        selectableTiles.Clear();
    }

    private void calculateHeading(Vector3 target) //이동하는 방향 계산
    {
        headingDirection = target - transform.position;
        headingDirection.Normalize();
    }

    private void setHorizontalVelocity()
    {
        velocity = headingDirection * moveSpeed;
    }


    public void FindPath(Tile targetTile) //A-star
    {
        ComputeAdjacencyList(targetTile);
        GetCurrentTile();

        List<Tile> openList = new List<Tile>(); //any tile that has not been processed
        List<Tile> closedList = new List<Tile>(); //every tile that has been processed

        openList.Add(CurrentTile);
        //currentTile.g == 0; (부모타일부터 현재타일까지의 비용) (처음에 같은위치에 있으므로)
        CurrentTile.h = Vector3.Distance(CurrentTile.transform.position, targetTile.transform.position);
        CurrentTile.f = CurrentTile.h; //f = g+h

        while(openList.Count > 0)
        {
            Tile t = FindLowestF(openList); //openList에서 가장 낮은 f를 찾기 (openList에서 제외됨)
            closedList.Add(t); //openList에서 뺀 가장 낮은 f의 타일을 closedList에 넣기 (이 타일을 다시 처리하지 않기 위해)

            if(t == targetTile)
            {
                actualTargetTile = FindEndTile(t);
                MoveToTile(actualTargetTile);
                return;
            }

            foreach(Tile tile in t.AdjacencyList) //가장 낮은 f의 타일의 인접 타일들
            {
                if(closedList.Contains(tile)) //타일이 closedList에 있을 경우
                {
                    //Do nothing, already processed
                }
                else if(openList.Contains(tile)) //타일이 openList에 있을 경우
                {
                    float tmpG = t.g + Vector3.Distance(tile.transform.position, t.transform.position);

                    if(tmpG < tile.g)
                    {
                        tile.ParentTile = t; //조건을 만족한다면 더 빠른 길을 찾았으므로
                        tile.g = tmpG;
                        tile.f = tile.g + tile.h;
                    }
                }
                else //타일이 어느 곳에도 속해있지 않을 경우
                {
                    tile.ParentTile = t;
                    tile.g = t.g + Vector3.Distance(tile.transform.position, t.transform.position); //g = 부모 타일부터 현재 타일까지의 비용 (새로운 타일의 g를 구함)
                    tile.h = Vector3.Distance(tile.transform.position, targetTile.transform.position); //h = 현재 타일부터 목적지까지 비용
                    tile.f = tile.g + tile.h; //f = g+h

                    openList.Add(tile); //타일의 f,g,h값을 계산한 후 openList에 추가 (openList에 있다는 것은 나중에 처리될 것임을 의미)
                }
            }
        }
    }

    public Tile FindLowestF(List<Tile> list)
    {
        Tile lowest = list[0];
        foreach(Tile t in list)
        {
            if(t.f < lowest.f)
            {
                lowest = t;
            }
        }
        list.Remove(lowest); //openList에서 가장 낮은f를 가진 타일을 빼내서 closedList에 집어넣기 위해

        return lowest;
    }

    public Tile FindEndTile(Tile t)
    {
        Stack<Tile> tmpPath = new Stack<Tile>();

        Tile next = t.ParentTile;
        while(next != null)
        {
            tmpPath.Push(next);
            next = next.ParentTile; //ParentTile이 없다면 시작한 타일이고 while문이 끝나게 됨
        }

        if(tmpPath.Count <= moveRange) //범위 내의 경로라면
        {
            return t.ParentTile;
        }

        Tile endTile = null;
        for(int i=0; i<moveRange; i++)
        {
            endTile = tmpPath.Pop(); //범위만큼만 tmpPath 스택에서 Pop한다
        }

        return endTile;
    }
}
