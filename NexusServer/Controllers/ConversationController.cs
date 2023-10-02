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
    [Authorize] // Basic authentication
    public class ConversationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private IHelper _helper;
        public ConversationController(IUserRepository userRepository, IHelper helper)
        {
            _userRepository = userRepository;
            _helper = helper;
        }
        
        [AllowAnonymous] // Allow unauthenticated access for signup
        [HttpPost("")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest request,[FromHeaderAttribute(Name ="sessionToken")] string sessionToken = "7cc56ec46dd8387586efd5e92d6a6256")
        {
            var user = _userRepository.GetUserBySessionToken(sessionToken);
            if (string.IsNullOrEmpty(request.title))
            {
                return BadRequest("Title is required.");
            }
            var conversation = new Conversation
            {
                userId = user.id,
                title = request.title,
                waiting = false
            };
            // Save the user to the database
            _userRepository.CreateConversation(conversation);
            // Return a success response 
            var response = new ConversationResponse
            {
                id = conversation.id,
                userId = conversation.userId,
                title = conversation.title,
                waiting = conversation.waiting
            };

            return CreatedAtAction(nameof(CreateConversation), response); // 201 Created
        }

        [AllowAnonymous] // Allow unauthenticated access for signup
        [HttpGet("")]
        public List<Conversation> Conversations([FromHeaderAttribute(Name = "sessionToken")] string sessionToken = "7cc56ec46dd8387586efd5e92d6a6256")
        {
            var conversation = _userRepository.GetAllConversations(sessionToken);
            return conversation; // 201 Created
        }

        [AllowAnonymous] // Allow unauthenticated access for signup
        [HttpGet("{id}")]
        public IActionResult GetConversationDetails(int id)
        {
            // Call your repository or service to fetch conversation details by id
            var conversation = _userRepository.GetConversationById(id);

            if (conversation == null)
            {
                return NotFound(); // Return a 404 Not Found response if the conversation is not found
            }
            var response = new Conversation
            {
                id = conversation.id,
                userId = conversation.userId,
                title = conversation.title,
                waiting = conversation.waiting
            };

            return Ok(response);
        }
        
        [AllowAnonymous]
        [HttpPost("{id}")]
        public Conversation UpdateConversationDetails(int id, [FromBody] ConversationUpdateRequest request)
        {
            
            var conversation = _userRepository.UpdateConversation(request.title,id);
           
            var response = new Conversation
            {
                id = conversation.id,
                userId = conversation.userId,
                title = conversation.title,
                waiting = conversation.waiting
            };

            return response;
        }

        [AllowAnonymous]
        [HttpDelete("{id}")]
        public Conversation DeleteConversationDetails(int id)
        {
            var deleteconversation = _userRepository.DeleteConversation(id);
            var response = new Conversation
            {
                id = deleteconversation.id,
                userId = deleteconversation.userId,
                title = deleteconversation.title,
                waiting = deleteconversation.waiting
            };

            return response;
        }

        [AllowAnonymous]
        [HttpGet("{id}/msg")]
        public IActionResult GetMessagesForConversation(int id)
        {
            try
            {
                if (_userRepository.GetConversationById(id) == null)
                {
                    return NotFound();
                }

                var message = _userRepository.GetMessageByCoversationId(id);

                

                return Ok(new { msgs = message });
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [AllowAnonymous]
        [HttpGet("{id}/msg/{index}")]
        public IActionResult ReadingConversationMessages(int id,int index)
        {
            try
            {
                if (_userRepository.GetConversationById(id) == null || _userRepository.GetMessageByCoversationId(id) ==null)
                {
                    return NotFound();
                }

                var message = _userRepository.ReadingMessageByIndex(index,id);



                return Ok(new { msgs = message });
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        //[AllowAnonymous]
        //[HttpGet("{id}/msg")]
        //public IActionResult AppendingMessages(int id)
        //{
        //    try
        //    {
        //        if (_userRepository.GetConversationById(id) == null)
        //        {
        //            return NotFound();
        //        }

        //        _userRepository.AppendMessage(id);

        //        return Ok(new { msgs = message });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle exceptions as needed
        //        return StatusCode(500, "An error occurred while processing the request.");
        //    }
        //}

        [AllowAnonymous]
        [HttpDelete("{id}/msg/{index}")]
        public IActionResult DeleteMessageDetails(int id,int index)
        {
            try
            {
                var conversation = _userRepository.GetConversationById(id);
                if (conversation.waiting == true)
                {
                    return StatusCode(400, "Conversation is waiting");
                }
                var deletemsg = _userRepository.DeleteMessage(id,index);


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
                return StatusCode(500, "An error occurred while processing the request.");
            }
            
        }
    }
}