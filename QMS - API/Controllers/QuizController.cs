using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QMS_API.Data;
using QMS_API.Models;
using QMS_API.Resources;
using QMS_API.Utils;
using static QMS_API.Enums.Enums;

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
            var currentAttempt = await _context.QuizAttempts.Include(qa => qa.Link).FirstOrDefaultAsync(a => a.Email == quizAttempt.Email && a.Link.Id == quizAttempt.LinkId);

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
            try
            {


                var newAnswer = new QuizAnswer()
                {
                    QuizAttempt = await _context.QuizAttempts.Include(qa => qa.QuizAnswers)
                        .FirstOrDefaultAsync(l => l.Id == quizAnswer.QuizAttemptId),
                    Test = await _context.Tests.Include(t => t.TestQuestions)
                        .FirstOrDefaultAsync(l => l.Id == quizAnswer.TestId),
                    Question = await _context.Questions.Include(q => q.Answers)
                        .FirstOrDefaultAsync(l => l.Id == quizAnswer.QuestionId),
                    Answer = quizAnswer.Answer,
                    MatchingText = quizAnswer.MatchingText
                };


                if (newAnswer.Question.QuestionType.Equals(QuestionTypes.Matching))
                {
                    var existingAnswer = await _context.QuizAnswers.Where(a =>
                        a.QuizAttempt == newAnswer.QuizAttempt && a.Test == newAnswer.Test &&
                        a.Question == newAnswer.Question && a.Answer == newAnswer.Answer &&
                        a.QuizAttempt == newAnswer.QuizAttempt).FirstOrDefaultAsync();

                    var hasAnsweredBefore =
                        newAnswer.QuizAttempt.QuizAnswers.Any(qa => qa.Question == newAnswer.Question);

                    newAnswer.QuizAttempt.QuizAnswers?.FindAll(qa =>
                        !qa.IsAnswerCorrect && qa.Question == newAnswer.Question);

                    newAnswer.IsAnswerCorrect =
                        newAnswer.Question.Answers.Find(a => a.Name == newAnswer.Answer)?.MatchingText ==
                        newAnswer.MatchingText;

                    if (existingAnswer != null)
                    {
                        existingAnswer.MatchingText = newAnswer.MatchingText;
                        existingAnswer.IsAnswerCorrect = newAnswer.IsAnswerCorrect;
                    }
                    else
                    {
                        await _context.QuizAnswers.AddAsync(newAnswer);
                    }

                    var hasAnsweredAll =
                        newAnswer.QuizAttempt.QuizAnswers.FindAll(qa => qa.Question == newAnswer.Question).Count ==
                        newAnswer.Question.Answers.Count;


                    var incorrectAnswerCountAfterUpdate = newAnswer.QuizAttempt.QuizAnswers.FindAll(qa =>
                        !qa.IsAnswerCorrect && qa.Question == newAnswer.Question &&
                        qa.QuizAttempt == newAnswer.QuizAttempt);

                    if (incorrectAnswerCountAfterUpdate.Count > 0 && hasAnsweredBefore && hasAnsweredAll)
                    {
                        newAnswer.QuizAttempt.Score -= newAnswer.Question.Points;
                        newAnswer.QuizAttempt.CorrectQuestions--;
                    }
                    else if (incorrectAnswerCountAfterUpdate.Count == 0 && hasAnsweredAll)
                    {
                        newAnswer.QuizAttempt.Score += newAnswer.Question.Points;
                        newAnswer.QuizAttempt.CorrectQuestions++;
                    }


                    //calculate percentage
                    newAnswer.QuizAttempt.Percentage = (int) (0.5f + ((100f * newAnswer.QuizAttempt.CorrectQuestions) /
                                                                      newAnswer.Test.TestQuestions.Count()));

                    await _context.SaveChangesAsync();


                }
                else
                {
                    var existingAnswers = await _context.QuizAnswers.Where(a =>
                        a.QuizAttempt == newAnswer.QuizAttempt && a.Test == newAnswer.Test &&
                        a.Question == newAnswer.Question && a.QuizAttempt == newAnswer.QuizAttempt).ToListAsync();


                    var correctAnswerCountBeforeUpdate =
                        newAnswer.QuizAttempt.QuizAnswers.FindAll(qa =>
                            qa.IsAnswerCorrect && qa.Question == newAnswer.Question);

                    _context.QuizAnswers.RemoveRange(existingAnswers);

                    newAnswer.IsAnswerCorrect = newAnswer.Question.Answers.Find(a => a.IsCorrectAnswer)?.Name ==
                                                newAnswer.Answer;


                    await _context.QuizAnswers.AddAsync(newAnswer);


                    var correctAnswerCountAfterUpdate = newAnswer.QuizAttempt.QuizAnswers.FindAll(qa =>
                        qa.IsAnswerCorrect && qa.Question == newAnswer.Question &&
                        qa.QuizAttempt == newAnswer.QuizAttempt);
                    if (correctAnswerCountBeforeUpdate.Count > 0)
                    {
                        newAnswer.QuizAttempt.Score -= newAnswer.Question.Points;
                        newAnswer.QuizAttempt.CorrectQuestions--;
                    }
                    else if (correctAnswerCountAfterUpdate.Count > 0)
                    {
                        newAnswer.QuizAttempt.Score += newAnswer.Question.Points;
                        newAnswer.QuizAttempt.CorrectQuestions++;
                    }


                    //calculate percentage
                    newAnswer.QuizAttempt.Percentage = (int) (0.5f + ((100f * newAnswer.QuizAttempt.CorrectQuestions) /
                                                                      newAnswer.Test.TestQuestions.Count()));

                    await _context.SaveChangesAsync();

                }

                return Ok(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmitResource submitResource)
        {
            try
            {
                var attempt =
                    await _context.QuizAttempts.FirstOrDefaultAsync(qa => qa.Id == submitResource.QuizAttemptId);
                if (attempt == null)
                    return NotFound();
                attempt.FinishDate = submitResource.FinishTime;
                var duration = submitResource.FinishTime - attempt.StartDate;
                attempt.Duration = new Time()
                    {Hours = duration.Hours, Minutes = duration.Minutes, Seconds = duration.Seconds}.ToString();
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,e);
            }
        }
    }
}
