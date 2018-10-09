using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMove : TacticsMove
{
    GameObject target;
	// Use this for initialization
	void Start ()
    {
        Initialize();
	}

    // Update is called once per frame
    void Update()
    {
        if (!IsMoving)
        {
            findNearestTarget(); //target변수에 가장 가까운 타겟을 넣어주는 함수
            calculatePath();
            FindSelectableTiles(); //TacticsMove에서 정의한 함수 (선택 가능한 타일들을 찾는 알고리즘 적용)
            actualTargetTile.IsTarget = true;
        }
        else
        {
            Move();
        }
    }
    private void calculatePath()
    {
        Tile targetTile = GetTargetTile(target);
        FindPath(targetTile);
    }

    private void findNearestTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        GameObject nearestTarget = null;
        float distance = Mathf.Infinity;

        foreach(GameObject obj in targets)
        {
            float d = Vector3.Distance(transform.position, obj.transform.position); //npc와 플레이어 태그가 붙은 오브젝트 사이의 거리
            if(d < distance)
            {
                distance = d; //가장 가까이 있는 거리를 업데이트 하고 가장 가까이 있는 타겟도 업데이트 (가장 가까운걸 찾을때까지 업데이트)
                nearestTarget = obj;
            }
        }
        target = nearestTarget;
    }
}
