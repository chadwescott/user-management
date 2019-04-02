using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using UserManager.Domain.Requests;
using UserManager.Domain.Responses;

namespace UserManager.Api.Controllers
{
    public class LoginController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public LoginController(
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IEmailSender emailSender)
            : base(mapper)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Creates a new login.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route(Routes.Login)]
        [SwaggerOperation(OperationId = "register", Tags = new[] { Categories.Logins })]
        [SwaggerResponse(200, Type = typeof(LoginResponse))]
        [SwaggerResponse(400)]
        [SwaggerResponse(404)]
        [SwaggerResponse(500)]
        public async  Task<IActionResult> Register(LoginRequest request)
        {
            var user = Mapper.Map<IdentityUser>(request);
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", code);

                return new OkObjectResult(Mapper.Map<LoginResponse>(user));
            }
            else
            {
                var errors = result.Errors.Select(x => x.Description).ToArray();
                throw new ArgumentException(string.Join("\n", errors));
            }
        }
    }
}