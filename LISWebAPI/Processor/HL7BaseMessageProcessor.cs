using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;
using RCL.LISConnector.HL7Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LISWebAPI.Processor
{
    public abstract class HL7BaseMessageProcessor : IMessageProcessor
    {
        private string HL7AppName = Helpers.Constants.ReceivingAppName;
        private string HL7Facility = Helpers.Constants.ReceivingAppFacility;

        protected virtual Patient GetPatient(Message message)
        {
            Patient patient = new Patient();

            try
            {
                string INTERNALPATIENTID = message.GetValue("PID.3");
                if (!string.IsNullOrEmpty(INTERNALPATIENTID))
                    patient.InternalPatientId = INTERNALPATIENTID;

                string FAMILYNAME = message.GetValue("PID.5.1");
                if (!string.IsNullOrEmpty(FAMILYNAME))
                    patient.FamilyName = FAMILYNAME;

                string GIVENNAME = message.GetValue("PID.5.2");
                if (!string.IsNullOrEmpty(GIVENNAME))
                    patient.GivenName = GIVENNAME;

                string MIDDLENAME = message.GetValue("PID.5.3");
                if (!string.IsNullOrEmpty(MIDDLENAME))
                    patient.MiddleName = MIDDLENAME;

                string DATEOFBIRTH = message.GetValue("PID.7");
                if (!string.IsNullOrEmpty(DATEOFBIRTH))
                    patient.DateOfBirth = Helpers.Converters.ConvertStringToDate(message.GetValue("PID.7"), "yyyyMMdd");

                string SEX = message.GetValue("PID.8");
                if (!string.IsNullOrEmpty(SEX))
                    patient.Sex = SEX;

                string RACE = message.GetValue("PID.10");
                if (!string.IsNullOrEmpty(RACE))
                    patient.Race = RACE;

                string ACCOUNTNUMBER = message.GetValue("PID.18");
                if (!string.IsNullOrEmpty(ACCOUNTNUMBER))
                    patient.AccountNumber = ACCOUNTNUMBER;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return patient;
        }

        protected virtual DiagnosticReport GetDiagnosticReport(Message message)
        {
            DiagnosticReport _diagnosticReport = null;

            try
            {
                string MessageType = message.GetValue("MSH.9.2");
                if (!string.IsNullOrEmpty(MessageType))
                {
                    if (MessageType == "R01")
                    {
                        var diagnosticReport = new DiagnosticReport();

                        diagnosticReport.ReceivingApplication = HL7AppName;
                        diagnosticReport.ReceivingFacility = HL7Facility;

                        string SENDINGAPPLICATION = message.Segments("MSH")[0]?.Fields(2)?.Value;
                        if (!string.IsNullOrEmpty(SENDINGAPPLICATION))
                            diagnosticReport.SendingApplication = SENDINGAPPLICATION;

                        string SENDINGFACILITY = message.Segments("MSH")[0]?.Fields(3)?.Value;
                        if (!string.IsNullOrEmpty(SENDINGFACILITY))
                            diagnosticReport.SendingApplication = SENDINGFACILITY;

                        string PATID = message.Segments("OBX")[0]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(PATID))
                            diagnosticReport.PatientInternalId = PATID;

                        string VISITID = message.Segments("OBX")[1]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(VISITID))
                            diagnosticReport.VisitId = VISITID;

                        string FAMILYNAME = message.Segments("OBX")[2]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(FAMILYNAME))
                            diagnosticReport.FamilyName = FAMILYNAME;

                        string GIVENNAME = message.Segments("OBX")[3]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(GIVENNAME))
                            diagnosticReport.GivenName = GIVENNAME;

                        string SEX = message.Segments("OBX")[4]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(SEX))
                            diagnosticReport.Sex = SEX;

                        string DATEOFBIRTH = message.Segments("OBX")[5]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(DATEOFBIRTH))
                            diagnosticReport.DateOfBirth = Helpers.Converters.ConvertStringToDate(DATEOFBIRTH, "yyyyMMdd");

                        string ANALYZERNAME = message.Segments("OBX")[6]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(ANALYZERNAME))
                            diagnosticReport.AnalyzerName = ANALYZERNAME;

                        string ANALYZEDATETIME = message.Segments("OBX")[7]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(ANALYZEDATETIME))
                            diagnosticReport.AnalyzerDateTime = Helpers.Converters.ConvertStringToDate(ANALYZEDATETIME, "yyyyMMddHHmmss");

                        string OPID = message.Segments("OBX")[8]?.Fields(5)?.Value;
                        if (!string.IsNullOrEmpty(OPID))
                            diagnosticReport.OperatorId = OPID;

                        _diagnosticReport = diagnosticReport;

                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _diagnosticReport;

        }

        protected virtual List<Result> GetResults(Message message)
        {
            List<Result> _results = null;

            try
            {
                string MessageType = message.GetValue("MSH.9.2");
                if (!string.IsNullOrEmpty(MessageType))
                {
                    if (MessageType == "R01")
                    {
                        var results = new List<Result>();

                        List<Segment> OBXList = message.Segments("OBX");
                        int NoSegments = OBXList.Count;
                        Dictionary<int, string> dictComments = new Dictionary<int, string>();

                        for (int i = 9; i < NoSegments; i++)
                        {
                            var result = new Result();
                            
                            if (!message.Segments("OBX")[i].Fields(3).Value.Contains("COMMENT"))
                            {
                                string TESTCODE = message.Segments("OBX")[i]?.Fields(3)?.Value;
                                if (!string.IsNullOrEmpty(TESTCODE))
                                    result.TestCode = TESTCODE;
                                string VALUE = message.Segments("OBX")[i]?.Fields(5)?.Value;
                                if (!string.IsNullOrEmpty(VALUE))
                                    result.Value = VALUE;
                                string UNITS = message.Segments("OBX")[i]?.Fields(6)?.Value;
                                if (!string.IsNullOrEmpty(UNITS))
                                    result.Units = UNITS;
                                string REFERENCERANGE = message.Segments("OBX")[i]?.Fields(7)?.Value;
                                if (!string.IsNullOrEmpty(REFERENCERANGE))
                                    result.ReferenceRange = REFERENCERANGE;
                                string ABNORMALFLAGS = message.Segments("OBX")[i]?.Fields(8)?.Value;
                                if (!string.IsNullOrEmpty(ABNORMALFLAGS))
                                    result.AbnormalFlags = ABNORMALFLAGS;
                                string RESULTDATETIME = message.Segments("OBX")[i]?.Fields(14)?.Value;
                                result.ResultDateTime = Helpers.Converters.ConvertStringToDate(RESULTDATETIME, "yyyyMMddHHmmss");

                                results.Add(result);
                            }
                            if (message.Segments("OBX")[i].Fields(3).Value.Contains("COMMENT"))
                            {
                                string COMMENTS = message.Segments("OBX")[i]?.Fields(5)?.Value;
                                if (!string.IsNullOrEmpty(COMMENTS))
                                {
                                    string tag = message.Segments("OBX")[i].Fields(3).Value;
                                    string tagend = tag.Substring(tag.Length - 1);
                                    int num = 0;
                                    int.TryParse(tagend, out num);
                                    if (num > 0)
                                    {
                                        dictComments.Add(num, COMMENTS);
                                    }
                                }
                            }
                        }

                        if (results?.Count > 0)
                        {
                            for (int i = 1; i < (results.Count + 1); i++)
                            {
                                if (!string.IsNullOrEmpty(dictComments[i]))
                                {
                                    results[i - 1].Comments = dictComments[i];
                                }

                            }
                        }

                        _results = results;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _results;
        }

        public virtual List<PatientDiagnosticRecord> ProcessMessage(DeviceMessage deviceMessage)
        {
            List<PatientDiagnosticRecord> records = new List<PatientDiagnosticRecord>();

            try
            {
                string strMessage = deviceMessage.ContentsList[0];
                List<Message> Messages = GetMessages(strMessage);
                foreach (Message message in Messages)
                {
                    Patient _patient = GetPatient(message);
                    string PatientInternalId = _patient?.InternalPatientId;
                    if (!string.IsNullOrEmpty(PatientInternalId))
                    {
                        var _diagnosticReport = GetDiagnosticReport(message);
                        if (_diagnosticReport != null)
                        {
                            var _results = GetResults(message);
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return records;
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

        private List<Message> GetMessages(string Content)
        {
            List<Message> messages = new List<Message>();

            try
            {
                var _messages = MessageHelper.ExtractMessages(Content);
                foreach (var strMsg in _messages)
                {
                    Message _message = new Message(strMsg);
                    bool isParsed = false;
                    try
                    {
                        isParsed = _message.ParseMessage();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    messages.Add(_message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return messages;
        }
    }
}
