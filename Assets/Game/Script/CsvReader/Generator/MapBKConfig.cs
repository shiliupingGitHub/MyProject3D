using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSVHelper;
[System.Serializable]
public class MapBKConfig {
	public 	int	id;
	public 	string	path;
	public 	string	name;
	static Dictionary<int,MapBKConfig> mDic = null;
 	 public static Dictionary<int,MapBKConfig>  dic {
			get 
 				 {
					if ( mDic == null) {
							mDic = new  Dictionary<int,MapBKConfig>();
							 CsvHelper.ReadConfig("MapBKConfig",OnLoad);
							}
					 return mDic;
				}
	 }
	public static void OnLoad(List<CsvRow> rows)
	{
		for(int i = 3; i < rows.Count; i++)
		{
			CsvRow r = rows[i];
			 if (string.IsNullOrEmpty(r.LineText)) continue;
			MapBKConfig e = new MapBKConfig ();
		 if(r.Count >0)
			e.id= CsvHelper.Toint(r[0]);

		 if(r.Count >1)
			e.path= CsvHelper.Tostring(r[1]);

		 if(r.Count >2)
			e.name= CsvHelper.Tostring(r[2]);

			dic[e.id] = e;
		}

	}
}
