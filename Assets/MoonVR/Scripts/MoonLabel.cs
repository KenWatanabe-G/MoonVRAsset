using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

public class MoonLabel : MonoBehaviour {

    public enum SampleType { UNKNOWN, MSB_INTEGER, MSB_UNSIGNED_INTEGER };
    public SampleType sampleType;
    public int lines;
    public int lineSample;
    public float scalingFactor;
    public int dummy;
    public int maximum;
    public int minimum;
    public float height { get { return scalingFactor * (maximum - minimum);}}

    public void LoadData(string data)
    {
        // get lines
        Match m = Regex.Match(data, "LINES = (?<a>[0-9]+)");
        lines = int.Parse(m.Groups["a"].Value);

        // get line sample
        m = Regex.Match(data, "LINE_SAMPLES = (?<a>[0-9]+)");
        lineSample = int.Parse(m.Groups["a"].Value);

        // get line sample
        m = Regex.Match(data, "SCALING_FACTOR = (?<a>[.0-9]+)");
        scalingFactor = float.Parse(m.Groups["a"].Value);

        // get dummy
        m = Regex.Match(data, "DUMMY = (?<a>-?[0-9]+)");
        dummy = int.Parse(m.Groups["a"].Value);

        // get maximum
        m = Regex.Match(data, " MAXIMUM = (?<a>-?[0-9]+)");
        maximum = int.Parse(m.Groups["a"].Value);

        // get minimum
        m = Regex.Match(data, " MINIMUM = (?<a>-?[0-9]+)");
        minimum = int.Parse(m.Groups["a"].Value);

        // get sample type
        m = Regex.Match(data, "SAMPLE_TYPE = (?<a>[A-Z_]+)");
        string sampleTypeStr = m.Groups["a"].Value;
        foreach(SampleType type in Enum.GetValues(typeof(SampleType))) {
            if (sampleTypeStr == type.ToString())
            {
                sampleType = type;
            }
        }
    }
}
