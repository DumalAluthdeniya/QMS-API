using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                currentAttempt.StartDate = DateTime.Now;
                await _context.SaveChangesAsync();
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
                    Test = await _context.Tests.Include(t => t.TestQuestions).ThenInclude(tq => tq.Question)
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

                    var correctAnswerCountBeforeUpdate = newAnswer.QuizAttempt.QuizAnswers.FindAll(qa =>
                        qa.IsAnswerCorrect && qa.Question == newAnswer.Question &&
                        qa.QuizAttempt == newAnswer.QuizAttempt);

                    if (correctAnswerCountBeforeUpdate.Count > 0)
                    {
                        var correctAnswers = correctAnswerCountBeforeUpdate.Count;
                        var totalAnswers = newAnswer.Question.Answers.Count;
                        decimal score = (decimal)correctAnswers / totalAnswers;
                        score *= newAnswer.Question.Points;
                        newAnswer.QuizAttempt.Score -= score;
                    }

                    var hasAnsweredAll =
                        newAnswer.QuizAttempt.QuizAnswers.FindAll(qa => qa.Question == newAnswer.Question).Count ==
                        newAnswer.Question.Answers.Count;

                    var hasAnsweredBefore =
                        newAnswer.QuizAttempt.QuizAnswers.Any(qa => qa.Question == newAnswer.Question);

                    newAnswer.QuizAttempt.QuizAnswers?.FindAll(qa =>
                        !qa.IsAnswerCorrect && qa.Question == newAnswer.Question);

                    var hasGivenCorrectAnswerBefore = false;

                    if (hasAnsweredAll)
                    {
                        hasGivenCorrectAnswerBefore = newAnswer.QuizAttempt.QuizAnswers.FindAll(qa =>
                            !qa.IsAnswerCorrect && qa.Question == newAnswer.Question &&
                            qa.QuizAttempt == newAnswer.QuizAttempt).Count == 0;
                    }

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
                        hasAnsweredAll =
                            newAnswer.QuizAttempt.QuizAnswers.FindAll(qa => qa.Question == newAnswer.Question).Count ==
                            newAnswer.Question.Answers.Count;
                    }


                    var correctAnswerCountAfterUpdate = newAnswer.QuizAttempt.QuizAnswers.FindAll(qa =>
                        qa.IsAnswerCorrect && qa.Question == newAnswer.Question &&
                        qa.QuizAttempt == newAnswer.QuizAttempt);

                    if (correctAnswerCountAfterUpdate.Count > 0)
                    {

                        var correctAnswers = correctAnswerCountAfterUpdate.Count;
                        var totalAnswers = newAnswer.Question.Answers.Count;
                        decimal score = (decimal)correctAnswers / totalAnswers;
                        score = score * newAnswer.Question.Points;
                        newAnswer.QuizAttempt.Score += score;
                        if (correctAnswerCountAfterUpdate.Count == totalAnswers)
                            newAnswer.QuizAttempt.CorrectQuestions++;

                    }
                    else if (hasAnsweredAll && correctAnswerCountAfterUpdate.Count == 0 && correctAnswerCountBeforeUpdate.Count > 0)
                    {

                        newAnswer.QuizAttempt.CorrectQuestions--;
                    }


                    //calculate percentage
                    var totalPoints = newAnswer.Test.TestQuestions.Sum(q => q.Question.Points);
                    var questionscore = newAnswer.QuizAttempt.Score;
                    newAnswer.QuizAttempt.Percentage = (int)(questionscore * 100 / (decimal)totalPoints);

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
                    if (correctAnswerCountBeforeUpdate.Count > 0 && !newAnswer.Question.QuestionType.Equals(QuestionTypes.FreeText))
                    {
                        newAnswer.QuizAttempt.Score -= newAnswer.Question.Points;
                        newAnswer.QuizAttempt.CorrectQuestions--;
                    }
                    else if (correctAnswerCountAfterUpdate.Count > 0 && !newAnswer.Question.QuestionType.Equals(QuestionTypes.FreeText))
                    {
                        newAnswer.QuizAttempt.Score += newAnswer.Question.Points;
                        newAnswer.QuizAttempt.CorrectQuestions++;
                    }


                    //calculate percentage
                    //calculate percentage
                    var totalPoints = newAnswer.Test.TestQuestions.Sum(q => q.Question.Points);
                    var questionscore = newAnswer.QuizAttempt.Score;
                    newAnswer.QuizAttempt.Percentage = (int)(questionscore * 100 / (decimal)totalPoints);

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


        [HttpPost("duration")]

        public async Task<IActionResult> AddQuizAnswerDuration([FromBody] QuizAnswerResource quizAnswer)
        {
            var answer = await _context.QuizAnswers
                .Include(qa => qa.QuizAttempt)
                .Include(qa => qa.Test)
                .Include(qa => qa.Question)
                .FirstOrDefaultAsync(q =>
                    q.QuizAttempt.Id == quizAnswer.QuizAttemptId && q.Question.Id == quizAnswer.QuestionId &&
                    q.Test.Id == quizAnswer.TestId);
            if (answer == null)
            {
                return NotFound();
            }

            answer.Duration = quizAnswer.Duration;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmitResource submitResource)
        {
            try
            {
                var attempt =
                    await _context.QuizAttempts
                    .Include(qq => qq.Link)
                    .ThenInclude(l => l.Test)
                    .ThenInclude(l => l.TestQuestions)
                    .ThenInclude(ll => ll.Question)
                    .ThenInclude(a => a.Answers)
                    .Include(qa => qa.QuizAnswers)
                    .ThenInclude(a => a.Question)
                    .FirstOrDefaultAsync(qa => qa.Id == submitResource.QuizAttemptId);
                if (attempt == null)
                    return NotFound();
                attempt.FinishDate = DateTime.Now;
                var duration = DateTime.Now - attempt.StartDate;
                attempt.Duration = new Time()
                { Hours = duration.Hours, Minutes = duration.Minutes, Seconds = duration.Seconds }.ToString();
                attempt.Submitted = true;


                var summery = new ResultsSummery() { SummeryTexts = new System.Collections.Generic.List<string>() };

                summery.TotalQuestions = attempt.Link.Test.TestQuestions.Count();
                summery.TotalMark = attempt.Link.Test.TestQuestions.Sum(q => q.Question.Points);
               
                summery.Duration = attempt.Duration;
                summery.StartTime = attempt.StartDate;
                summery.FinishedTime = attempt.FinishDate;


                foreach (var q in attempt.Link.Test.TestQuestions)
                {

                    string text = string.Format("{0} ({1}): {2} out of {3}", q.Question.Title, q.Question.QuestionType, GetScoreAsync(q.Question, attempt).ToString(), q.Question.Points);
                    summery.SummeryTexts.Add(text);

                }

                summery.CorrectAnswers = attempt.CorrectQuestions;
                summery.IncorrectAnswers = attempt.Link.Test.TestQuestions.Count() - attempt.CorrectQuestions;
                summery.MarksObtained = attempt.Score;
                await _context.SaveChangesAsync();

                return Ok(summery);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        private decimal GetScoreAsync(Question q, QuizAttempt qa)
        {
            if (q.QuestionType == Enums.Enums.QuestionTypes.Matching)
            {
                var correctAnswers = qa.QuizAnswers.FindAll(qa =>
                        qa.IsAnswerCorrect && qa.Question == q).Count;

                var totalAnswers = q.Answers.Count();
                decimal score = (decimal)correctAnswers / totalAnswers;
                return score *= q.Points;
            }
            else if (q.QuestionType == Enums.Enums.QuestionTypes.FreeText)
            {
                var givenAnswer = qa.QuizAnswers.Find(qan => qan.Question == q);
                decimal score = 0;
                if (givenAnswer != null)
                {
                    var url = string.Format("http://tidal-geode-315120.et.r.appspot.com/send_message/{0}/{1}", givenAnswer.Answer,
                        givenAnswer.MatchingText);
                    //var response = client.GetAsync(url).GetAwaiter().GetResult();

                    using (var client = new HttpClient())
                    {
                        var response = client.GetAsync(url).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var responseContent = response.Content;

                            // by calling .Result you are synchronously reading the result
                            string responseString = responseContent.ReadAsStringAsync().Result;

                            var details = JObject.Parse(responseString);
                            var similarity = (decimal)details["Similarity"];

                            if (similarity >= (decimal)0.5)
                            {
                                qa.CorrectQuestions++;
                                qa.QuizAnswers.Find(qq => qq.Question.Id == q.Id).IsAnswerCorrect = true;
                            }
                            else
                            {
                                qa.QuizAnswers.Find(qq => qq.Question.Id == q.Id).IsAnswerCorrect = false;
                            }
                            score = similarity * q.Points;
                            qa.Score += score;
                        }
                    }

                  
                }

                return score;

            }
            else
            {
                var correctAnswers = qa.QuizAnswers.FindAll(qa =>
                       qa.IsAnswerCorrect && qa.Question == q).Count;
                if (correctAnswers > 0)
                    return q.Points;

                return 0;
            }

        }
    }
}
