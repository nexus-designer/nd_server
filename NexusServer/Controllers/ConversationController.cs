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
        public ConversationController(IUserRepository userRepository, IHelper helper)
        {
            _userRepository = userRepository;
            _helper = helper;
        }
        
        
        [HttpPost("")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request,[FromHeaderAttribute(Name ="sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if(user == null)
                {
                    return StatusCode(400,"User not found");
                }   
                if (string.IsNullOrEmpty(request.title))
                {
                    return BadRequest("Title is required.");
                }
                var conversation = new Conversation
                {
                    userId = user.id,
                    title = request.title,
                    waiting = false,
                };
                // Save the user to the database
                _userRepository.CreateConversation(conversation);
                // Return a success response 
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
                // Handle exceptions as needed
                return StatusCode(500,ex.Message);
            }

        }

         
        [HttpGet("")]
        public async Task<IActionResult> Conversations([FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                var conversation = _userRepository.GetAllConversations(user.id);
                if(conversation == null)
                {
                    return StatusCode(400, "Conversation not found");
                }
                return Ok(conversation); // 201 Created
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

         
        [HttpGet("{id}")]
        public async Task<IActionResult> GetConversationDetails(int id, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try 
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                // Call your repository or service to fetch conversation details by id
                var conversation = _userRepository.GetConversationById(id,user.id);

                if (conversation == null)
                {
                    return StatusCode(400, "Conversation not found"); // Return a 404 Not Found response if the conversation is not found
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
                return StatusCode(500, ex.Message);
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
                    return StatusCode(400, "User not found");
                }
                var conversation = _userRepository.UpdateConversation(request.title, id,user.id);
                if (conversation == null)
                {
                    return StatusCode(400, "Conversation not found");
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
                return StatusCode(500, ex.Message);
            }

        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConversationDetails(int id, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                var deleteconversation = _userRepository.DeleteConversation(id,user.id);
                if (deleteconversation == null)
                {
                    return StatusCode(400, "Conversation not found");
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
                return StatusCode(500, ex.Message);
            }

        }

        
        [HttpGet("{id}/msg")]
        public async Task<IActionResult> GetMessagesForConversation(int id, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                if (_userRepository.GetConversationById(id,user.id) == null)
                {
                    return StatusCode(400, "Conversation not found");
                }
                var message = _userRepository.GetMessageByCoversationId(id);
                if(message == null)
                {
                    return StatusCode(400, "Message not found");
                }   
                return Ok(new { msgs = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        [HttpGet("{id}/msg/{index}")]
        public async Task<IActionResult> ReadingConversationMessages(int id,int index, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                if (_userRepository.GetConversationById(id,user.id) == null || _userRepository.GetMessageByCoversationId(id) ==null)
                {
                    return NotFound();
                }

                var message = _userRepository.ReadingMessageByIndex(index,id);
                if(message == null)
                {
                    return StatusCode(400, "Message not found.");
                }


                return Ok(new { msgs = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        [HttpPost("{id}/msg")]
        public IActionResult AppendingMessages(int id, AppendMessageRequest request, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                if (_userRepository.GetConversationById(id,user.id) == null)
                {
                    return NotFound();
                }

                var msg = _userRepository.AppendMessage(id,request.content);

                return Ok(msg);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        
        [HttpPost("{id}/msg/{index}")]
        public IActionResult UpdateMessages(int id,int index,AppendMessageRequest request, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                if (_userRepository.GetConversationById(id, user.id) == null)
                {
                    return NotFound();
                }
                var msg = _userRepository.UpdateMessage(id,index, request.content);
                if(msg == null)
                {
                    return StatusCode(400, "Message not found");
                }
                return Ok(msg);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpDelete("{id}/msg/{index}")]
        public async Task<IActionResult> DeleteMessageDetails(int id,int index, [FromHeaderAttribute(Name = "sessionToken")] string sessionToken)
        {
            try
            {
                var user = _userRepository.GetUserBySessionToken(sessionToken);
                if (user == null)
                {
                    return StatusCode(400, "User not found");
                }
                var conversation = _userRepository.GetConversationById(id,user.id);
                if (conversation == null)
                {
                    return StatusCode(400, "Conversation not found");
                }
                if (conversation.waiting == true)
                {
                    return StatusCode(400, "Conversation is waiting");
                }
                var deletemsg = _userRepository.DeleteMessage(id,index);
                if(deletemsg == null)
                {
                    return StatusCode(400, "Message not found");
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
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}