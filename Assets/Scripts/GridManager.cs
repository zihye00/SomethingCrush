using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//3개 이상 반복 안되도록 하는 것 실행 안되는 것 같다... 좀 더 해보기!
//추가 생각할 사항 : 1. 프로그래머가 아닌 팀원이 조작하기 쉽도록 하는 법 2. 구성요소를 어떻게 변경할 수 있을까?  3. 그리드를 직사각형으로 하려면? 4...
public class GridManager : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public GameObject tilePrefab;
    public int GridDimension = 8;
    public float Distance = 1.0f;
    private GameObject[,] grid;

    //ppopulate : 채우다

    private void Start()
    {
        grid = new GameObject[GridDimension, GridDimension];
        InitGrid();
    }

    void InitGrid()
    {
        Vector3 positionOffset = transform.position - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0); //1 그리드 위치 중앙에 오도록 오프셋 계산
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) //2 2차원 배열 이중 반복문
            {
                List<Sprite> possibleSprites = new List<Sprite>(); //1

                //이 셀에 사용될 스프라이트 선택
                Sprite left1 = GetSpriteAt(column - 1, row); //2
                Sprite left2 = GetSpriteAt(column - 2, row);
                if (left2 != null && left1 == left2)
                {
                    possibleSprites .Remove(left1); //4
                }

                Sprite down1 = GetSpriteAt(column, row - 1); //5
                Sprite down2 = GetSpriteAt(column, row - 2);
                if (down2 != null && down1 == down2)
                {
                    possibleSprites .Remove(down1);
                }



                GameObject newTile = Instantiate(tilePrefab); //3 프리팹을 사용해 새로운 셀 생성
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>(); //4 스프라이트렌더러 구성 요소에 대한 참조 가져와 셀의 스프라이트 설정
                renderer.sprite = sprites[Random.Range(0, sprites.Count)]; //5 스프라이트 할당. 임의의 요소
                newTile.transform.parent = transform; //6 부모 설정
                newTile.transform.position = new Vector3(column *  Distance, row * Distance, 0) + positionOffset; //7 오프셋 추가해 게임오브젝트 그리드의 위치에 해당하는 중심의 그리드를 얻음
                grid[column, row] = newTile; //8 새로 생서오딘 셀에 대한 참조 저장
            }
        }
    }

    // 타일 이미지가 세개이상 겹치지 않도록 하는 것
    Sprite GetSpriteAt(int column,  int row)
    {
        if(column < 0 || column >= GridDimension || row < 0 || row >= GridDimension)
        {
            return null;
        }
        GameObject tile = grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

}
