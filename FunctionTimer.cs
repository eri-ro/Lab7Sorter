using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ManagedSort;
using UnityEngine;

public class FunctionTimer : MonoBehaviour
{
    [DllImport("TestDLL", EntryPoint = "TestSort", CallingConvention = CallingConvention.Cdecl)]
    static extern void TestSort(int[] a, int length);

    void Start()
    {
        MeasureFunctionExecutionTime();
    }

    void MeasureFunctionExecutionTime()
    {
        int[] source = LoadIntegers();
        if (source == null)
            return;

        TimeMethod("Method 1 native C++ plugin", source, SortNative);
        TimeMethod("Method 2 managed C# plugin", source, SortManaged);
        TimeMethod("Method 3 MonoBehaviour C#", source, MonoBehaviourSort.Sort);
    }

    static void SortNative(int[] values)
    {
        TestSort(values, values.Length);
    }

    static void SortManaged(int[] values)
    {
        IntegerSorter.Sort(values);
    }

    void TimeMethod(string label, int[] source, Action<int[]> sort)
    {
        int[] values = (int[])source.Clone();

        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        sort(values);
        stopwatch.Stop();

        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log(
            label + " took " + elapsedMilliseconds + " ms to execute.");
    }

    int[] LoadIntegers()
    {
        TextAsset asset = Resources.Load<TextAsset>("integers");
        if (asset == null)
        {
            UnityEngine.Debug.LogError("Missing Resources/integers.txt.");
            return null;
        }

        string[] lines = asset.text.Split(new[] { '\r', '\n', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        int[] values = new int[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            int value;
            if (!int.TryParse(lines[i], out value))
            {
                UnityEngine.Debug.LogError("integers.txt contains a value that is not an integer.");
                return null;
            }

            values[i] = value;
        }

        return values;
    }
}
