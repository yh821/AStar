using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawGrid : MonoBehaviour
{
    public const int row = 51;
    public const int colomn = 51;

    public int w = 10;
    public int r = 10;

    public Transform plane;
    public GameObject pixelPrefab;
    public Color[] typeColor;

    private int[,] MapData = new int[row, colomn];
    private Image[,] pixels;


    void Start()
    {
        Init();
    }

    void Init()
    {
        pixels = new Image[row, colomn];
        var grid = GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = colomn;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < colomn; j++)
            {
                GameObject go = Instantiate(pixelPrefab, new Vector3(i, 0, j), Quaternion.identity);
                var pixel = go.transform.GetComponent<Image>();
                pixel.color = typeColor[0];
                pixel.transform.parent = plane;
                pixels[i, j] = pixel;
            }
        }
    }

    private void PutPixel(int x, int y, int c)
    {
        var pixel = pixels[x, y];
        pixel.color = typeColor[c];
    }

    public void Clean()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < colomn; j++)
            {
                var pixel = pixels[i, j];
                pixel.color = typeColor[0];
            }
        }
    }

    public void DrawCircle1()
    {
        Bresenham(r, r, r);
    }

    public void DrawCircle2()
    {
        NormalDraw(r, r, r, w);
    }

    private void NormalDraw(int xc, int yc, int r, int w)
    {
        int angle = 0;
        for (int i = 0, len = 360 / w; i < len; i++)
        {
            var x = Mathf.RoundToInt(Mathf.Cos(angle) * r) + xc;
            var y = Mathf.RoundToInt(Mathf.Sin(angle) * r) + yc;
            PutPixel(x, y, 1);
            angle += w;
        }
    }

    private void Bresenham(int xc, int yc, int r, bool fill = false)
    {
        // (xc, yc) 为圆心，r 为半径
        // fill 为是否填充

        int x = 0, y = r, yi, d;
        d = 3 - 2 * r;

        if (fill)
        {
            //画实心圆
            while (x <= y)
            {
                for (yi = x; yi <= y; yi++)
                {
                    draw_circle_8(xc, yc, x, yi, 1);
                }

                if (d < 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    d = d + 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
        }
        else
        {
            //画空心圆
            while (x <= y)
            {
                draw_circle_8(xc, yc, x, y, 1);

                if (d < 0)
                {
                    d = d + 4 * x + 6;
                }
                else
                {
                    d = d + 4 * (x - y) + 10;
                    y--;
                }
                x++;
            }
        }
    }

    public void draw_circle_8(int xc, int yc, int x, int y, int c)
    {
        PutPixel(xc + x, yc + y, c);
        PutPixel(xc - x, yc + y, c);
        PutPixel(xc + x, yc - y, c);
        PutPixel(xc - x, yc - y, c);
        PutPixel(xc + y, yc + x, c);
        PutPixel(xc - y, yc + x, c);
        PutPixel(xc + y, yc - x, c);
        PutPixel(xc - y, yc - x, c);
    }

}
