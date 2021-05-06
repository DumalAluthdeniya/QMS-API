using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QMS_API.Data;
using QMS_API.Models;
using QMS_API.Resources;
using QMS_API.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public TestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/<TestController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tests = await _context.Tests.Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                .ThenInclude(q => q.Answers)
                .Include(t => t.Links)
                .ThenInclude(l => l.QuizAttempts)
                .ThenInclude(qa => qa.QuizAnswers)
                .Include(t => t.CreatedBy)
                .Where(t => !t.IsDeleted)
                .ToListAsync();

            var testResources = new List<TestResource>();

            tests.ForEach(test =>
            {

                var questionResources = new List<QuestionResource>();
                test.TestQuestions.Select(tq => tq.Question).ToList().ForEach(q =>
                {
                    var answerResources = new List<AnswerResource>();
                    q.Answers.ForEach(a =>
                    {
                        var answerResource = new AnswerResource()
                        {
                            Id = a.Id,
                            IsCorrectAnswer = a.IsCorrectAnswer,
                            Name = a.Name,
                            MatchingText = a.MatchingText
                        };

                        answerResources.Add(answerResource);
                    });

                    var questionResource = new QuestionResource()
                    {
                        Id = q.Id,
                        Title = q.Title,
                        QuestionType = q.QuestionType,
                        AnswerMaxLength = q.AnswerMaxLength,
                        DifficultyLevel = q.DifficultyLevel,
                        Points = q.Points,
                        RandomizeAnswers = q.RandomizeAnswers,
                        Answers = answerResources
                    };
                    questionResources.Add(questionResource);
                });


                var testResource = new TestResource();

                decimal totalScore = 0;
                double totalDuration = 0;

                var linkResources = new List<LinkResource>();

                test.Links.ForEach(l =>
                {
                    l.QuizAttempts.ForEach(qa =>
                    {
                        totalScore += qa.Score;
                        totalDuration += (qa.FinishDate - qa.FinishDate).Ticks;
                    });

                    var linkResource = new LinkResource()
                    {
                        Id = l.Id,
                        Name = l.Name
                    };
                    linkResources.Add(linkResource);

                });

                decimal average = 0;
                double averageDuration = 0;

                var totalQuizAttempts = test.Links.Sum(l => l.QuizAttempts.Count);
                if (totalQuizAttempts > 0)
                {
                    average = totalScore / totalQuizAttempts;
                    averageDuration = totalDuration / totalQuizAttempts;
                }

                var newAverage = new DateTime((long)averageDuration);
                var dd = new Time()
                {
                    Hours = newAverage.Hour,
                    Minutes = newAverage.Minute,
                    Seconds = newAverage.Second,
                }.ToString();

                testResource.Id = test.Id;
                testResource.Name = test.Name;
                testResource.Introduction = test.Introduction;
                testResource.User = test.CreatedBy.UserName;
                testResource.Questions = test.TestQuestions.Select(q => q.Question.Id).ToList();
                testResource.QuestionsList = questionResources;
                testResource.NoOfStudents = test.Links.Sum(l => l.QuizAttempts.Count);
                testResource.NoOfQuestions = test.TestQuestions.Count();
                testResource.AverageMark = average;
                testResource.AverageDuration = dd;
                testResource.Links = linkResources;

                testResources.Add(testResource);
            });

            return Ok(testResources);
        }

        // GET api/<TestController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var test = await _context.Tests
                .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                .ThenInclude(q => q.Answers)
                .Include(t => t.Links)
                .Include(t => t.CreatedBy)
                .FirstOrDefaultAsync(t => t.Id == id);

            //var links = await _context.Links.Include(l => l.Test).Where(l => l.Test.Id == id).ToListAsync();

            var questionResources = new List<QuestionResource>();

            test.TestQuestions.Select(tq => tq.Question).ToList().ForEach(q =>
            {
                var answerResources = new List<AnswerResource>();
                q.Answers.ForEach(a =>
                {
                    var answerResource = new AnswerResource()
                    {
                        Id = a.Id,
                        IsCorrectAnswer = a.IsCorrectAnswer,
                        Name = a.Name,
                        MatchingText = a.MatchingText
                    };

                    answerResources.Add(answerResource);
                });

                var questionResource = new QuestionResource()
                {
                    Id = q.Id,
                    Title = q.Title,
                    QuestionType = q.QuestionType,
                    AnswerMaxLength = q.AnswerMaxLength,
                    DifficultyLevel = q.DifficultyLevel,
                    Points = q.Points,
                    RandomizeAnswers = q.RandomizeAnswers,
                    Answers = answerResources
                };
                questionResources.Add(questionResource);
            });

            var linkResources = new List<LinkResource>();
            test.Links.ForEach(l =>
            {
                var linkResource = new LinkResource()
                {
                    Id = l.Id,
                    Name = l.Name

                };

                linkResources.Add(linkResource);
            });


            var testResource = new TestResource()
            {
                Id = test.Id,
                Name = test.Name,
                Introduction = test.Introduction,
                User = test.CreatedBy.UserName,
                Questions = test.TestQuestions.Select(q => q.Question.Id).ToList(),
                QuestionsList = questionResources,
                Links = linkResources
            };

            return Ok(testResource);
        }

        // POST api/<TestController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TestResource testResource)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(testResource.User);

                var test = new Test()
                {
                    Name = testResource.Name,
                    Introduction = testResource.Introduction,
                    CreatedBy = user
                };

                var testQuestions = new List<TestQuestion>();
                testResource.Questions.ForEach(questionId =>
                {
                    var testQuestion = new TestQuestion()
                    {
                        Question = _context.Questions.FirstOrDefault(q => q.Id == questionId),
                        Test = test
                    };

                    testQuestions.Add(testQuestion);
                });

                test.TestQuestions = testQuestions;

                await _context.Tests.AddAsync(test);
                await _context.SaveChangesAsync();

                return Ok(test.Id);
            }
            catch (Exception ex)
            {

                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // PUT api/<TestController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TestResource testResource)
        {
            try
            {
                var test = await _context.Tests.Include(t => t.TestQuestions)
                    .ThenInclude(tq => tq.Question)
                    .Include(t => t.Links)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (test == null)
                {
                    return NotFound("Invalid test id.");
                }

                test.Name = testResource.Name;
                test.Introduction = testResource.Introduction;

                _context.TestQuestions.RemoveRange(test.TestQuestions);

                var testQuestions = new List<TestQuestion>();
                testResource.Questions.ForEach(questionId =>
                {
                    var testQuestion = new TestQuestion()
                    {
                        Question = _context.Questions.FirstOrDefault(q => q.Id == questionId),
                        Test = test
                    };

                    testQuestions.Add(testQuestion);
                });

                test.TestQuestions = testQuestions;

                await _context.SaveChangesAsync();

                return Ok(test.Id);

            }
            catch (Exception ex)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE api/<TestController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                return NotFound("Invalid test id.");
            }

            test.IsDeleted = true;

            await _context.SaveChangesAsync();
            return Ok(id);
        }
    }
}
