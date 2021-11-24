using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class ImportFamilyImage : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            try
            {
                FileSystemInfo familypath = PickFolderPath();
                if (familypath == null)
                {
                    return Result.Cancelled;
                }
                List<FileInfo> familyFileList = GetFamilyFiles(familypath);
                if (familyFileList == null)
                {
                    return Result.Cancelled;
                }
                ExportImage(uiapp, familyFileList);

            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        private FileSystemInfo PickFolderPath()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择族文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return new DirectoryInfo(dialog.SelectedPath);
            }
            return null;
        }

        private List<FileInfo> GetFamilyFiles(FileSystemInfo familypath)
        {
            List<FileInfo> familyFileList = new List<FileInfo>();
            if (!familypath.Exists)
            {
                return null;
            }
            DirectoryInfo dir = familypath as DirectoryInfo;
            if (dir == null)
            {
                return null;
            }
            familyFileList.AddRange(dir.GetFiles("*.rfa"));
            foreach (DirectoryInfo dInfor in dir.GetDirectories())
            {
                familyFileList.AddRange(GetFamilyFiles(dInfor));
            }
            return familyFileList;
        }

        private bool ExportImage(UIApplication uiapp, List<FileInfo> familyFileList)
        {
            Document doc = null;
            foreach (FileInfo file in familyFileList)
            {
                UIDocument newUIDoc = uiapp.OpenAndActivateDocument(file.FullName);
                Document newDoc = newUIDoc.Document;
                if(doc!=null)
                doc.Close(false);
                doc = newDoc;
                FilteredElementCollector viewCollector = new FilteredElementCollector(newDoc);
                viewCollector.OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View3D));
                if (viewCollector.Count() == 0)
                {
                    continue;
                }
                View3D view = viewCollector.First() as View3D;
                newUIDoc.ActiveView = view;
                using (Transaction trans = new Transaction(newDoc, "设置视图"))
                {
                    trans.Start();
                    view.Scale = 5;
                    view.DisplayStyle = DisplayStyle.RealisticWithEdges;
                    view.DetailLevel = ViewDetailLevel.Fine;
                    view.OrientTo(new XYZ(-0.577350269189626, 0.577350269189626, -0.577350269189626));
                    trans.Commit();
                }
                ImageExportOptions option = new ImageExportOptions();
                option.FilePath = file.FullName;
                option.ZoomType = ZoomFitType.FitToPage;
                option.PixelSize = 1000;
                option.ImageResolution = ImageResolution.DPI_300;
                newDoc.ExportImage(option);

            }

            return true;
        }

    }
}
