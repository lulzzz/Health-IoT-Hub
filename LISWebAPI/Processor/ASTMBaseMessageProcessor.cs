using RCL.LISConnector.ASTMParser;
using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace LISWebAPI.Processor
{
    public class ASTMBaseMessageProcessor : IMessageProcessor
    {
        private string ReceivingAppName = Helpers.Constants.ReceivingAppName;
        private string ReceivingFacility = Helpers.Constants.ReceivingAppFacility;

        private readonly string _SendingFacility;
        private Message _astmMessage;

        protected virtual Patient GetPatient()
        {
            Patient patient = new Patient();

            try
            {
                string INTERNALPATIENTID = _astmMessage.GetValue("P.3");
                if (!string.IsNullOrEmpty(INTERNALPATIENTID))
                    patient.InternalPatientId = INTERNALPATIENTID;

                string FAMILYNAME = _astmMessage.GetValue("P.6.1");
                if (!string.IsNullOrEmpty(FAMILYNAME))
                    patient.FamilyName = FAMILYNAME;

                string GIVENNAME = _astmMessage.GetValue("P.6.2");
                if (!string.IsNullOrEmpty(GIVENNAME))
                    patient.GivenName = GIVENNAME;

                string MIDDLENAME = _astmMessage.GetValue("P.6.3");
                if (!string.IsNullOrEmpty(MIDDLENAME))
                    patient.MiddleName = MIDDLENAME;

                string DATEOFBIRTH = _astmMessage.GetValue("PID.8");
                if (!string.IsNullOrEmpty(DATEOFBIRTH))
                    patient.DateOfBirth = Helpers.Converters.ConvertStringToDate(_astmMessage.GetValue("PID.7"), "yyyyMMdd");

                string SEX = _astmMessage.GetValue("PID.9");
                if (!string.IsNullOrEmpty(SEX))
                    patient.Sex = SEX;

                string RACE = _astmMessage.GetValue("PID.10");
                if (!string.IsNullOrEmpty(RACE))
                    patient.Race = RACE;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return patient;
        }

        protected virtual DiagnosticReport GetDiagnosticReport()
        {
            DiagnosticReport diagnosticReport = new DiagnosticReport();

            try
            {
                diagnosticReport.ReceivingApplication = ReceivingAppName;
                diagnosticReport.ReceivingFacility = ReceivingFacility;

                string PatientId = _astmMessage.GetValue("P.3");
                if (!string.IsNullOrEmpty(PatientId))
                    diagnosticReport.PatientId = Convert.ToInt32(PatientId);

                string AnalyzerName = _astmMessage.GetValue("H.5");
                if (!string.IsNullOrEmpty(AnalyzerName))
                    diagnosticReport.AnalyzerName = AnalyzerName;
                    diagnosticReport.SendingApplication = AnalyzerName;
                    diagnosticReport.SendingFacility = AnalyzerName;

                string AnalyzerDateTime = _astmMessage.GetValue("H.14");
                if (!string.IsNullOrEmpty(AnalyzerDateTime))
                    diagnosticReport.AnalyzerDateTime = Helpers.Converters.ConvertStringToDate(AnalyzerDateTime, "yyyyMMddHHmmss");

                string OperatorId = _astmMessage.GetValue("R.11");
                if (!string.IsNullOrEmpty(OperatorId))
                    diagnosticReport.OperatorId = OperatorId;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return diagnosticReport;
        }

        protected virtual List<Result> GetResults()
        {
            List<Result> lstResults = new List<Result>();

            try
            {
                Result result = new Result();

                string TestCode = _astmMessage.GetValue("R.3.4");
                if (!string.IsNullOrEmpty(TestCode))
                    result.TestCode = TestCode.Replace("^", string.Empty);

                string Value = _astmMessage.GetValue("R.4");
                if (!string.IsNullOrEmpty(Value))
                    result.Value = Value;

                string Units = _astmMessage.GetValue("R.5");
                if (!string.IsNullOrEmpty(Units))
                    result.Units = Units;

                string RefRange = _astmMessage.GetValue("R.6");
                if (!string.IsNullOrEmpty(RefRange))
                    result.ReferenceRange = RefRange;

                string AbFlags = _astmMessage.GetValue("R.7");
                if (!string.IsNullOrEmpty(AbFlags))
                    result.AbnormalFlags = AbFlags;

                string RsltDateTime = _astmMessage.GetValue("R.13");
                result.ResultDateTime = Helpers.Converters.ConvertStringToDate(RsltDateTime, "yyyyMMddHHmmss");

                string Comments = _astmMessage.GetValue("C.4");
                if (!string.IsNullOrEmpty(Comments))
                    result.ReferenceRange = Comments;

                lstResults.Add(result);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return lstResults;
        }

        public virtual List<PatientDiagnosticRecord> ProcessMessage(DeviceMessage deviceMessage)
        {
            string strRawMessage = deviceMessage.ContentsList[0];
            string strMessage = ExtractMessage(strRawMessage);
            Message msg = new Message(strMessage);
            bool b = msg.ParseMessage();
            _astmMessage = msg;

            List<PatientDiagnosticRecord> records = new List<PatientDiagnosticRecord>();
            Patient _patient = GetPatient();

            if (!string.IsNullOrEmpty(_patient?.InternalPatientId))
            {
                DiagnosticReport _diagnosticReport = GetDiagnosticReport();
                if (!string.IsNullOrEmpty(_diagnosticReport?.SendingFacility))
                {
                    List<Result> _results = GetResults();
                    if (_results?.Count > 0)
                    {
                        string TestCodes = GetTestCodesFromResults(_results);
                        if (!string.IsNullOrEmpty(TestCodes))
                        {
                            _diagnosticReport.TestCodes = TestCodes;
                        }

                        PatientDiagnosticRecord _record = new PatientDiagnosticRecord
                        {
                            patient = _patient,
                            diagnosticReport = _diagnosticReport,
                            results = _results
                        };

                        records.Add(_record);
                    }
                }
            }

            return records;
        }

        protected virtual string ExtractMessage(string message)
        {
            string msg = string.Empty;
            StringBuilder sb = new StringBuilder();
            try
            {
                var expr = "\x02(.*?)\x03";
                var matches = Regex.Matches(message, expr);
                var list = new List<string>();
                foreach (Match m in matches)
                {
                    sb = sb.Append(m.Groups[1].Value.Remove(0, 1));
                }

                msg = sb.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return msg;
        }

        private string GetTestCodesFromResults(List<Result> Results)
        {
            string r = string.Empty;

            try
            {
                if (Results?.Count > 0)
                {
                    foreach (var item in Results)
                    {
                        if (r?.Length < 350)
                        {
                            r = $"{r},{item.TestCode}";
                        }
                    }

                    r = r.TrimStart(',');
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return r;
        }
    }
}
