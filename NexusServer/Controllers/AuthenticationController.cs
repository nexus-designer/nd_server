using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusServer.Interfaces;
using NexusServer.Model;
using NexusServer.Data;
using Microsoft.Extensions.Logging;

namespace NexusServer.Controllers
{
    [Route("")]
    [ApiController]
    [Authorize] // Basic authentication
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationController> _logger;

        private IHelper _helper;
        public AuthenticationController(IUserRepository userRepository, IHelper helper, ILogger<AuthenticationController> logger)
        {
            _userRepository = userRepository;
            _helper = helper;
            _logger = logger;
        }

         
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.name) || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.pwd))
                {
                    _logger.LogInformation("Invalid input. Name, email, and password are required.");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 100,
                        errormessage = "Invalid Inputs"
                    };
                    return StatusCode(400, errorBody);

                }
                if (!_helper.IsValidEmail(request.email))
                {
                    _logger.LogInformation("Invalid email.");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 100,
                        errormessage = "Invalid Inputs"
                    };
                    return StatusCode(400, errorBody);
                }

                // Check if the user already exists
                var existingUser = _userRepository.GetUserByEmail(request.email);
                if (existingUser != null)
                {
                    _logger.LogInformation("User with this email already exists.");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 409,
                        errorcode = 106,
                        errormessage = "User already exists"
                    };
                    return StatusCode(400, errorBody);
                }
                string hashedPassword = _helper.HashPassword(request.pwd);
                // Create a new user
                var user = new User
                {
                    name = request.name,
                    email = request.email,
                    pwdHash = hashedPassword,
                    //sessionToken = _helper.GenerateSessionToken()
                };

                // Save the user to the database
                _userRepository.CreateUser(user);

                // Return a success response 
                var response = new SignUpResponse
                {
                    message = "User registration successful",
                };

                return CreatedAtAction(nameof(SignUp), response); // 201 Created
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during SignUp: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }

        }


         
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                // Validate the request body
                if (request == null || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.pwd))
                {
                    _logger.LogInformation("Invalid request body.");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 100,
                        errormessage = "Invalid Inputs"
                    };
                    return StatusCode(400, errorBody);
                }
                var user = await _userRepository.AuthenticateUserAsync(request.email, request.pwd);

                if (user == null)
                {
                    _logger.LogInformation("Invalid email or password.");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 401,
                        errorcode = 105,
                        errormessage = "Unauthorised user"
                    };
                    return StatusCode(400, errorBody);
                }
                var sessionToken = _helper.GenerateSessionToken();
                _userRepository.SaveSessionToken(user.email, sessionToken);

                var responseModel = new SignInResponse
                {
                    id = user.id,
                    email = user.email,
                    name = user.name,
                    sessionToken = sessionToken
                };

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during SignIn: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }

        }

         
        [HttpPost("signout")]
        public async Task<IActionResult> SignOut([FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                _userRepository.UpdateNullSessionToken(sessionToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during SignOut: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }
        }
    }
}