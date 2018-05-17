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

#if UNITY_EDITOR
    int[] readImgFile(MoonLabel labelData)
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

    MoonLabel readLblFile()
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

    public void makeTerrain()
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
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MoonTerrainImpoter))]//拡張するクラスを指定
public class ExampleScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Make Terrain"))
        {
            MoonTerrainImpoter impoter = target as MoonTerrainImpoter;
            impoter.makeTerrain();
        }
    }
}
#endif
