using UnityEngine;
using System;
using System.Collections.Generic;
#if NETFX_CORE
using StreamReader = WinRTLegacy.IO.StreamReader; 
using StreamWriter = WinRTLegacy.IO.StreamWriter;
#else
using StreamReader = System.IO.StreamReader;
using StreamWriter = System.IO.StreamWriter;
#endif
using System.IO;
using System.Text;
namespace CSVHelper
{
    public class CsvHelper
    {
       public static  System.Action<string, System.Action<string, string, System.Action<List<CsvRow>>>, System.Action<List<CsvRow>>> mLoader;
        public static float Tofloat(string text)
        {
            string r = text.Trim();
            if (!string.IsNullOrEmpty(r))
                return System.Convert.ToSingle(r);
            else
                return 0;
        }
        public static  void ReadConfig(string szName, System.Action<List<CsvRow>> callback )
        {
            if (null != mLoader)
                mLoader(szName, OnCsvReadCallBack, callback);
        }
        public static void OnCsvReadCallBack(string szName,string content, System.Action<List<CsvRow>> callback)
        {
           List<CsvRow> mRows =   ParseText(content);
            if (null != callback)
                callback(mRows);
        }
        public static string[] Tostring_array(string text)
        {
            string r = text.Trim();
            if (!string.IsNullOrEmpty(r))
                return ToStringArray(r);
            else
                return null;
        }
        public static string Tostring(string text)
        {
            if (!string.IsNullOrEmpty(text))
                return text;
            else
                return null;
        }

        public static int Toint(string text)
        {
            try
            {
                string r = text.Trim();
                if (!string.IsNullOrEmpty(r))
                    return System.Convert.ToInt32(r);
                else
                    return 0;
            }
            catch(System.Exception e)
            {
                Debug.LogError(text + e.StackTrace);
            }
            return 0;
     
        }
        public static int[] Toint_array(string text)
        {
            string r = text.Trim();
            if (!string.IsNullOrEmpty(r))
                return ToIntArray(r);
            else
                return null;
        }

        public static Vector2Int[] ToVector2Int_array(string text)
        {
            string r = text.Trim();
            if (!string.IsNullOrEmpty(r))
                return ToVector2IntArray(r);
            else
                return null;
        }
        public static float[] Tofloat_array(string text)
        {
            string r = text.Trim();
            if (!string.IsNullOrEmpty(r))
                return ToFloatArray(r);
            else
                return null;
        }
        public static List<CsvRow> ParseCSV_Exp(string pathNameInResource)
        {
            StreamReader mysr = new StreamReader(pathNameInResource);
            string str = mysr.ReadToEnd();
            if (str == string.Empty) return null;

            string contents = str;
            byte[] datas = System.Text.Encoding.UTF8.GetBytes(contents);
            CsvFileReader reader = new CSVHelper.CsvFileReader(new MemoryStream(datas));
            List<CsvRow> result = new List<CsvRow>();
            CsvRow row = new CsvRow();

            while (reader.ReadRow(row))
            {
                result.Add(row);
                row = new CsvRow();
            }
            return result;

        }

        public static List<CsvRow> ParseCSV(TextAsset csvFile)
        {
            if (csvFile)
            {
				byte[] datas = System.Text.Encoding.UTF8.GetBytes(csvFile.text);
                CsvFileReader reader = new CSVHelper.CsvFileReader(new MemoryStream(datas));
                List<CsvRow> result = new List<CsvRow>();
                CsvRow row;
                do
                {
                    row = new CsvRow();
                    result.Add(row);
                }
                while (reader.ReadRow(row));
                return result;
            }
            return null;
        }

        public static Vector2Int[] ToVector2IntArray(string content)
        {
            Vector2Int[] ret = null;
            if(!string.IsNullOrEmpty(content))
            {
                string[] szA = content.Split(';');
                if (null != szA && szA.Length > 0)
                {
                    ret = new Vector2Int[szA.Length];
                    for (int i = 0; i < szA.Length; i++)
                    {
                        try
                        {
                            if (!string.IsNullOrEmpty(szA[i]))
                            {
                                ret[i] = new Vector2Int();

                                string[] szXY = szA[i].Split(":");

                                if (szXY.Length >= 2)
                                {
                                    ret[i].x = Toint(szXY[0]);
                                    ret[i].y = Toint(szXY[1]);
                                }
                                
                                
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(content);
                            Debug.LogError(ex.Message.ToString());
                        }
                    
                    }
                }
            }

            return ret;
        }
        public static int[] ToIntArray(string content)
        {
            
            int[] ret = null;
			if(!string.IsNullOrEmpty(content))
			{
           	 string[] szA = content.Split(';');
           	 if (null != szA && szA.Length > 0)
           	 {
                ret = new int[szA.Length];
                for (int i = 0; i < szA.Length; i++)
                {
                    try
                    {
                         if(!string.IsNullOrEmpty(szA[i]))
                                ret[i] = Convert.ToInt32(szA[i]);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(content);
                        Debug.LogError(ex.Message.ToString());
                    }
                    
                }
           	 }
			}
            return ret;
        }
        public static string[] ToStringArray(string conteng)
        {
            string[] ret = conteng.Split(';');
            return ret;
        }
        public static byte[] ToByteArray(string content)
        {
            byte[] ret = null;
			if(!string.IsNullOrEmpty(content))
			{
            string[] szA = content.Split(';');
            if (null != szA && szA.Length > 0)
            {
                ret = new byte[szA.Length];
                for (int i = 0; i < szA.Length; i++)
                    ret[i] = Convert.ToByte(szA[i]);
            }
			}
            return ret;
        }
        public static double[] ToDoubleArray(string content)
        {
            double[] ret = null;
			if(!string.IsNullOrEmpty(content))
			{
            string[] szA = content.Split(';');
            if (null != szA && szA.Length > 0)
            {
                ret = new double[szA.Length];
                for (int i = 0; i < szA.Length; i++)
                    ret[i] = Convert.ToDouble(szA[i]);
            }
			}
            return ret;
        }
        public static float[] ToFloatArray(string content)
        {
            float[] ret = null;
			if(!string.IsNullOrEmpty(content))
			{
            string[] szA = content.Split(';');
            if (null != szA && szA.Length > 0)
            {
                ret = new float[szA.Length];
                    for (int i = 0; i < szA.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(szA[i]))
                            ret[i] = Convert.ToSingle(szA[i]);
                    }
            }
			}
            return ret;
        }

        public static List<CsvRow> ParseText(string text)
        {
			byte[] datas = System.Text.Encoding.UTF8.GetBytes(text);
            CsvFileReader reader = new CSVHelper.CsvFileReader(new MemoryStream(datas));
            List<CsvRow> result = new List<CsvRow>();
            CsvRow row;
            do
            {
                row = new CsvRow();
                result.Add(row);
            }
            while (reader.ReadRow(row));
            return result;
        }
        public static List<CsvRow> ParseBytes(byte[] datas)
        {
            CsvFileReader reader = new CSVHelper.CsvFileReader(new MemoryStream(datas));
            List<CsvRow> result = new List<CsvRow>();
            CsvRow row;
            do
            {
                row = new CsvRow();
                result.Add(row);
            }
            while (reader.ReadRow(row));
            return result;
        }
    }
    /// <summary>
    /// Class to store one CSV row
    /// </summary>
    public class CsvRow : List<string>
    {
        public string LineText { get; set; }
    }

    /// <summary>
    /// Class to write data to a CSV file
    /// </summary>
    public class CsvFileWriter : StreamWriter
    {
        public CsvFileWriter(Stream stream)
            : base(stream)
        {
        }

        public CsvFileWriter(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Writes a single row to a CSV file.
        /// </summary>
        /// <param name="row">The row to be written</param>
        public void WriteRow(CsvRow row)
        {
            StringBuilder builder = new StringBuilder();
            bool firstColumn = true;
            foreach (string value in row)
            {
                // Add separator if this isn't the first value
                if (!firstColumn)
                    builder.Append(',');
                // Implement special handling for values that contain comma or quote
                // Enclose in quotes and double up any double quotes
                if (value.IndexOfAny(new char[] { '"', ',' }) != -1)
                    builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
                else
                    builder.Append(value);
                firstColumn = false;
            }
            row.LineText = builder.ToString();
            WriteLine(row.LineText);
        }
    }

    /// <summary>
    /// Class to read data from a CSV file
    /// </summary>
    public class CsvFileReader : StreamReader
    {
        public CsvFileReader(Stream stream)
            : base(stream)
        {
        }

        public CsvFileReader(string filename)
            : base(filename)
        {
        }

        /// <summary>
        /// Reads a row of data from a CSV file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public bool ReadRow(CsvRow row)
        {
            row.LineText = ReadLine();
            if (String.IsNullOrEmpty(row.LineText))
                return false;

            int pos = 0;
            int rows = 0;

            while (pos < row.LineText.Length)
            {
                string value;

                // Special handling for quoted field
                if (row.LineText[pos] == '"')
                {
                    // Skip initial quote
                    pos++;

                    // Parse quoted value
                    int start = pos;
                    while (pos < row.LineText.Length)
                    {
                        // Test for quote character
                        if (row.LineText[pos] == '"')
                        {
                            // Found one
                            pos++;

                            // If two quotes together, keep one
                            // Otherwise, indicates end of value
                            if (pos >= row.LineText.Length || row.LineText[pos] != '"')
                            {
                                pos--;
                                break;
                            }
                        }
                        pos++;
                    }
                    value = row.LineText.Substring(start, pos - start);
                    value = value.Replace("\"\"", "\"");
                }
                else
                {
                    // Parse unquoted value
                    int start = pos;
                    while (pos < row.LineText.Length && row.LineText[pos] != ',')
                        pos++;
                    value = row.LineText.Substring(start, pos - start);
                }

                // Add field to list
                if (rows < row.Count)
                    row[rows] = value;
                else
                    row.Add(value);
                rows++;

                // Eat up to and including next comma
                while (pos < row.LineText.Length && row.LineText[pos] != ',')
                    pos++;
                if (pos < row.LineText.Length)
                    pos++;
            }
            // Delete any unused items
            while (row.Count > rows)
                row.RemoveAt(rows);

            // Return true if any columns read
            return (row.Count > 0);
        }
    }
}