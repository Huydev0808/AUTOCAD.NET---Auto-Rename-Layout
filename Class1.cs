using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace Relayout1
{
    public class Class1
    {
        [CommandMethod("LayRenum")]
        public void CmdLayRenum()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var edt = doc.Editor;
            var db = doc.Database;
            var layoutMgr = LayoutManager.Current;

            try
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    var layoutDict = (DBDictionary)trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
                    var layouts = layoutDict
                        .Cast<System.Collections.DictionaryEntry>()
                        .Select(entry => (Layout)trans.GetObject((ObjectId)entry.Value, OpenMode.ForWrite))
                        .OrderBy(layout => layout.TabOrder)
                        .ToArray();
                    for (int i = 1; i < layouts.Length; i++)
                    {
                        var layout = layouts[i];
                        LayoutManager.Current.RenameLayout(layout.LayoutName, $"S{i:0}");
                    }
                    trans.Commit();
                }
            }

            catch (System.Exception ex)
            {
                edt.WriteMessage("\nError >> " + ex.Message);
            }
        }
    }
}
