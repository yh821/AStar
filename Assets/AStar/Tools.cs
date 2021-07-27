using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public partial class AStar
{
	public static int[,] MapData = new int[24, 40];

	public string ReadMapFile() {
		string rootPath = Path.Combine (Application.dataPath, "MapData");
		var files = Directory.GetFiles (rootPath, "*.txt", SearchOption.AllDirectories);
		foreach (var file in files) {
			return file;
		}
		return "";
	}

    public void ImportMap () {
		var fullpath = ReadMapFile ();
        if (!File.Exists(fullpath))
            return;
		List<List<int>> map = new List<List<int>>();
        using(StreamReader sr = new StreamReader(fullpath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Replace("\n", "");
                if (line.StartsWith("#", StringComparison.Ordinal))
                    continue;
                var mapline = line.Split(',');
                List<int> list = new List<int>();
                for(int i = 0; i < mapline.Length; i++)
                {
                    list.Add(int.Parse(mapline[i]));
                }
                map.Add(list);
            }
        }
        int row = MapData.GetLength(0);
        int colomn = MapData.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for(int j = 0; j < colomn; j++)
            {
                try
                {
                    MapData[i, j] = map[i][j];
                }
                catch (Exception ex)
                {
                    MapData[i, j] = 0;
                    Debug.LogWarningFormat("不存在[{0},{1}]数据, {2}", i, j, ex.ToString());
                }
            }
        }
        Debug.LogFormat("导入完成,{0}", fullpath);
        ClickClean();
    }

    public void ExportMap () {
		var file = string.Format ("MapData/map_{0}.txt", DateTime.Now.ToString ("MMddHHMMss"));
		var fullpath = Path.Combine(dataPath, file);
        using(StreamWriter sw = new StreamWriter(fullpath,false,Encoding.UTF8))
        {
            int row = MapData.GetLength(0);
            int colomn = MapData.GetLength(1);
            for (int i=0;i< row;i++)
            {
                var line = new StringBuilder();
                for (int j = 0; j < colomn; j++)
                {
                    if (j == 0)
                        line.Append(MapData[i, j]);
                    else
                        line.Append(',').Append(MapData[i, j]);
                }
                sw.WriteLine(line.ToString());
            }
        }
		UnityEditor.AssetDatabase.Refresh ();
        Debug.LogFormat("导出成功,{0}",fullpath);
    }

}
