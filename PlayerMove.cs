using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{
    private int width;
    private int height;
	// Use this for initialization
	void Start ()
    {
        width = GameObject.Find("MapCreater").GetComponent<MapCreater>().Width;
        height = GameObject.Find("MapCreater").GetComponent<MapCreater>().Height;
        Initialize();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.DrawRay(transform.position, transform.forward);
        if (!IsMoving)
        {
            FindSelectableTiles(); //TacticsMove에서 정의한 함수 (선택 가능한 타일들을 찾는 알고리즘 적용)
            checkMouse();
            chechKeyBoard();
        }
        else
        {
            Move();
        }
	}

    private void checkMouse() //마우스 이동
    {
        if(Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //returns a ray going from camera through a screen point
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.tag == "Tile")
                {
                    Tile t = hit.collider.GetComponent<Tile>(); //Raycast에 닿은 타일 스크립트 정보
                    if(t.IsSelectable)
                    {
                        MoveToTile(t);
                    }
                }
            }
        }
    }

    private void chechKeyBoard() //키보드 이동
    {
        GetCurrentTile(); //현재 타일 정보 얻어옴
        if(CurrentTile.GetTileIndex() == 1) //Bottom Left
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
        }
        else if(CurrentTile.GetTileIndex() == width) //Bottom Right
        {
            if(Input.GetKeyDown(KeyCode.RightArrow)||Input.GetKeyDown(KeyCode.DownArrow))
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
        }
        else if(CurrentTile.GetTileIndex() < width) //Bottom
        {
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[2]);
            }
        }
        else if(CurrentTile.GetTileIndex() == width * (height-1) + 1) //Top Left
        {
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
        }
        else if(CurrentTile.GetTileIndex() == width * height) //Top Right
        {
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
        }
        else if(CurrentTile.GetTileIndex() < width * height && CurrentTile.GetTileIndex() > width * (height -1) + 1) //Top
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[2]);
            }
        }
        else if(CurrentTile.GetTileIndex() % width == 1) //Left
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                return;
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[2]);
            }
        }
        else if(CurrentTile.GetTileIndex() % width == 0) //Right
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[2]);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[0]);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[1]);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[2]);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToTile(CurrentTile.AdjacencyList[3]);
            }
        }
    }
}
