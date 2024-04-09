using System.Collections.Generic;
using CSVHelper;

namespace Game.Script.UI.Extern
{
    public class LanguageData {
        public 	string	id;
        public 	string	data;
        public Dictionary<string, LanguageData> dic { get; set; } = new();

        public void Load(string content)
        {
            List<CsvRow> rows =   CsvHelper.ParseText(content);
            OnLoad(rows);
        }
        public  void OnLoad(List<CsvRow> rows)
        {
            for(int i = 3; i < rows.Count; i++)
            {
                CsvRow r = rows[i];
                if (string.IsNullOrEmpty(r.LineText)) continue;
                LanguageData e = new LanguageData ();
                if(r.Count >0)
                    e.id= CsvHelper.Tostring(r[0]);

                if(r.Count >1)
                    e.data= CsvHelper.Tostring(r[1]);
                
                dic[e.id] = e;
            }

        }
    }
}