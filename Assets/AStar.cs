//然后主逻辑AStar类：
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public partial class AStar : MonoBehaviour
{
    /// <summary>
    /// 单例脚本
    /// </summary>
    public static AStar instance;
    public const int MinD = 10;
    //参考物体预设体
    public GameObject pixelPrefab;
    public Text tips;
    //格子数组
    public Grid[,] grids;
    //格子数组对应的参考物（方块）对象
    public Pixel[,] objs;
    //开启列表
    public ArrayList openList;
    //关闭列表
    public ArrayList closeList;
    //起始点坐标
    [HideInInspector]
    public int startX;
    [HideInInspector]
    public int startY;
    //目标点坐标
    [HideInInspector]
    public int targetX;
    [HideInInspector]
    public int targetY;

    public Color[] typeColor;
    public GameObject obstaclePrefab;
    public bool showFootPrint = false;

    [HideInInspector]
    public GridType curType = GridType.Map;
    [HideInInspector]
    public WeightType curWeight = WeightType.Level0;
    public Grid startGrid;
    public Grid endGrid;
    [HideInInspector]
    public Pixel startRect;
    [HideInInspector]
    public Pixel endRect;
    [HideInInspector]
    public Color curColor;

    //格子行列数
    private int row;
    private int colomn;
    //结果栈
    private Stack<string> parentList;
    //基础物体
    private Transform plane;
    private Transform obstacles;
    //流颜色参数
    private float alpha = 0;
    private float incrementPer = 0;
    private string dataPath = "";
    void Awake()
    {
        instance = this;
        plane = transform.Find("Map");
        obstacles = transform.Find("TopLeft/Obstacles");
        parentList = new Stack<string>();
        openList = new ArrayList();
        closeList = new ArrayList();
        dataPath = Application.dataPath;
    }
    /// <summary>
    /// 初始化操作
    /// </summary>
    void Init()
    {
		//导入地图
		ImportMap ();

        //计算行列数
        int x = MapData.GetLength(0);
        int y = MapData.GetLength(1);
        row = x;
        colomn = y;
        grids = new Grid[x, y];
        objs = new Pixel[x, y];

        for (int i = 0; i < typeColor.Length; i++)
        {
            GameObject item = Instantiate(obstaclePrefab, obstacles, false);
            item.name = ((WeightType)i).ToString();
            var bt = item.GetComponent<Pixel>();
            bt.image.color = typeColor[i];
            bt.button.onClick.AddListener(bt.ClickColor);
        }

        //起始坐标
        Vector3 startPos = new Vector3(-4.75f, 0, 0.5f);
        //生成参考物体（Cube）
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                grids[i, j] = new Grid(i, j);
                GameObject go = Instantiate(pixelPrefab, new Vector3(i, 0, j) + startPos, Quaternion.identity);
                var pixel = go.transform.GetComponent<Pixel>();
                pixel.x = i;
                pixel.y = j;
                pixel.image.color = typeColor[MapData[i,j]];
                pixel.transform.parent = plane;
                pixel.button.onClick.AddListener(pixel.OnMouseDown);
                objs[i, j] = pixel;
            }
        }
        curColor = typeColor[0];

		//清理地图
		ClickClean();
    }
    /// <summary>
    /// A*计算
    /// </summary>
    IEnumerator Count()
    {
        //等待前面操作完成
        yield return new WaitForSeconds(0.1f);
        //添加起始点
        openList.Add(grids[startX, startY]);
        //声明当前格子变量，并赋初值
        Grid currentGrid = openList[0] as Grid;
        //循环遍历路径最小F的点
        while (openList.Count > 0 && currentGrid.type != GridType.End)
        {
            //获取此时最小F点
            currentGrid = openList[0] as Grid;
            //Debug.Log(string.Format("top({0},{1}),{2}={3}+{4}", currentGrid.x, currentGrid.y,currentGrid.f, currentGrid.g, currentGrid.h));
            //如果当前点就是目标
            if (currentGrid.type == GridType.End)
            {
                Debug.Log("Find");
                //生成结果
                GenerateResult(currentGrid);
            }
            //上下左右，左上左下，右上右下，遍历八个方向
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        //计算坐标
                        int x = currentGrid.x + i;
                        int y = currentGrid.y + j;
                        //如果未超出所有格子范围，不是障碍物，不是重复点
                        if (x >= 0 && y >= 0 && x < row && y < colomn
                            && grids[x, y].weight != WeightType.Unable
                            && !closeList.Contains(grids[x, y]))
                        {
                            var grid = grids[x, y];
                            //计算G值=相对坐标绝对值的和开平方
                            int g = currentGrid.g + (int)(Mathf.Sqrt(Mathf.Abs(i) + Mathf.Abs(j)) * MinD) + (int)grid.weight * (MinD>>1);
                            //与原G值对照
                            if (grid.g == 0 || grid.g > g)
                            {
								//Debug.LogFormat ("更新G值:{0}=>>{1}", grid.g, g);
                                //更新G值
                                grid.g = g;
                                //更新父格子
                                grid.parent = currentGrid;
                            }
                            //计算H值, 曼哈顿方式计算H值
                            grid.h = Manhattan(x, y);
                            //计算F值
                            grid.f = grid.g + grid.h;
							//Debug.LogFormat ("f({0})=g({1})+h({2})", grid.f, grid.g, grid.h);
                            //如果未添加到开启列表
                            if (!openList.Contains(grid))
                            {
                                //添加
                                openList.Add(grid);
                                if (showFootPrint && grid.type == GridType.Map)
                                    objs[x, y].image.color = Color.cyan;
                            }
                            //重新排序
                            openList.Sort();
                        }
                    }
                }
            }
            //完成遍历添加该点到关闭列表
            closeList.Add(currentGrid);
            if (showFootPrint && currentGrid.type == GridType.Map)
                objs[currentGrid.x, currentGrid.y].image.color = Color.grey;
            //从开启列表中移除
            openList.Remove(currentGrid);
            //如果开启列表空，未能找到路径
            if (openList.Count == 0)
            {
                Debug.Log("Can not Find");
            }
        }
    }
    /// <summary>
    /// 生成结果
    /// </summary>
    /// <param name="currentGrid">Current grid.</param>
    void GenerateResult(Grid currentGrid)
    {
        //如果当前格子有父格子
        if (currentGrid.parent != null)
        {
            //添加到父对象栈（即结果栈）
            parentList.Push(currentGrid.x + "|" + currentGrid.y);
			Debug.LogFormat ("g:{0},h:{1}", currentGrid.g, currentGrid.h);
            //递归获取
            GenerateResult(currentGrid.parent);
        }
    }
    /// <summary>
    /// 显示结果
    /// </summary>
    /// <returns>The result.</returns>
    IEnumerator ShowResult()
    {
        //等待前面计算完成
        yield return new WaitForSeconds(0.3f);
        //计算每帧颜色值增量
        incrementPer = 1 / (float)parentList.Count;
        //展示结果
        while (parentList.Count != 0)
        {
            //出栈
            string str = parentList.Pop();
            //等0.1秒
            yield return new WaitForSeconds(0.1f);
            //拆分获取坐标
            string[] xy = str.Split(new char[] { '|' });
            int x = int.Parse(xy[0]);
            int y = int.Parse(xy[1]);
            //当前颜色值
            alpha += incrementPer;
            //以颜色方式绘制路径
            objs[x, y].image.color = new Color(1, 1 - alpha, 0, 1);
        }
    }
    /// <summary>
    /// 曼哈顿方式计算H值
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    int Manhattan(int x, int y)
    {
        //H(n)=D*(abs(current.x–target.x)+abs(current.y–target.y))
        return (int)(Mathf.Abs(targetX - x) + Mathf.Abs(targetY - y)) * MinD;
    }
    void Start()
    {
        Init();
    }
    public void ClickStart()
    {
        StartCoroutine(Count());
        StartCoroutine(ShowResult());
    }
    public void ClickStop()
    {
        StopAllCoroutines();
    }
    public void ClickClean()
    {
        startX = startY = targetX = targetY = 0;
        alpha = 0;
        openList.Clear();
        closeList.Clear();
        parentList.Clear();

        startGrid = null;
        endGrid = null;
        startRect = null;
        endRect = null;
        curType = GridType.Map;
        curColor = typeColor[(int)WeightType.Level0];
        curWeight = WeightType.Level0;

        for (int x = 0; x < row; x++)
        {
            for (int y = 0; y < colomn; y++)
            {
                grids[x, y].Clear();
                grids[x, y].weight = (WeightType)MapData[x, y];
                objs[x, y].image.color = typeColor[MapData[x, y]];
            }
        }
		Debug.Log ("Clean");
    }
    public void ClickQiDian()
    {
        curType = GridType.Start;
        var go = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        curColor = go.GetComponent<Image>().color;
        Debug.Log(curType.ToString());
    }
    public void ClickZhongDian()
    {
        curType = GridType.End;
        var go = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        curColor = go.GetComponent<Image>().color;
        Debug.Log(curType.ToString());
    }
}