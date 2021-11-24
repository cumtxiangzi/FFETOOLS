using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    public class CreatDrawing : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector DrawingCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
            IList<Element> drawings = DrawingCollector.ToElements();
            List<string> DrawingType = new List<string>();
            foreach (Element elm in DrawingCollector)
            {
                FamilySymbol drw = elm as FamilySymbol;
                if ((!drw.FamilyName.Contains("图签")) && (!drw.FamilyName.Contains("版号")) && (!drw.FamilyName.Contains("会签")) && (!drw.FamilyName.Contains("做法")) && (!drw.FamilyName.Contains("非标")))
                {
                    DrawingType.Add(drw.FamilyName + "：" + drw.Name);
                }
            }

            List<Element> noTitleType = new List<Element>();
            foreach (Element elm in DrawingCollector)
            {
                FamilySymbol drw = elm as FamilySymbol;
                if (drw.FamilyName.Contains("共用_图纸"))
                {
                    noTitleType.Add(drw);
                }
            }



            try
            {
                using (Transaction transaction = new Transaction(doc, "批量创建图纸"))
                {
                    transaction.Start();
                    CreatDrawings crwForm = new CreatDrawings(DrawingType);
                    crwForm.ShowDialog();
                    int num = ExistingDrawingNumber(doc, crwForm.drawingMajorName);

                    if (crwForm.type.Contains("共用_图纸_A1"))
                    {
                        ViewSheet vws = null;
                        FamilySymbol title = null;
                        FamilySymbol edition = null;
                        title = TitleBlock(doc);
                        edition = Edition(doc);                     
                        XYZ titlelocation = new XYZ(1.88748115805841, 4.4597850267791, 0);
                        XYZ editionlocation = new XYZ(1.88748115805841, 4.60742282205469, 0);
                        for (int i = 0; i < crwForm.number; i++)
                        {
                            vws = CreateDrawingView(doc, crwForm.type, i + num + 1, crwForm.drawingMajorName);
                            FamilyInstance titlein = doc.Create.NewFamilyInstance(titlelocation, title, vws);
                            FamilyInstance editionin = doc.Create.NewFamilyInstance(editionlocation, edition, vws);
                            if (crwForm.CH_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(1);
                                editionin.LookupParameter("中文").Set(1);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(1);
                                editionin.LookupParameter("英文").Set(1);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.CH_EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(1);
                                editionin.LookupParameter("中英文").Set(1);
                            }

                        }
                    }
                    else if (crwForm.type.Contains("共用_图纸_A0"))
                    {
                        ViewSheet vws = null;
                        FamilySymbol title = null;
                        FamilySymbol edition = null;
                        title = TitleBlock(doc);
                        edition = Edition(doc);
                        XYZ titlelocation = new XYZ(3.02921344152298, 4.4597850267791, 0);
                        XYZ editionlocation = new XYZ(3.02921344152298, 4.60742282205469, 0);
                        for (int i = 0; i < crwForm.number; i++)
                        {
                            vws = CreateDrawingView(doc, crwForm.type, i + num + 1, crwForm.drawingMajorName);
                            FamilyInstance titlein = doc.Create.NewFamilyInstance(titlelocation, title, vws);
                            FamilyInstance editionin = doc.Create.NewFamilyInstance(editionlocation, edition, vws);
                            if (crwForm.CH_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(1);
                                editionin.LookupParameter("中文").Set(1);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(1);
                                editionin.LookupParameter("英文").Set(1);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.CH_EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(1);
                                editionin.LookupParameter("中英文").Set(1);
                            }
                        }
                    }
                    else if (crwForm.type.Contains("共用_图纸_A2"))
                    {
                        ViewSheet vws = null;
                        FamilySymbol title = null;
                        FamilySymbol edition = null;
                        title = TitleBlock(doc);
                        edition = Edition(doc);
                        XYZ titlelocation = new XYZ(1.07710058063059, 4.4597850267791, 0);
                        XYZ editionlocation = new XYZ(1.07710058063059, 4.60742282205469, 0);
                        for (int i = 0; i < crwForm.number; i++)
                        {
                            vws = CreateDrawingView(doc, crwForm.type, i + num + 1, crwForm.drawingMajorName);
                            FamilyInstance titlein = doc.Create.NewFamilyInstance(titlelocation, title, vws);
                            FamilyInstance editionin = doc.Create.NewFamilyInstance(editionlocation, edition, vws);
                            if (crwForm.CH_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(1);
                                editionin.LookupParameter("中文").Set(1);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(1);
                                editionin.LookupParameter("英文").Set(1);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.CH_EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(1);
                                editionin.LookupParameter("中英文").Set(1);
                            }
                        }
                    }
                    else if (crwForm.type.Contains("共用_图纸_A3"))
                    {
                        ViewSheet vws = null;
                        FamilySymbol title = null;
                        FamilySymbol edition = null;
                        title = TitleBlock(doc);
                        edition = Edition(doc);
                        XYZ titlelocation = new XYZ(0.52265176173295, 4.4597850267791, 0);
                        XYZ editionlocation = new XYZ(0.52265176173295, 4.60742282205469, 0);
                        for (int i = 0; i < crwForm.number; i++)
                        {
                            vws = CreateDrawingView(doc, crwForm.type, i + num + 1, crwForm.drawingMajorName);
                            FamilyInstance titlein = doc.Create.NewFamilyInstance(titlelocation, title, vws);
                            FamilyInstance editionin=doc.Create.NewFamilyInstance(editionlocation, edition, vws);
                            if (crwForm.CH_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(1);
                                editionin.LookupParameter("中文").Set(1);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(1);
                                editionin.LookupParameter("英文").Set(1);
                                titlein.LookupParameter("中英文").Set(0);
                                editionin.LookupParameter("中英文").Set(0);
                            }

                            if (crwForm.CH_EN_Button.IsChecked == true)
                            {
                                titlein.LookupParameter("中文").Set(0);
                                editionin.LookupParameter("中文").Set(0);
                                titlein.LookupParameter("英文").Set(0);
                                editionin.LookupParameter("英文").Set(0);
                                titlein.LookupParameter("中英文").Set(1);
                                editionin.LookupParameter("中英文").Set(1);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < crwForm.number; i++)
                        {
                            CreateDrawingView(doc, crwForm.type, i + num + 1, crwForm.drawingMajorName);
                        }
                    }
                    transaction.Commit();
                }
                return Result.Succeeded;

            }
            catch (Exception)
            {
                //throw;
                return Result.Failed;
            }
        }

        private ViewSheet CreateDrawingView(Document document, string typeName, int number, String name)
        {
            IEnumerable<FamilySymbol> familyList = from elem in new FilteredElementCollector(document).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                   let type = elem as FamilySymbol
                                                   where (type.FamilyName + "：" + type.Name).Equals(typeName)
                                                   select type;

            ViewSheet viewSheet = ViewSheet.Create(document, familyList.First().Id);
            viewSheet.SheetNumber = name + "-" + number.ToString().PadLeft(3, '0');
            if (null == viewSheet)
            {
                throw new Exception("创建图纸失败");
            }
            return viewSheet;
        }

        private int ExistingDrawingNumber(Document doc, string name)
        {
            FilteredElementCollector existingDrawing = new FilteredElementCollector(doc);
            existingDrawing.OfClass(typeof(ViewSheet));
            IList<Element> existingDrawings = existingDrawing.ToElements();
            List<ViewSheet> existingDrawingsc = new List<ViewSheet>();
            foreach (Element elm in existingDrawings)
            {
                ViewSheet drw = elm as ViewSheet;
                if (drw.SheetNumber.Contains(name))
                {
                    existingDrawingsc.Add(drw);
                }

            }
            return existingDrawingsc.Count;
        }

        private FamilySymbol TitleBlock(Document doc)
        {
            FilteredElementCollector titleCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
            IList<Element> titles = titleCollect.ToElements();
            FamilySymbol title = null;
            foreach (Element elm in titles)
            {
                FamilySymbol tl = elm as FamilySymbol;
                if (tl.FamilyName.Contains("共用_图纸_图签"))
                {
                    title = tl;
                    break;
                }
            }
            if (null == title)
            {
                MessageBox.Show("请载入共用_图纸_图签族", "错误");
            }
            return title;
        }

        private FamilySymbol Edition(Document doc)
        {
            FilteredElementCollector editionCollect = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
            IList<Element> editions = editionCollect.ToElements();
            FamilySymbol edition = null;
            foreach (Element elm in editions)
            {
                FamilySymbol ed = elm as FamilySymbol;
                if (ed.FamilyName.Contains("共用_图纸_版号"))
                {
                    edition = ed;
                    break;
                }
            }
            if (null == edition)
            {
                MessageBox.Show("请载入共用_图纸_版号栏族", "错误");
            }
            return edition;
        }

    }
}
