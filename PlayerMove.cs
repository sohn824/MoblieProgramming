using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : TacticsMove
{

	// Use this for initialization
	void Start ()
    {
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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CurrentTile.CheckTile(Vector3.forward, null); 
            MoveToTile(CurrentTile.AdjacencyList[0]); //위쪽 방향
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CurrentTile.CheckTile(Vector3.back, null);
            MoveToTile(CurrentTile.AdjacencyList[1]); //아래쪽 방향
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CurrentTile.CheckTile(Vector3.left, null);
            MoveToTile(CurrentTile.AdjacencyList[2]); //왼쪽 방향
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CurrentTile.CheckTile(Vector3.right, null);
            MoveToTile(CurrentTile.AdjacencyList[3]); //오른쪽 방향
        }
    }
}
