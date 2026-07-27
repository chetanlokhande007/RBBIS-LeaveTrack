using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LeaveAttendance.API.Data;
using LeaveAttendance.API.Models;
using LeaveAttendance.API.DTOs;
using LeaveAttendance.API.Services.Interfaces;

namespace LeaveAttendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly LeaveTrackDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(LeaveTrackDbContext context, IEmailService emailService, ILogger<ContactController> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitContactForm([FromBody] ContactMessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 1. Save to Database
                var message = new ContactMessage
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Message = dto.Message,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ContactMessages.Add(message);
                await _context.SaveChangesAsync();

                // 2. Send Email Notification
                var adminEmail = "chetanlokhande4206@gmail.com"; 
                var subject = $"New Contact Request from {dto.Name}";
                var body = $"You have received a new contact request.\n\nName: {dto.Name}\nEmail: {dto.Email}\nMessage:\n{dto.Message}";

                await _emailService.SendEmailAsync(adminEmail, subject, body);

                return Ok(new { message = "Contact message submitted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form submission from {Email}", dto.Email);
                return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." });
            }
        }
    }
}
