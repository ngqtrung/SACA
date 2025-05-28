using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.LeaderBoard.Request;
using SACA_Common.DTOs.LeaderBoard.Response;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.Routes;
using SACA_Service.Services;

namespace SACA_API.Controllers
{
    [Route(LeaderBoardRoute.INDEX)]
    [ApiController]
    public class LeaderBoardController : AuthorizeController
    {
        private ILeaderBoardService _leaderBoardService;
        private readonly IContestService _contestService;
        public LeaderBoardController
        (
            ILeaderBoardService leaderBoardService,
            IContestService contestService,
            ILogger<ILeaderBoardService> logger
        ) : base(logger)
        {
            _leaderBoardService = leaderBoardService;
            _contestService = contestService;
        }
        [HttpGet(LeaderBoardRoute.ACTION.SEARCH)]
        public async Task<IActionResult> Search([FromQuery] LeaderBoardTableFilter filter)
        {
            try
            {
                return Ok(new Response<LeaderBoardTableView>(await _leaderBoardService.Search(filter, true, UserHeader.role == "Lecturer")));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost(LeaderBoardRoute.ACTION.Export)]
        public async Task<IActionResult> Export([FromBody] LeaderBoardTableFilter filter)
        {
            try
            {
                var contest = await _contestService.GetDetailAsync(filter.contest_id);
                var bytes = await _leaderBoardService.ExportLeaderBoard(filter);
                var fileName = $"{contest.code}_LeaderBoard.xlsx";
                //return File(bytes, "application/octet-stream", fileName);
                return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
