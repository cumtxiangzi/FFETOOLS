using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Architecture;

namespace FFETOOLS
{
    public class ViewDuplicateFailureHandler : IFailuresPreprocessor
    {
        public string ErrorMessage { set; get; }
        public string ErrorSeverity { set; get; }
        public ViewDuplicateFailureHandler()
        {
            //失败信息
            ErrorMessage = "";
            //失败类型
            ErrorSeverity = "";
        }
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            //获得所有失败信息，包括错误和警告
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
            //遍历失败信息
            foreach (FailureMessageAccessor failure in failureMessages)
            {
                FailureDefinitionId id = failure.GetFailureDefinitionId();
                try
                {
                    ErrorMessage = failure.GetDescriptionText();
                }
                catch
                {
                    ErrorMessage = "Unknown Error";
                }
                try
                {
                    FailureSeverity failureSeverity = failure.GetSeverity();
                    ErrorSeverity = failureSeverity.ToString();

                    //警告框处理
                    if (failureSeverity == FailureSeverity.Warning)
                    {
                        //例如出现"高亮显示的墙重叠"警告时，根据上次设置处理
                        if (ErrorMessage.Contains("某些尺寸标注在粘贴过程中丢失了其部分参照") || ErrorMessage.Contains("已删除图元"))
                        {
                            //根据上次设置处理
                            failure.SetCurrentResolutionType(FailureResolutionType.DeleteElements);
                            failuresAccessor.ResolveFailure(failure);
                            //返回事务完成
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        else
                        {
                            //删除警告信息
                            failuresAccessor.DeleteWarning(failure);
                            //继续下一操作
                            return FailureProcessingResult.Continue;
                        }
                    }

                    // 错误框处理：则取消导致错误的操作，但是仍然继续整个事务  
                    if (failureSeverity == FailureSeverity.Error)
                    {
                        if (ErrorMessage.Contains("某些尺寸标注在粘贴过程中丢失了其部分参照")|| ErrorMessage.Contains("已删除图元"))
                        {
                            //根据上次的设置处理
                            failure.SetCurrentResolutionType(FailureResolutionType.DeleteElements);
                            if (failure.HasResolutions())
                            {
                                failuresAccessor.ResolveFailure(failure);
                            }

                            //返回事务完成                            
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        if (ErrorMessage == "不能进行拉伸")
                        {
                            //返回事务回滚
                            return FailureProcessingResult.ProceedWithRollBack;
                        }
                    }

                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {

                }
            }
            return FailureProcessingResult.Continue;
        }

        /// <summary>
        /// 这个方法用在事务开始前，在FailureHandler初始化后调用
        /// </summary>
        public static void SetFailedHandlerBeforeTransaction(IFailuresPreprocessor failureHandler, Transaction transaction)
        {
            FailureHandlingOptions failureHandlingOptions = transaction.GetFailureHandlingOptions();
            failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
            // 这句话是关键  
            //failureHandlingOptions.SetClearAfterRollback(true);
            transaction.SetFailureHandlingOptions(failureHandlingOptions);
        }
    }
}
