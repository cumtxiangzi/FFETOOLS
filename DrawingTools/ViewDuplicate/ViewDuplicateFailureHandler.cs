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
            //ʧ����Ϣ
            ErrorMessage = "";
            //ʧ������
            ErrorSeverity = "";
        }
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            //�������ʧ����Ϣ����������;���
            IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
            //����ʧ����Ϣ
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

                    //�������
                    if (failureSeverity == FailureSeverity.Warning)
                    {
                        //�������"������ʾ��ǽ�ص�"����ʱ�������ϴ����ô���
                        if (ErrorMessage.Contains("ĳЩ�ߴ��ע��ճ�������ж�ʧ���䲿�ֲ���") || ErrorMessage.Contains("��ɾ��ͼԪ"))
                        {
                            //�����ϴ����ô���
                            failure.SetCurrentResolutionType(FailureResolutionType.DeleteElements);
                            failuresAccessor.ResolveFailure(failure);
                            //�����������
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        else
                        {
                            //ɾ��������Ϣ
                            failuresAccessor.DeleteWarning(failure);
                            //������һ����
                            return FailureProcessingResult.Continue;
                        }
                    }

                    // ���������ȡ�����´���Ĳ�����������Ȼ������������  
                    if (failureSeverity == FailureSeverity.Error)
                    {
                        if (ErrorMessage.Contains("ĳЩ�ߴ��ע��ճ�������ж�ʧ���䲿�ֲ���")|| ErrorMessage.Contains("��ɾ��ͼԪ"))
                        {
                            //�����ϴε����ô���
                            failure.SetCurrentResolutionType(FailureResolutionType.DeleteElements);
                            if (failure.HasResolutions())
                            {
                                failuresAccessor.ResolveFailure(failure);
                            }

                            //�����������                            
                            return FailureProcessingResult.ProceedWithCommit;
                        }
                        if (ErrorMessage == "���ܽ�������")
                        {
                            //��������ع�
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
        /// ���������������ʼǰ����FailureHandler��ʼ�������
        /// </summary>
        public static void SetFailedHandlerBeforeTransaction(IFailuresPreprocessor failureHandler, Transaction transaction)
        {
            FailureHandlingOptions failureHandlingOptions = transaction.GetFailureHandlingOptions();
            failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
            // ��仰�ǹؼ�  
            //failureHandlingOptions.SetClearAfterRollback(true);
            transaction.SetFailureHandlingOptions(failureHandlingOptions);
        }
    }
}
