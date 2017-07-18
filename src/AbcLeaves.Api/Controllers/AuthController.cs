﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AbcLeaves.Api.Domain;
using AbcLeaves.Core;

namespace AbcLeaves.Api.Controllers
{
    [Route("api/auth")]
    [Authorize(ActiveAuthenticationSchemes = "Bearer")]
    public class AuthController : Controller
    {
        private readonly GoogleCalendarManager googleCalManager;

        public AuthController(GoogleCalendarManager googleCalManager)
        {
            this.googleCalManager = googleCalManager;
        }

        // POST /api/auth/googlecal/
        [HttpPost("googlecal")]
        public async Task<IActionResult> GrantAccessToGoogleCalendar(
            [FromQuery]string code,
            [FromQuery]string redirectUrl)
        {
            var result = await googleCalManager.GrantAccess(code, redirectUrl, HttpContext.User);
            return result.ToMvcActionResult();
        }
    }
}