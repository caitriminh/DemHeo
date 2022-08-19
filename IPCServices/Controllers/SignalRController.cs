using IPC.SignalR;
using IPCServices;
using IPCServices.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalRController : ControllerBase
    {
        private readonly IHubContext<ApplicationHub> _hub;
        private IDataRepository _dataRepository;
        public SignalRController(IHubContext<ApplicationHub> hub, IDataRepository dataRepository)
        {
            this._hub = hub;
            _dataRepository = dataRepository;
        }

        [HttpPost("GetDevicesConnected")]
        public List<SignalUser> GetDevicesConnected()
        {
            List<SignalUser> signalUsers = ApplicationHub.Users.Values.ToList();
            return signalUsers;
        }

        //[HttpGet("UpdateApp")]
        //public IActionResult UpdateApp()
        //{
        //    _hub.Clients.All.SendAsync("CheckAutoUpdate");
        //    return Ok();
        //}

        //[HttpGet("UpdateAppNow")]
        //public IActionResult UpdateAppNow()
        //{
        //    _hub.Clients.All.SendAsync("UpdateNow");
        //    return Ok();
        //}

        //[HttpGet("ShowMessage")]
        //public IActionResult ShowMessage(string title, string content)
        //{
        //    _hub.Clients.All.SendAsync("ShowMessage", title, content);
        //    return Ok();
        //}

        [HttpGet("GetNumber")]
        public IActionResult GetNumber(string CageId)
        {
            _hub.Clients.All.SendAsync("GetNumber", CageId);
            return Ok();
        }



    }
}
