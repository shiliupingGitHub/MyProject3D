using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CSVHelper;
[System.Serializable]
public class ActorConfig {
	public 	int	id;
	public 	string	path;
	public 	string	name;
	static Dictionary<int,ActorConfig> mDic = null;
 	 public static Dictionary<int,ActorConfig>  dic {
			get 
 				 {
					if ( mDic == null) {
							mDic = new  Dictionary<int,ActorConfig>();
							 CsvHelper.ReadConfig("ActorConfig",OnLoad);
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
			ActorConfig e = new ActorConfig ();
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
