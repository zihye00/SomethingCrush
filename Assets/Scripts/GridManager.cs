using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//3�� �̻� �ݺ� �ȵǵ��� �ϴ� �� ���� �ȵǴ� �� ����... �� �� �غ���!
//�߰� ������ ���� : 1. ���α׷��Ӱ� �ƴ� ������ �����ϱ� ������ �ϴ� �� 2. ������Ҹ� ��� ������ �� ������?  3. �׸��带 ���簢������ �Ϸ���? 4...
public class GridManager : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public GameObject tilePrefab;
    public int GridDimension = 8;
    public float Distance = 1.0f;
    private GameObject[,] grid;

    //ppopulate : ä���

    private void Start()
    {
        grid = new GameObject[GridDimension, GridDimension];
        InitGrid();
    }

    void InitGrid()
    {
        Vector3 positionOffset = transform.position - new Vector3(GridDimension * Distance / 2.0f, GridDimension * Distance / 2.0f, 0); //1 �׸��� ��ġ �߾ӿ� ������ ������ ���
        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++) //2 2���� �迭 ���� �ݺ���
            {
                List<Sprite> possibleSprites = new List<Sprite>(); //1

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



                GameObject newTile = Instantiate(tilePrefab); //3 �������� ����� ���ο� �� ����
                SpriteRenderer renderer = newTile.GetComponent<SpriteRenderer>(); //4 ��������Ʈ������ ���� ��ҿ� ���� ���� ������ ���� ��������Ʈ ����
                renderer.sprite = sprites[Random.Range(0, sprites.Count)]; //5 ��������Ʈ �Ҵ�. ������ ���
                newTile.transform.parent = transform; //6 �θ� ����
                newTile.transform.position = new Vector3(column *  Distance, row * Distance, 0) + positionOffset; //7 ������ �߰��� ���ӿ�����Ʈ �׸����� ��ġ�� �ش��ϴ� �߽��� �׸��带 ����
                grid[column, row] = newTile; //8 ���� �������� ���� ���� ���� ����
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
        GameObject tile = grid[column, row];
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        return renderer.sprite;
    }

}
