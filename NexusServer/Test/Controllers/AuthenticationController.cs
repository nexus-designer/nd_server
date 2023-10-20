using NexusServer.Interfaces;
using NexusServer.Model;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;


[TestFixture]
public class AuthenticationControllerTests : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private IHelper _helper;
    public AuthenticationControllerTests(IUserRepository userRepository, IHelper helper)
    {
        _userRepository = userRepository;
        _helper = helper;
    }

    [Test]
    public async Task SignUp_ValidRequest_ReturnsCreated()
    {

    }
}
