using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.ApplicationServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class SwitchBackGround : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)        {            UIDocument uidoc = commandData.Application.ActiveUIDocument;            Document doc = uidoc.Document;            Color black = new Color(0, 0, 0);            Color white = new Color(255, 255, 255);

            using (Transaction trans = new Transaction(doc, "背景颜色"))
            {
                trans.Start();
                Color col = doc.Application.BackgroundColor;
                if ((col.Red == 255) && (col.Blue == 255) && (col.Green == 255))
                {
                    col = new Color(0, 0, 0);
                    doc.Application.BackgroundColor = col;

                }
                else if ((col.Red == 0) && (col.Blue == 0) && (col.Green == 0))
                {
                    col = new Color(255, 255, 255);
                    doc.Application.BackgroundColor = col;
                }
                trans.Commit();
            }
            return Result.Succeeded;
        }
    }
}
