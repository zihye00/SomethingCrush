using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//3�� �̻� �ݺ� �ȵǵ��� �ϴ� �� ���� �ȵǴ� �� ����... �� �� �غ���!
//�߰� ������ ���� : 1. ���α׷��Ӱ� �ƴ� ������ �����ϱ� ������ �ϴ� �� 2. ������Ҹ� ��� ������ �� ������?  3. �׸��带 ���簢������ �Ϸ���? 4...
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public List<Sprite> Sprites = new List<Sprite>();
    public GameObject TilePrefab;
    public int GridDimension = 8;
    public float Distance = 1.0f;
    private GameObject[,] Grid;


    //ppopulate : ä���

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
        Vector3 positionOffset = transform.position - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0); //1 �׸��� ��ġ �߾ӿ� ������ ������ ���

        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) //2 2���� �迭 ���� �ݺ���
            {
                List<Sprite> possibleSprites = new List<Sprite>(Sprites); //1 -> ??
                
                //�� ���� ���� ��������Ʈ ����
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

                GameObject newTile = Instantiate(TilePrefab); //3 �������� ����� ���ο� �� ����
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>(); //4 ��������Ʈ������ ���� ��ҿ� ���� ���� ������ ���� ��������Ʈ ����
                //renderer.sprite = Sprites[Random.Range(0, Sprites.Count)]; //5 ��������Ʈ �Ҵ�. ������ ���
                renderer.sprite = possibleSprites[Random.Range(0,possibleSprites.Count)];
                 
                Tile tile = newTile.AddComponent<Tile>();
                tile.position = new Vector2Int(column, row);
                newTile.transform.parent = transform; //6 �θ� ����
                newTile.transform.position = new Vector3(column *  Distance, row * Distance, 0) + positionOffset; //7 ������ �߰��� ���ӿ�����Ʈ �׸����� ��ġ�� �ش��ϴ� �߽��� �׸��带 ����
                Grid[column, row] = newTile; //8 ���� ������ ���� ���� ���� ����
            }
        }
    }

    // Ÿ�� �̹����� �����̻� ��ġ�� �ʵ��� �ϴ� ��
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

    public void SwapTiles(Vector2Int tile1Position, Vector2Int tile2Position) //1. ��ġ ǥ��, ���� ���� ��� Ȱ�� ���� Vector2Int ��ü ���. ��,�� �ε��� ǥ���� ����
    {
        //2 �� ���� SpriteRenderer�� ������
        GameObject tile1 = Grid[tile1Position.x, tile1Position.y];
        SpriteRenderer renderer1 = tile1.GetComponent<SpriteRenderer>();

        GameObject tile2 = Grid[tile2Position.x, tile2Position.y];
        SpriteRenderer renderer2 = tile2.GetComponent<SpriteRenderer>();

        // 3. ��ȯ.
        Sprite temp = renderer1.sprite;
        renderer1.sprite = renderer2.sprite;
        renderer2.sprite = temp;

        //���Ⱑ ����!!!!
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
