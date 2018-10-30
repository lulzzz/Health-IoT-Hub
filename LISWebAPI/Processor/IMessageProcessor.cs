using RCL.LISConnector.DataEntity.IOT;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LISWebAPI.Processor
{
    public interface IMessageProcessor
    {
        List<PatientDiagnosticRecord> ProcessMessage(DeviceMessage deviceMessage);
    }
}
