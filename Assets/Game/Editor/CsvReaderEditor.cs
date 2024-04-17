using UnityEngine;
using System.Collections;
using UnityEditor;
using CSVHelper;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class CsvReaderEditor : Editor {

    [MenuItem("CsvReader/CreateReadScript")]
    static void CreateScript()
    {
        string savePath = "Game/Script/Config";
        foreach (Object o in Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets | SelectionMode.Assets))
        {


            string filePath = AssetDatabase.GetAssetPath(o);

            string csPath = Application.dataPath + "/"+savePath +"/" + Path.GetFileName(filePath).Replace(".csv", ".cs");
            StreamWriter sc = new StreamWriter(csPath, false);
            var ta = o as TextAsset;
            var content = System.Text.Encoding.GetEncoding("GBK").GetString(ta.bytes);
            List<CsvRow> rows = CsvHelper.ParseText(content);
            if (rows.Count < 3)
            {

                return;
            }
            sc.Write("using UnityEngine;\n");
            sc.Write("using System.Collections;\n");
            sc.Write("using System.Collections.Generic;\n");
            sc.Write("using CSVHelper;\n");
            sc.Write("[System.Serializable]\n");
            sc.Write("public class " + o.name + " {\n");
            CsvRow types = rows[0];
            CsvRow vars = rows[1];
            CsvRow des = rows[2];
            //            CsvRow des = rows[2];
            for (int i = 0; i < types.Count; i++)
            {
                if (string.IsNullOrEmpty(types[i]))
                    continue;
                sc.Write("\tpublic \t" + types[i] + "\t" + vars[i] + ";" + "//"+ des[i] + "\n");
            }
            sc.Write("\tstatic Dictionary<" + types[0] + "," + o.name + "> mDic = null;\n ");


            sc.Write("\t public static Dictionary<" + types[0] + "," + o.name + ">  dic {\n");
            sc.Write("\t\t\tget \n \t\t\t\t {\n");
            sc.Write("\t\t\t\t\tif ( mDic == null) {\n");
            sc.Write("\t\t\t\t\t\t\tmDic = new " + " Dictionary<" + types[0] + "," + o.name + ">();\n");
            sc.Write("\t\t\t\t\t\t\t CsvHelper.ReadConfig(\"" + o.name + "\",OnLoad);\n");
            sc.Write("\t\t\t\t\t\t\t}\n");
            sc.Write("\t\t\t\t\t return mDic;\n");
            sc.Write("\t\t\t\t}\n");

            sc.Write("\t }\n");

            sc.Write("\tpublic static void OnLoad(List<CsvRow> rows)\n\t{");
            sc.Write("\n\t\tfor(int i = 3; i < rows.Count; i++)\n\t\t{\n");
            sc.Write("\t\t\tCsvRow r = rows[i];\n");
            sc.Write("\t\t\t if (string.IsNullOrEmpty(r.LineText)) continue;\n");
            sc.Write("\t\t\t" + o.name + " e = new " + o.name + " ();\n");
            for (int k = 0; k < types.Count; k++)
            {
                if (string.IsNullOrEmpty(types[k]))
                    continue;
                sc.Write("\t\t if(r.Count >" + k + ")\n");
                sc.Write("\t\t\te." + vars[k] + "= CsvHelper.To");
                if (types[k].Contains("[]"))
                {
                    sc.Write(types[k].Remove(types[k].Length - 2) + "_array(r[" + k + "]);\n");
                }
                else
                {
                    sc.Write(types[k] + "(r[" + k + "]);\n");
                }

                sc.Write("\n");
            }
            sc.Write("\t\t\tdic[e." + vars[0] + "] = e;\n");
            sc.Write("\t\t}");
            sc.Write("\n");
            sc.Write("\n\t}\n");
            sc.Write("}\n");
            sc.Close();
        }
        AssetDatabase.Refresh();
    }
}
