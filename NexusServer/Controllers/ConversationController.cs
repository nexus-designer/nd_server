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
using Microsoft.EntityFrameworkCore;


namespace NexusServer.Controllers
{
    [Route("conversation")]
    [ApiController]

    public class ConversationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private IHelper _helper;
        private readonly ILogger<AuthenticationController> _logger;
        public ConversationController(IUserRepository userRepository, IHelper helper, ILogger<AuthenticationController> logger)
        {
            _userRepository = userRepository;
            _helper = helper;
            _logger = logger;
        }
        
        
        [HttpPost("")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request,[FromHeaderAttribute(Name ="sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if(user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400,errorBody);
                }   
                if (string.IsNullOrEmpty(request.title))
                {
                    _logger.LogInformation("Title is required");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 100,
                        errormessage = "Invalid Inputs"
                    };
                    return StatusCode(400, errorBody);
                }
                var conversation = new Conversation
                {
                    userId = user.id,
                    title = request.title,
                    waiting = false,
                };

                _userRepository.CreateConversation(conversation);

                var response = new ConversationResponse
                {
                    id = conversation.id,
                    userId = conversation.userId,
                    title = conversation.title,
                    waiting = conversation.waiting,
                    createdOn = conversation.createdOn
                };

                return CreatedAtAction(nameof(CreateConversation), response); // 201 Created
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during CreateConversation: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }

        }

         
        [HttpGet("")]
        public async Task<IActionResult> Conversations([FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var conversation = _userRepository.GetAllConversations(user.id);
                if(conversation == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }
                _logger.LogInformation("Conversations listed successfully");
                return Ok(conversation);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during Conversations: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }

        }

         
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversationDetails(int id, [FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try 
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var conversation = _userRepository.GetConversationById(id,user.id);

                if (conversation == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var response = new Conversation
                {
                    id = conversation.id,
                    userId = conversation.userId,
                    title = conversation.title,
                    waiting = conversation.waiting,
                    createdOn = conversation.createdOn
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during GetConversationDetails: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }
        }
        
        
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateConversationDetails(int id, [FromBody] ConversationUpdateRequest request, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var conversation = _userRepository.UpdateConversation(request.title, id,user.id);
                if (conversation == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var response = new Conversation
                {
                    id = conversation.id,
                    userId = conversation.userId,
                    title = conversation.title,
                    waiting = conversation.waiting,
                    createdOn = conversation.createdOn
                };

                return Ok(response);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred during UpdateConversationDetails: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }

        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversationDetails(int id, [FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var deleteconversation = _userRepository.DeleteConversation(id,user.id);
                if (deleteconversation == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var response = new Conversation
                {
                    id = deleteconversation.id,
                    userId = deleteconversation.userId,
                    title = deleteconversation.title,
                    waiting = deleteconversation.waiting,
                    createdOn = deleteconversation.createdOn
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during DeleteConversationDetails: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }

        }

        
        [HttpGet("{id}/msg")]
        public async Task<IActionResult> GetMessagesForConversation(int id, [FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                if (_userRepository.GetConversationById(id,user.id) == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var message = _userRepository.GetMessageByCoversationId(id);
                if(message == null)
                {
                    _logger.LogInformation("Message not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 103,
                        errormessage = "Message not found"
                    };
                    return StatusCode(400, errorBody);
  
                }   
                return Ok(new { msgs = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during GetMessagesForConversation: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }
        }

        
        [HttpGet("{id}/msg/{index}")]
        public async Task<IActionResult> ReadingConversationMessages(int id,int index, [FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                if (_userRepository.GetConversationById(id,user.id) == null || _userRepository.GetMessageByCoversationId(id) ==null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }

                var message = _userRepository.ReadingMessageByIndex(index,id);
                if(message == null)
                {
                    _logger.LogInformation("Message not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 103,
                        errormessage = "Message not found"
                    };
                    return StatusCode(400, errorBody);
                }


                return Ok(new { msgs = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during ReadingConversationMessages: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }
        }

        
        [HttpPost("{id}/msg")]
        public IActionResult AppendingMessages(int id, AppendMessageRequest request, [FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                if (_userRepository.GetConversationById(id,user.id) == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }

                var msg = _userRepository.AppendMessage(id,request.content);

                return Ok(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during AppendingMessages: {ErrorMessage}", ex.Message);
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }
        }

        
        [HttpPost("{id}/msg/{index}")]
        public IActionResult UpdateMessages(int id,int index,AppendMessageRequest request, [FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                if (_userRepository.GetConversationById(id, user.id) == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var msg = _userRepository.UpdateMessage(id,index, request.content);
                if(msg == null)
                {
                    _logger.LogInformation("Message not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 103,
                        errormessage = "Message not found"
                    };
                    return StatusCode(400, errorBody);
                }
                return Ok(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during UpdateMessages: {ErrorMessage}", ex.Message);    
                var errorBody = new ErrorBody
                {
                    statuscode = 500,
                    errorcode = 107,
                    errormessage = "Contact System Admin for more information"
                };
                return StatusCode(500, errorBody);
            }
        }
        
        [HttpDelete("{id}/msg/{index}")]
        public async Task<IActionResult> DeleteMessageDetails(int id,int index, [FromHeader(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 101,
                        errormessage = "User not found"
                    };
                    return StatusCode(400, errorBody);
                }
                var conversation = _userRepository.GetConversationById(id,user.id);
                if (conversation == null)
                {
                    _logger.LogInformation("Conversation not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 102,
                        errormessage = "Conversation not found"
                    };
                    return StatusCode(400, errorBody);
                }
                if (conversation.waiting == true)
                {
                    _logger.LogInformation("Conversation is waiting");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 104,
                        errormessage = "Conversation is waiting"
                    };
                    return StatusCode(400, errorBody);

                }
                var deletemsg = _userRepository.DeleteMessage(id,index);
                if(deletemsg == null)
                {
                    _logger.LogInformation("Message not found");
                    var errorBody = new ErrorBody
                    {
                        statuscode = 400,
                        errorcode = 103,
                        errormessage = "Message not found"
                    };
                    return StatusCode(400, errorBody);
                }

                var response = new Msg
                {
                    id = deletemsg.id,
                    conversationId = deletemsg.conversationId,
                    index = deletemsg.index,
                    fromBot = deletemsg.fromBot,
                    content = deletemsg.content,
                    media = deletemsg.media

                };

                return Ok(response);
            }
            catch(Exception ex)            
            {
                _logger.LogError(ex, "An error occurred during DeleteMessageDetails: {ErrorMessage}", ex.Message);
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