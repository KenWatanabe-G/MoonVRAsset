using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Terrain))]
public class MoonTerrainImpoter : MonoBehaviour
{
    public int Sampling = 2;
    public UnityEngine.Object moonImgFile;
    public UnityEngine.Object moonLblFile;


    public void SmoothTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        int w = terrain.terrainData.heightmapWidth;
        int h = terrain.terrainData.heightmapHeight;
        var data = terrain.terrainData.GetHeights(0, 0, w, h);
        var smoothData = smooth(data);
        terrain.terrainData.SetHeights(0, 0, smoothData);
    }

#if UNITY_EDITOR

    public void MakeTerrain()
    {
        MoonLabel labelData = readLblFile();
        int[] readData = readImgFile(labelData);

        int resolution = Math.Max(labelData.lines, labelData.lineSample);
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData.heightmapResolution = resolution / Sampling;
        float length = terrain.terrainData.heightmapResolution * Sampling;
        terrain.terrainData.size = new Vector3(length, labelData.height, length);

        List<int> dataList = new List<int>(readData);
        TerrainData tData = terrain.terrainData;
        int h = tData.heightmapHeight;
        int w = tData.heightmapWidth;
        float[,] data = new float[labelData.lineSample / Sampling, labelData.lines / Sampling];

        Debug.Log("min1 = " + dataList.Min() + ", max1 = " + dataList.Max());
        Debug.Log("min2 = " + labelData.minimum + ", max2 = " + labelData.maximum);

        int last = 0;
        for (int x = 0; x < labelData.lines / Sampling; x++)
        {
            for (int y = 0; y < labelData.lineSample / Sampling; y++)
            {
                int hData = readData[x * Sampling * labelData.lineSample + y * Sampling];
                if (hData == labelData.dummy)
                {
                    hData = last;
                }
                else
                {
                    last = hData;
                }

                float height = (hData - labelData.minimum) / (float)(labelData.maximum - labelData.minimum);
                data[y, x] = height;
            }
        }

        terrain.terrainData.SetHeights(0, 0, data);
    }

    private int[] readImgFile(MoonLabel labelData)
    {
        var split = AssetDatabase.GetAssetPath(this.moonImgFile).Split('/');
        var path = Application.dataPath + "/";
        for (int i = 1; i < split.Length - 1; i++)
        {
            path += split[i] + "/";
        }
        path += split[split.Length - 1];

        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] bs = new byte[(int)bin.BaseStream.Length];
        bs = bin.ReadBytes((int)bin.BaseStream.Length);
        fileStream.Close();

        if (BitConverter.IsLittleEndian)
            bs = bs.Reverse().ToArray();

        int[] data = new int[bs.Length / 2];
        for (int i = 0; i < data.Length; i++)
        {
            if (labelData.sampleType == MoonLabel.SampleType.MSB_INTEGER)
            {
                data[i] = BitConverter.ToInt16(bs, i * 2);
            }
            else
            {
                data[i] = BitConverter.ToUInt16(bs, i * 2);
            }
        }

        return data;
    }

    private MoonLabel readLblFile()
    {
        var split = AssetDatabase.GetAssetPath(this.moonLblFile).Split('/');
        var path = Application.dataPath + "/";
        for (int i = 1; i < split.Length - 1; i++)
        {
            path += split[i] + "/";
        }
        path += split[split.Length - 1];

        StreamReader fileStream = new StreamReader(path);
        var data = fileStream.ReadToEnd();

        if (gameObject.GetComponent<MoonLabel>())
        {
            DestroyImmediate(gameObject.GetComponent<MoonLabel>());
        }
        var moonLabel = gameObject.AddComponent<MoonLabel>();
        moonLabel.LoadData(data);

        return moonLabel;
    }

    private float[,] smooth(float[,] data)
    {
        float[,] rtn = new float[data.GetLength(0), data.GetLength(1)];
        Array.Copy(data, rtn, data.Length);

        int d = 3;
        int count = (2 * d + 1) * (2 * d + 1);
        for (int i = d; i < data.GetLength(0) - d; i++)
        {
            for (int j = d; j < data.GetLength(1) - d; j++)
            {
                float sum = 0;
                for (int x = i - d; x <= i + d; x++)
                {
                    for (int y = j - d; y <= j + d; y++)
                    {
                        sum += data[x, y];
                    }
                }
                float average = sum / count;
                rtn [i, j] = average;
            }
        }

        return rtn;
    }

#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MoonTerrainImpoter))]//拡張するクラスを指定
public class MoonTerrainImpoterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Make Terrain"))
        {
            MoonTerrainImpoter impoter = target as MoonTerrainImpoter;
            impoter.MakeTerrain();
        }

        if (GUILayout.Button("Smooth Terrain"))
        {
            MoonTerrainImpoter impoter = target as MoonTerrainImpoter;
            impoter.SmoothTerrain();
        }
    }
}
#endif
