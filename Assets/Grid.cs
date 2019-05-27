//本案例采用基于单元的导航图寻路方式以及曼哈顿距离估价法进行实现。
//首先需要创建一个格子类Grid：
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 格子类型
/// </summary>
public enum GridType
{
    //地形类型
    Normal,
    //起点类型
    Start,
    //终点类型
    End
}
public enum WeightType
{
    Grass,//草地
    Brook,//小溪
    Massif,//山丘
    River,//江
    Mountain,//高山
    Sea,//海
    Unable,
}
/// <summary>
/// 格子类（实现IComparable方便排序）
/// </summary>
public class Grid : IComparable
{
    //格子坐标x-y
    public int x;
    public int y;
    //格子A*三属性f-g-h
    public int f;
    public int g;
    public int h;
    //weight权值
    public WeightType weight = WeightType.Grass;
    //格子类型
    public GridType type;
    //格子的归属（父格子）
    public Grid parent;
    //构造赋值
    public Grid(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    /// <summary>
    /// 实现排序接口方法
    /// </summary>
    /// <returns>The to.</returns>
    /// <param name="obj">Object.</param>
    public int CompareTo(object obj)
    {
        Grid grid = (Grid)obj;
        if (this.f < grid.f)
        {
            //升序
            return -1;
        }
        if (this.f > grid.f)
        {
            //降序
            return 1;
        }
        return 0;
    }

    public void Clear()
    {
        f = g = h = 0;
        type = GridType.Normal;
        parent = null;
    }
}