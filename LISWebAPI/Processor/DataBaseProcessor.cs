using RCL.LISConnector.DataEntity.Services;
using RCL.LISConnector.DataEntity.SQL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LISMessageProcessorAPI.Processor
{
    public class DatabaseProcessor
    { 

        protected async Task<Patient> GetPatientFromDatabaseAsync(string apiBaseUrl, int patientId)
        {
            Patient _patient = new Patient();
            AccessToken token = await GetAccessTokenAsync();
            PatientService svc = new PatientService(apiBaseUrl, token.access_token);

            try
            {
                Patient existingPatient = await svc.GetPatientByIdAsync(patientId);
                if (existingPatient?.Id > 0)
                {
                    _patient = existingPatient;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return _patient;
        }

        protected async Task<int> AddDiagnosticReportToDatabaseAsync(string apiBaseUrl, Patient patient, DiagnosticReport diagnosticReport, List<Result> results)
        {
            AccessToken token = await GetAccessTokenAsync();
            DiagnosticReportService drsvc = new DiagnosticReportService(apiBaseUrl, token.access_token);
            ResultService rsvc = new ResultService(apiBaseUrl, token.access_token);
            int diagnosticReportId = 0;

            try
            {
                PatientService svc = new PatientService(apiBaseUrl, token.access_token);
                diagnosticReport.PatientId = patient.Id;
                diagnosticReport.GivenName = patient.GivenName;
                diagnosticReport.FamilyName = patient.FamilyName;
                diagnosticReport.DateOfBirth = patient.DateOfBirth;

                DiagnosticReport newReport = await drsvc.PostDiagnosticReportAsync(diagnosticReport);
                if (newReport?.Id > 0)
                {
                    diagnosticReportId = newReport.Id;

                    if (results?.Count > 0)
                    {
                        foreach (var item in results)
                        {
                            item.DiagnosticReportId = newReport.Id;
                            Result newResult = await rsvc.PostResultAsync(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return diagnosticReportId;
        }

        private async Task<AccessToken> GetAccessTokenAsync()
        {
            AccessToken token = new AccessToken();

            try
            {
                //token = await WebService.GetTokenAsync(_oauthCredentials.TokenUrl, _oauthCredentials.ClientId, _oauthCredentials.ClientSecret, _oauthCredentials.Resource);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return token;
        }
    }
}
