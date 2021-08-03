using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class BezierController : MonoBehaviour
{
    public bool isRunning;
    public bool isShowGizmos;
    public bool isShowLineRenderer;
    public bool isShowFixedPoints;
    [Header("控制手柄")]
    public Transform[] handles;
    Vector3[] handlesOriginalPoint;//控制带你的原始位置
    public int vertexCount;
    List<Vector3> pointList = new List<Vector3>();
    [Header("固定点距离点集合")]
    public List<Vector3> fixedSpacePoints = new List<Vector3>();
    public float fixedSpace;
    [Header("线渲染工具")]
    public LineRenderer lineRenderer;

    private void Start()
    {
        handlesOriginalPoint = new Vector3[handles.Length];
        for (int i = 0; i < handles.Length; i++)
        {
            handlesOriginalPoint[i] = handles[i].position;
        }
        Running();
    }

    private void Update()
    {
        if (CheckHandlesMove() && isRunning)
        {
            Running();
        }
    }

    void Running()
    {
        pointList = Bezier.BezierCurveWithUnlimitPoints(handles, vertexCount);
        //fixedSpacePoints = MathTools.GetEqualySpacePoints(pointList, fixedSpace);

        if (isShowLineRenderer)
        {
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPositions(pointList.ToArray());
        }
    }

    //获取路径点
    public List<Vector3> GetPointList()
    {
        return Bezier.BezierCurveWithUnlimitPoints(handles, vertexCount);
    }

    //获取固定间隔距离位置
    //public List<Vector3> GetFixedSpacePoints()
    //{
    //    CheckHandlesMove();
    //    List<Vector3> list = Bezier.BezierCurveWithUnlimitPoints(handles, vertexCount);
    //    return MathTools.GetEqualySpacePoints(list, fixedSpace);
    //}

    //控制点是否发生更新： 是否改变了控制点的数量或者控制点是否发生了位移
    bool CheckHandlesMove()
    {
        bool hasMove = false;

        if (handlesOriginalPoint == null) handlesOriginalPoint = new Vector3[] { };
        if (handlesOriginalPoint.Length != handles.Length)
        {
            handlesOriginalPoint = new Vector3[handles.Length];
            for (int i = 0; i < handles.Length; i++)
            {
                handlesOriginalPoint[i] = handles[i].position;
            }
            return true;
        }

        for (int i = 0; i < handles.Length; i++)
        {
            if (handles[i].position != handlesOriginalPoint[i])
            {
                hasMove = true;
                break;
            }
        }
        return hasMove;
    }

    private void OnDrawGizmos()
    {
        if (isShowGizmos)
        {
            if (handles.Length > 3)
            {
                #region 无限制顶点数

                Gizmos.color = Color.green;

                for (int i = 0; i < handles.Length - 1; i++)
                {
                    Gizmos.DrawLine(handles[i].position, handles[i + 1].position);
                }

                Gizmos.color = Color.red;

                Vector3[] temp = new Vector3[handles.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    temp[i] = handles[i].position;
                }
                int n = temp.Length - 1;
                for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
                {
                    for (int i = 0; i < n - 2; i++)
                    {
                        Gizmos.DrawLine(Vector3.Lerp(temp[i], temp[i + 1], ratio), Vector3.Lerp(temp[i + 2], temp[i + 3], ratio));
                    }
                }
                #endregion
            }
            else
            {
                #region 顶点数为3

                Gizmos.color = Color.green;

                Gizmos.DrawLine(handles[0].position, handles[1].position);

                Gizmos.color = Color.green;

                Gizmos.DrawLine(handles[1].position, handles[2].position);

                Gizmos.color = Color.red;

                for (float ratio = 0.5f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
                {

                    Gizmos.DrawLine(Vector3.Lerp(handles[0].position, handles[1].position, ratio), Vector3.Lerp(handles[1].position, handles[2].position, ratio));

                }

                #endregion
            }
        }

        if (isShowFixedPoints)
        {
            #region 显示固定距离的点列表
            Gizmos.color = Color.green;
            foreach (Vector3 point in fixedSpacePoints)
            {
                Gizmos.DrawSphere(point, 0.3f);
            }
            #endregion
        }

    }
}