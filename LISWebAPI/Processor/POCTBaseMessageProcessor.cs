using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;
using RCL.LISConnector.POCTParser;
using RCL.LISConnector.POCTParser.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace LISWebAPI.Processor
{
    public abstract class POCTBaseMessageProcessor : IMessageProcessor
    {
        private string _ReceivingAppName = Helpers.Constants.ReceivingAppName;
        private string _ReceivingAppFacility = Helpers.Constants.ReceivingAppFacility;
        private readonly string _SendingFacility;

        protected virtual RCL.LISConnector.DataEntity.SQL.Patient GetPatient(Service svc)
        {
            RCL.LISConnector.DataEntity.SQL.Patient patient = new RCL.LISConnector.DataEntity.SQL.Patient();

            try
            {
                if (!string.IsNullOrEmpty(svc?.patient?.patient_id?.Value))
                    patient.InternalPatientId = svc.patient.patient_id.Value;
                if (!string.IsNullOrEmpty(svc?.patient?.patientName?.family?.Value))
                    patient.FamilyName = svc.patient.patientName.family.Value;
                if (!string.IsNullOrEmpty(svc?.patient?.patientName?.given?.Value))
                    patient.FamilyName = svc.patient.patientName.given.Value;
                if (!string.IsNullOrEmpty(svc?.patient?.birth_date?.Value))
                    patient.DateOfBirth = ConvertStringToDateTime(svc.patient.birth_date.Value);
                if (!string.IsNullOrEmpty(svc?.patient?.gender_cd?.Value))
                    patient.Sex = svc?.patient?.gender_cd?.Value;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }

            return patient;
        }

        protected virtual DiagnosticReport GetDiagnosticReport(Service svc, string helr01)
        {
            DiagnosticReport _report = new DiagnosticReport();
            List<Service> lstService = new List<Service>();

            try
            {
                HELR01 _helr01 = Serialization.DeserializeObject<HELR01>(helr01);

                if (!string.IsNullOrEmpty(_helr01?.header?.creation_dttm?.Value))
                    _report.AnalyzerDateTime = ConvertStringToDateTime(_helr01.header.creation_dttm.Value);
                else
                    _report.AnalyzerDateTime = DateTime.Now;

                if (!string.IsNullOrEmpty(_helr01?.device?.device_name?.Value))
                    _report.AnalyzerName = _helr01.device.device_name.Value;

                if (!string.IsNullOrEmpty(svc?.operatorid?.operator_id?.Value))
                    _report.OperatorId = svc.operatorid.operator_id.Value;

                if (!string.IsNullOrEmpty(svc?.patient?.patient_id?.Value))
                    _report.PatientId = Convert.ToInt32(svc.patient.patient_id.Value);

                _report.ReceivingApplication = _ReceivingAppName;
                _report.ReceivingFacility = _ReceivingAppFacility;

                if (!string.IsNullOrEmpty(_helr01?.device?.device_name?.Value))
                    _report.SendingApplication = _helr01.device.device_name.Value;
                    _report.SendingFacility = _helr01.device.device_name.Value;

                if (svc?.patient?.observations != null)
                    _report.TestCodes = GetTestCodes(svc.patient.observations);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return _report;
        }

        protected virtual List<Result> GetResults(Service svc)
        {
            List<Result> _results = new List<Result>();

            try
            {
                Observation[] obs = svc.patient.observations;
                List<Observation> lstObs = obs.OfType<Observation>().ToList();
                foreach (Observation item in lstObs)
                {
                    Result result = new Result();

                    if (!string.IsNullOrEmpty(item?.normal_lo_hi_limit?.Value) && !string.IsNullOrEmpty(item?.critical_lo_hi_limit?.Value))
                        result.ReferenceRange = $"[Normal]{item.normal_lo_hi_limit.Value} [Critical]{item.critical_lo_hi_limit.Value} {item.value.Unit}";

                    if (!string.IsNullOrEmpty(svc?.observation_dttm?.Value))
                        result.ResultDateTime = ConvertStringToDateTime(svc.observation_dttm.Value);
                    else
                        result.ResultDateTime = DateTime.Now;

                    if (!string.IsNullOrEmpty(item?.observation_id?.Value))
                        result.TestCode = item.observation_id.Value;

                    if (!string.IsNullOrEmpty(item?.value?.Unit))
                        result.Units = item.value.Unit;

                    if (!string.IsNullOrEmpty(item?.value?.Value))
                        result.Value = item.value.Value;

                    if (item?.notes != null)
                    {
                        Note defaultNote = item.notes[0];
                        if (!string.IsNullOrEmpty(defaultNote?.text?.Value))
                        {
                            result.Comments = defaultNote.text.Value;
                        }
                    }

                    _results.Add(result);
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
                string strOBSR01 = deviceMessage.ContentsList[0];
                string strHELR01 = deviceMessage.ContentsList[1];

                OBSR01 _obsr01 = Serialization.DeserializeObject<OBSR01>(strOBSR01);
                Service[] services = _obsr01.services;
                List<Service> lstService = services.OfType<Service>().ToList();
                foreach (Service svc in lstService)
                {
                    RCL.LISConnector.DataEntity.SQL.Patient _patient = GetPatient(svc);
                    if (!string.IsNullOrEmpty(_patient?.InternalPatientId))
                    {
                        DiagnosticReport _diagnosticReport = GetDiagnosticReport(svc, strHELR01);
                        if (!string.IsNullOrEmpty(_diagnosticReport?.AnalyzerDateTime.ToString()))
                        {
                            List<Result> _results = GetResults(svc);
                            if (_results?.Count > 0)
                            {
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

        private DateTime ConvertStringToDate(string date)
        {
            DateTime dt = new DateTime();

            try
            {
                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    if (!DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:ss:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return dt.Date;
        }

        private DateTime ConvertStringToDateTime(string date)
        {
            DateTime dt = new DateTime();

            try
            {
                if (!DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:ss:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                {
                    if (!DateTime.TryParseExact(date, "yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return dt;
        }

        private string GetTestCodes(Observation[] Obs)
        {
            string r = string.Empty;

            try
            {
                List<Observation> lstObs = Obs.OfType<Observation>().ToList();

                if (lstObs?.Count > 0)
                {
                    foreach (Observation obs in lstObs)
                    {
                        if (r?.Length < 350)
                        {
                            r = $"{r},{obs.observation_id?.Value ?? string.Empty}";
                        }
                    }

                    r = r.TrimStart(',');
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return r;
        }
    }
}
