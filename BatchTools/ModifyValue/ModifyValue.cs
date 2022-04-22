using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace FFETOOLS
{
    [Transaction(TransactionMode.Manual)]
    class ModifyValue : IExternalCommand//批量修改参数
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            ModifyValueWindow uiForm = new ModifyValueWindow(document);
            System.Windows.Interop.WindowInteropHelper thisForm = new System.Windows.Interop.WindowInteropHelper(uiForm)
            {
                Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle
            };
            if (uiForm.ShowDialog() == true)
            {
                using (Transaction tran = new Transaction(document, "修改 参数值"))
                {
                    tran.Start();

                    //0——文本替换
                    //1——加前缀
                    //2——加后缀
                    switch (uiForm.ModifyType)
                    {
                        case 0:
                            foreach (Element element in uiForm.GetElements)
                            {
                                Parameter parameter = element.get_Parameter(uiForm.ParameterDefinition);
                                if (parameter != null)
                                {
                                    parameter.Set(parameter.AsString().Replace(uiForm.Text1, uiForm.Text2));
                                }

                            }
                            break;
                        case 1:
                            foreach (Element element in uiForm.GetElements)
                            {
                                Parameter parameter = element.get_Parameter(uiForm.ParameterDefinition);
                                if (parameter != null)
                                {
                                    if (parameter.AsString() != "")
                                    {
                                        parameter.Set(uiForm.Text1);
                                    }
                                    else
                                    {
                                        parameter.Set(parameter.AsString().Insert(0, uiForm.Text1));
                                    }

                                }
                            }
                            break;
                        case 2:
                            foreach (Element element in uiForm.GetElements)
                            {
                                Parameter parameter = element.get_Parameter(uiForm.ParameterDefinition);
                                if (parameter != null)
                                {
                                    parameter.Set(parameter.AsString() + uiForm.Text1);
                                }
                            }
                            break;
                    }

                    tran.Commit();
                }
            }

            return Result.Succeeded;
        }

    }
}
