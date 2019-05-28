//最后是参考预设体方块的简单实现：
using UnityEngine;
using UnityEngine.UI;

public class Pixel : MonoBehaviour
{
    //颜色材质区分
    [HideInInspector]
    public Image image;
    [HideInInspector]
    public Button button;
    //当前格子坐标
    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }

    /// <summary>
    /// 鼠标点击显示当前格子基础信息
    /// </summary>
    public void OnMouseDown()
    {
        var grid = AStar.instance.grids[x, y];
        AStar.instance.tips.text = string.Format("XY({0},{1})\n({2},{3},{4})",x,y,grid.f,grid.g,grid.h);

        var curType = AStar.instance.curType;
        if(curType == GridType.Start)
        {
            if (AStar.instance.startGrid != null)
            {
                AStar.instance.startGrid.type = GridType.Map;
                AStar.instance.startRect.image.color = AStar.instance.typeColor[0];
            }
            AStar.instance.startGrid = grid;
            AStar.instance.startRect = this;
            AStar.instance.startX = x;
            AStar.instance.startY = y;
        }
        else if (curType == GridType.End)
        {
            if (AStar.instance.endGrid != null)
            {
                AStar.instance.endGrid.type = GridType.Map;
                AStar.instance.endRect.image.color = AStar.instance.typeColor[0];
            }
            AStar.instance.endGrid = grid;
            AStar.instance.endRect = this;
            AStar.instance.targetX = x;
            AStar.instance.targetY = y;
        }

        image.color = AStar.instance.curColor;
        grid.type = curType;
        grid.weight = AStar.instance.curWeight;
        AStar.MapData[x,y] = (int)AStar.instance.curWeight;
    }

    public void ClickColor()
    {
        AStar.instance.curType = GridType.Map;
        AStar.instance.curWeight = (WeightType)System.Enum.Parse(typeof(WeightType), gameObject.name);
        AStar.instance.curColor = image.color;
        Debug.Log(AStar.instance.curWeight.ToString());
    }
}
