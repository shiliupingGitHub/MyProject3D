using System;
using CSVHelper;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine;
public class CSVReader 
{
    static CSVReader mInstance;
    public static CSVReader Instance
    {
        get
        {
            if (null == mInstance)
                mInstance = new CSVReader();
            return mInstance;
        }
    }
    delegate System.Object ReadDeleage(string text);
    Dictionary<System.Type, ReadDeleage> mReads = new Dictionary<System.Type, ReadDeleage>();
    System.Object Parse(string text, Type type)
    {
        if (mReads.Count == 0)
            Register();
        ReadDeleage read = mReads[type];
        return read(text);
    }
    public void Register()
    {
        mReads[typeof(int)] = IntReader;
        mReads[typeof(int[])] = IntArrayReader;
        mReads[typeof(float)] = FloatReader;
        mReads[typeof(float[])] = FloatArrayReader;
        mReads[typeof(string)] = StringReader;
        mReads[typeof(string[])] = StringArrayReader;
    }
    System.Object IntReader(string text)
    {
        try
        {
            if (!string.IsNullOrEmpty(text.Trim()))
                return System.Convert.ToInt32(text);
            else
                return 0;

        }
        catch (Exception)
        {
            Debug.LogError(text);
        }

        return 0;
    }
    System.Object IntArrayReader(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
            return CSVHelper.CsvHelper.ToIntArray(text);
        else
            return new int[]{0};
    }
    System.Object FloatReader(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
            return System.Convert.ToSingle(text);
        else
            return 0.0f;
    }
    System.Object FloatArrayReader(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
            return CSVHelper.CsvHelper.ToFloatArray(text);
        else
            return new float[]{0.0f};
    }
    System.Object StringReader(string text)
    {
        return text;
    }
    System.Object StringArrayReader(string text)
    {
        if (!string.IsNullOrEmpty(text))
            return CSVHelper.CsvHelper.ToStringArray(text);
        else
            return new string[]{""};
    }
    public void Read(System.Type t, List<CsvRow> rows,IDictionary dic)
    {
        if (rows.Count <= 3)
            return;
        List<string> fileNames = rows[1];
        Dictionary<int, FieldInfo> fs = new Dictionary<int, FieldInfo>();
        int k = 0;
        foreach(var szName in fileNames)
        {

#if NETFX_CORE
                FieldInfo      f= WinRTLegacy.TypeExtensions.GetField(t,fileNames[j],BindingFlags.Instance | BindingFlags.Public);
#else
            FieldInfo f = t.GetField(szName, BindingFlags.Instance | BindingFlags.Public);
#endif
            fs[k] = f;
            k++;
        }

        for (int i = 3; i < rows.Count; i++)
        {
            CsvRow r = rows[i];
            if (r.Count <= 0)
                continue;
            System.Object elem = System.Activator.CreateInstance(t);
            for (int j = 0; j < r.Count; j++)
            {

               
                    string text = r[j];
                    if (string.IsNullOrEmpty(text))
                        continue;
                 FieldInfo f = null;
                if (!fs.TryGetValue(j, out f))
                    continue;
                if (null != f)
                {
                    System.Object o = Parse(text, f.FieldType);
                    f.SetValue(elem, o);
                    if (j == 0)
                        dic[o] = elem;
                }

                   
                    
                       
            }
            
        }
    }
    int GetIndex(string Name, CsvRow row)
    {
        int i = -1;
        foreach (var r in row)
        {
            i++;
            if (r.Trim() == Name.Trim())
                return i;
            
        }
         return i;
    }
}

