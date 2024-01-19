using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//3개 이상 반복 안되도록 하는 것 실행 안되는 것 같다... 좀 더 해보기!
//추가 생각할 사항 : 1. 프로그래머가 아닌 팀원이 조작하기 쉽도록 하는 법 2. 구성요소를 어떻게 변경할 수 있을까?  3. 그리드를 직사각형으로 하려면? 4...
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public List<Sprite> Sprites = new List<Sprite>();
    public GameObject TilePrefab;
    public int GridDimension = 8;
    public float Distance = 1.0f;
    private GameObject[,] Grid;


    //ppopulate : 채우다

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Grid = new GameObject[GridDimension, GridDimension];
        InitGrid();
    }

    void InitGrid()
    {
        Vector3 positionOffset = transform.position - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0); //1 그리드 위치 중앙에 오도록 오프셋 계산

        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) //2 2차원 배열 이중 반복문
            {
                List<Sprite> possibleSprites = new List<Sprite>(Sprites); //1 -> ??
                
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

                GameObject newTile = Instantiate(TilePrefab); //3 프리팹을 사용해 새로운 셀 생성
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>(); //4 스프라이트렌더러 구성 요소에 대한 참조 가져와 셀의 스프라이트 설정
                //renderer.sprite = Sprites[Random.Range(0, Sprites.Count)]; //5 스프라이트 할당. 임의의 요소
                renderer.sprite = possibleSprites[Random.Range(0,possibleSprites.Count)];
                 
                Tile tile = newTile.AddComponent<Tile>();
                tile.position = new Vector2Int(column, row);
                newTile.transform.parent = transform; //6 부모 설정
                newTile.transform.position = new Vector3(column *  Distance, row * Distance, 0) + positionOffset; //7 오프셋 추가해 게임오브젝트 그리드의 위치에 해당하는 중심의 그리드를 얻음
                Grid[column, row] = newTile; //8 새로 생성된 셀에 대한 참조 저장
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
        GameObject tile = Grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position) //1. 위치 표현, 벡터 관련 기능 활용 위해 Vector2Int 객체 사용. 행,열 인덱스 표현에 적합
    {
        //2 두 셀의 SpriteRenderer를 가져옴
        GameObject tile1 = Grid[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = Grid[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        // 3. 교환.
        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        //여기가 문제!!!!
        bool changesOccurs = CheckMatches();
        if (!changesOccurs)
        {
            temp = renderer1.sprite;
            renderer1.sprite = renderer2.sprite;
            renderer2.sprite= temp;
        }
        else
        {
            do
            {
                FillHoles();
            }while (CheckMatches());
        }
    }

    SpriteRenderer GetSpriteRendererAt(int column, int row)
    {
        if (column < 0 || column >= GridDimension || row < 0 || row >= GridDimension)
            return null;
        GameObject tile = Grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer;
    }

    bool CheckMatches()
    {
        //HashSet matchedTiles = new System.Collections.Generic.HashSet();
        HashSet<SpriteRenderer> matchedTiles = new HashSet<SpriteRenderer> ();
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++)
            {
                SpriteRenderer current = GetSpriteRendererAt(column, row);

                List<SpriteRenderer> horizontalMatches = FindColumnMatchForTile(column, row, current.sprite);
                if (horizontalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(horizontalMatches);
                    matchedTiles.Add(current);
                }

                List<SpriteRenderer> verticalMatches = FindRowMatchForTile(column, row, current.sprite);
                if (verticalMatches.Count >= 2)
                {
                    matchedTiles.UnionWith(verticalMatches);
                    matchedTiles.Add(current);
                }
            }
        }

        foreach (SpriteRenderer renderer in matchedTiles)
        {
            renderer.sprite = null;
        }
        return matchedTiles.Count > 0;
    }

    List<SpriteRenderer> FindColumnMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = col + 1; i < GridDimension; i++)
        {
            SpriteRenderer nextColumn = GetSpriteRendererAt(i, row);
            if (nextColumn.sprite != sprite)
            {
                break;
            }
            result.Add(nextColumn);
        }
        return result;
    }


    List<SpriteRenderer> FindRowMatchForTile(int col, int row, Sprite sprite)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = row + 1; i < GridDimension; i++)
        {
            SpriteRenderer nextRow = GetSpriteRendererAt(col, i);
            if (nextRow.sprite != sprite)
            {
                break;
            }
            result.Add(nextRow);
        }
        return result;
    }

    void FillHoles()
    {
        for (int column = 0; column < GridDimension; column++)
        {
            for (int row = 0; row < GridDimension; row++)
            {
                while (GetSpriteRendererAt(column, row).sprite == null)
                {
                    for (int filler = row; filler < GridDimension - 1; filler++)
                    {
                        SpriteRenderer current = GetSpriteRendererAt(column, filler);

                        SpriteRenderer next = GetSpriteRendererAt(column, filler + 1);
                        current.sprite = next.sprite;
                    }

                    SpriteRenderer last = GetSpriteRendererAt(column, GridDimension - 1);
                    last.sprite = Sprites[Random.Range(0, Sprites.Count)];
                }
            }
        }
    }
}
