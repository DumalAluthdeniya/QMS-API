using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using QMS_API.Data;
using QMS_API.Models;
using QMS_API.Resources;

namespace QMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("attempt")]
        
        public async Task<IActionResult> AddQuizAttemptAsync([FromBody] QuizAttemptResource quizAttempt)
        {
            var currentAttempt = await _context.QuizAttempts.FirstOrDefaultAsync(a => a.Email == quizAttempt.Email);

            if (currentAttempt != null)
            {
                return Ok(currentAttempt.Id);
            }

            var attempt = new QuizAttempt()
            {
                Name = quizAttempt.Name,
                Email = quizAttempt.Email,
                Enrollment = quizAttempt.Enrollment,
                Link = await _context.Links.FirstOrDefaultAsync(l => l.Id == quizAttempt.LinkId)
            };

            await _context.QuizAttempts.AddAsync(attempt);
            await _context.SaveChangesAsync();

            return Ok(attempt.Id);
        }

        [HttpPost("answer")]
       
        public async Task<IActionResult> AddQuizAnswer([FromBody] QuizAnswerResource quizAnswer)
        {

            var newAnswer = new QuizAnswer()
            {
                QuizAttempt = await _context.QuizAttempts.FirstOrDefaultAsync(l => l.Id == quizAnswer.QuizAttemptId),
                Test = await _context.Tests.FirstOrDefaultAsync(l => l.Id == quizAnswer.TestId),
                Question = await _context.Questions.FirstOrDefaultAsync(l => l.Id == quizAnswer.QuestionId),
                Answer = quizAnswer.Answer,
                MatchingText = quizAnswer.MatchingText
            };

           
            if (newAnswer.Question.QuestionType.Equals(3))
            {
                var existingAnswer = await _context.QuizAnswers.Where(a =>
                   a.QuizAttempt == newAnswer.QuizAttempt && a.Test == newAnswer.Test && a.Question == newAnswer.Question && a.Answer == newAnswer.Answer).FirstOrDefaultAsync();

                if (existingAnswer != null)
                {
                    existingAnswer.MatchingText = newAnswer.MatchingText;
                }
                else
                {
                    await _context.QuizAnswers.AddAsync(newAnswer);
                }
            }
            else
            {
               var existingAnswers = await _context.QuizAnswers.Where(a =>
                    a.QuizAttempt == newAnswer.QuizAttempt && a.Test == newAnswer.Test && a.Question == newAnswer.Question).ToListAsync();

                _context.QuizAnswers.RemoveRange(existingAnswers);

                await _context.QuizAnswers.AddAsync(newAnswer);
            }

            await _context.SaveChangesAsync();

            return Ok(true);
        }
    }
}
