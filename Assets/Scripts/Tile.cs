using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    private static Tile selected; //현재 타일 저장 정적변수
    private SpriteRenderer Renderer; //터알 랜더러 저장(생상저장위해)

    public Vector2Int position;

    // Start is called before the first frame update
    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>(); //변수에 참조 저장.
    }

    public void Select()
    {
        Renderer.color = Color.gray; // 현재 셀 확인
    }

    public void Unselect()
    {
        Renderer.color = Color.white; //선택 취소
    }

    private void OnMouseDown() //셀 클릭시 인식하여 on mouse down 호출
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
