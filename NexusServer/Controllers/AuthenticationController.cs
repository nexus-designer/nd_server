using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusServer.Interfaces;
using NexusServer.Model;
using NexusServer.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;
using System;

namespace NexusServer.Controllers
{
    [Route("")]
    [ApiController]
    [Authorize] // Basic authentication
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private IHelper _helper;
        public AuthenticationController(IUserRepository userRepository, IHelper helper)
        {
            _userRepository = userRepository;
            _helper = helper;
        }

        [AllowAnonymous] // Allow unauthenticated access for signup
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            // Input validation 
            if (string.IsNullOrEmpty(request.name) || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.pwd))
            {
                return BadRequest("Invalid input. name, email, and password are required.");
            }
            if (!_helper.IsValidEmail(request.email))
            {
                return BadRequest("Invalid email.");
            }
            
            // Check if the user already exists
            var existingUser = _userRepository.GetUserByEmail(request.email);
            if (existingUser != null)
            {
                return Conflict("User with this email already exists.");
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


        [AllowAnonymous] // Allow unauthenticated access for signup
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            // Validate the request body
            if (request == null || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.pwd))
            {
                return BadRequest("Invalid request body.");
            }
            var user = await _userRepository.AuthenticateUserAsync(request.email, request.pwd);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
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

        [AllowAnonymous] // Allow unauthenticated access for signup
        [HttpPost("signout")]
        public void SignOut([FromHeaderAttribute(Name = "sessionToken")] string sessionToken = "7cc56ec46dd8387586efd5e92d6a6256")
        {
            _userRepository.UpdateNullSessionToken(sessionToken);
        }
        

    }
}