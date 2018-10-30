using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LISWebAPI.DeviceCategory;
using LISWebAPI.Processor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RCL.LISConnector.DataEntity.IOT;

namespace LISWebAPI.Controllers
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "processor")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DeviceMessagesController : ControllerBase
    {
        /// <summary>
        /// Post a device message for processing and conversion to patient diagnostic record entity
        /// </summary>
        /// <param name="deviceMessage">The device message entity</param>
        /// <response code="200">Returns a list of patient diagnostic record entities</response>
        /// <response code="400">If bad request</response>
        // POST: api/v1/DeviceMessages
        [HttpPost]
        [ProducesResponseType(typeof(List<PatientDiagnosticRecord>), 200)]
        [ProducesResponseType(400)]
        public IActionResult PostDeviceMessage([FromBody] DeviceMessage deviceMessage)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                IMessageProcessor _messageProcessor = null;
                string sendingFacility = deviceMessage.SendingFacility;

                #region HL7 messages

                if (deviceMessage.MessageType == Helpers.Constants.HL7)
                {
                    if (deviceMessage.DeviceCategory == "A")
                    {
                        _messageProcessor = new HL7MessageProcessorA();
                    };
                }

                #endregion

                #region ASTM messages

                if (deviceMessage.MessageType == Helpers.Constants.ASTM)
                {
                    if (deviceMessage.DeviceCategory == "A")
                    {
                        _messageProcessor = new ASTMMessageProcessorA();
                    };
                }

                #endregion

                #region POCT messages

                if (deviceMessage.MessageType == Helpers.Constants.ASTM)
                {
                    if (deviceMessage.DeviceCategory == "A")
                    {
                        _messageProcessor = new ASTMMessageProcessorA();
                    };
                }

                #endregion

                List<PatientDiagnosticRecord> records = _messageProcessor.ProcessMessage(deviceMessage);

                if (records?.Count > 0)
                {
                    return Ok(records);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest();
            }
        }
    }
}
