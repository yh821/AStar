﻿//最后是参考预设体方块的简单实现：
using UnityEngine;

public class Reference : MonoBehaviour
{
    //颜色材质区分
    public MeshRenderer mesh;
    //当前格子坐标
    public int x;
    public int y;

    private void Awake()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// 鼠标点击显示当前格子基础信息
    /// </summary>
    void OnMouseDown()
    {
        var grid = AStar.instance.grids[x, y];
        AStar.instance.tips.text = string.Format("XY({0},{1})\nFGH({2},{3},{4})",x,y,grid.f,grid.g,grid.h);

        var curType = AStar.instance.curType;
        if (grid.type == curType)
        {
            if (curType == GridType.Normal)
                mesh.material.color = AStar.instance.curColor;
            else
                mesh.material.color = AStar.instance.typeColor[0];
            grid.type = GridType.Normal;
        }
        else
        {
            if(curType == GridType.Start)
            {
                if (AStar.instance.startGrid != null)
                {
                    AStar.instance.startGrid.type = GridType.Normal;
                    AStar.instance.startRect.mesh.material.color = AStar.instance.typeColor[0];
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
                    AStar.instance.endGrid.type = GridType.Normal;
                    AStar.instance.endRect.mesh.material.color = AStar.instance.typeColor[0];
                }
                AStar.instance.endGrid = grid;
                AStar.instance.endRect = this;
                AStar.instance.targetX = x;
                AStar.instance.targetY = y;
            }

            mesh.material.color = AStar.instance.curColor;
            grid.type = curType;
        }
        grid.weight = AStar.instance.curWeight;
    }
}