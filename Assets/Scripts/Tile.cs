using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    private static Tile selected; //���� Ÿ�� ���� ��������
    private SpriteRenderer Renderer; //�;� ������ ����(������������)

    public Vector2Int position;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>(); //������ ���� ����.
    }

    public void Select()
    {
        Renderer.color = Color.gray; // ���� �� Ȯ��
    }

    public void Unselect()
    {
        Renderer.color = Color.white; //���� ���
    }

    private void OnMouseDown() //�� Ŭ���� �ν��Ͽ� on mouse down ȣ��
    {
        if(selected != null)
        {
            if (selected == this)
                return;
            selected.Unselect();
            if (Vector2Int.Distance(selected.position, position) == 1)
            {
                GridManager.Instance.SwapTiles(position, selected.position);
                selected = null;
            }
            else
            {
                selected = this;
                Select();
            }
        }
        else
        {
            selected = this;
            Select();
        }
    }

}
