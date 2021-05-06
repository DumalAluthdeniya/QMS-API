using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QMS_API.Data;
using QMS_API.Models;
using QMS_API.Resources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LinkController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LinkController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/<QuestionsController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var links = await _context.Links
                .Include(c => c.Test)
                .ThenInclude(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                .ThenInclude(q => q.Answers)
                .Include(l => l.CreatedBy)
                .Include(l => l.QuizAttempts)
                .ThenInclude(qa => qa.QuizAnswers)
                .ToListAsync();


            var linkResources = new List<LinkResource>();

            links.ForEach(link =>
            {
                var questionResources = new List<QuestionResource>();
                link.Test.TestQuestions.Select(tq => tq.Question).ToList().ForEach(q =>
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
                        Topic = q.Topic,
                        QuestionType = q.QuestionType,
                        AnswerMaxLength = q.AnswerMaxLength,
                        DifficultyLevel = q.DifficultyLevel,
                        Points = q.Points,
                        RandomizeAnswers = q.RandomizeAnswers,
                        Answers = answerResources
                    };
                    questionResources.Add(questionResource);
                });

                var quizAttempts = new List<QuizAttemptResource>();

                link.QuizAttempts.ForEach(qa =>
                {
                    var quizAnswers = new List<QuizAnswerResource>();

                    qa.QuizAnswers.ForEach(qan =>
                    {
                        var quizAnswer = new QuizAnswerResource()
                        {
                            Id = qan.Id,
                            QuestionId = qan.Question.Id,
                            Answer = qan.Answer,
                            QuizAttemptId = qan.QuizAttempt.Id,
                            TestId = qan.Id,
                            IsAnswerCorrect = qan.IsAnswerCorrect,
                            MatchingText = qan.MatchingText
                        };

                        quizAnswers.Add(quizAnswer);
                    });


                    var quizAttempt = new QuizAttemptResource()
                    {
                        Id = qa.Id,
                        Email = qa.Email,
                        Enrollment = qa.Enrollment,
                        Name = qa.Name,
                        Score = qa.Score,
                        Duration = qa.Duration,
                        Percentage = qa.Percentage,
                        QuizAnswers = quizAnswers
                    };

                    quizAttempts.Add(quizAttempt);
                });

               

                var linkResource = new LinkResource()
                {
                    Id = link.Id,
                    Name = link.Name,
                    Url = link.Url,
                    Password = link.Password,
                    TimeLimit = link.TimeLimit,
                    Test = new TestResource()
                    {
                        Id = link.Test.Id,
                        Name = link.Test.Name,
                        Introduction = link.Test.Introduction,
                        QuestionsList = questionResources
                    },
                    User = link.CreatedBy.UserName,
                    QuizAttempts = quizAttempts
                    
                    
                };

                linkResources.Add(linkResource);
            });

            return Ok(linkResources);

        }

        [HttpGet("code/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCode(Guid code)
        {
            var link = await _context.Links.Include(l => l.Test).ThenInclude(t => t.TestQuestions).FirstOrDefaultAsync(l => l.Code == code);
            var linkResource = new LinkResource()
            {
                Id = link.Id,
                Name = link.Name,
                Url = link.Url,
                Password = link.Password,
                TimeLimit = link.TimeLimit,
                Code = link.Code,
                TotalQuestions = link.Test.TestQuestions.Count()
            };
            return Ok(linkResource);
        }

        // GET api/<QuestionsController>/code/<guid>
        [HttpGet("code/{code}/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCodeAndEmail(Guid code, string email)
        {
            var link = await _context.Links
         .Include(c => c.Test)
         .ThenInclude(t => t.TestQuestions)
         .ThenInclude(tq => tq.Question)
         .ThenInclude(q => q.Answers)
         .Include(l => l.CreatedBy)
         .Include(l => l.QuizAttempts)
         .ThenInclude(qa => qa.QuizAnswers)
         .ThenInclude(qa => qa.Question)
         .FirstOrDefaultAsync(l => l.Code == code);

            if (link == null)
                return NotFound("Invalid quiz.");

            var quizAnswers = link.QuizAttempts.FirstOrDefault(qa => qa.Email == email)?.QuizAnswers;
            //link.QuizAttempts = link.QuizAttempts.FindAll(qa => qa.Email == email);


            var questionResources = new List<QuestionResource>();

            var questions = link.Test.TestQuestions.Select(tq => tq.Question).ToList();

            foreach(var q in questions)
            {
                var givenAnswerId = -1;

                var givenAnswersForCurrentQuestion = quizAnswers?.Where(qa => qa.Question.Id == q.Id && qa.Test.Id == link.Test.Id).ToList();
                string duration = null;
                if (givenAnswersForCurrentQuestion != null)
                {
                   
                    duration = givenAnswersForCurrentQuestion.Find(a => !string.IsNullOrEmpty(a.Duration))?.Duration;
                }

                var answerResources = new List<AnswerResource>();

                foreach(var a in q.Answers)
                {

                    if (givenAnswersForCurrentQuestion != null && givenAnswersForCurrentQuestion.Any(qa => qa.Answer == a.Name))
                    {
                        givenAnswerId = a.Id;
                        var answerResource = new AnswerResource()
                        {
                            Id = a.Id,
                            IsCorrectAnswer = a.IsCorrectAnswer,
                            Name = a.Name,
                            MatchingText = a.MatchingText,
                            GivenMatchingText = givenAnswersForCurrentQuestion.Find(qa => qa.Answer == a.Name)?.MatchingText,
                            


                        };
                        answerResources.Add(answerResource);
                    }
                    else if (q.QuestionType == Enums.Enums.QuestionTypes.FreeText)
                    {
                        if (givenAnswersForCurrentQuestion != null)
                        {
                            var answerResource = new AnswerResource()
                            {
                                Id = a.Id,
                                IsCorrectAnswer = a.IsCorrectAnswer,
                                Name = a.Name,
                                MatchingText = a.MatchingText,
                                GivenMatchingText = givenAnswersForCurrentQuestion.Find(qa => q.Id == a.Question.Id)?.MatchingText,
                               
                            };
                            answerResources.Add(answerResource);
                        }
                    }
                    else
                    {
                        if (givenAnswersForCurrentQuestion == null) continue;
                        var answerResource = new AnswerResource()
                        {
                            Id = a.Id,
                            IsCorrectAnswer = a.IsCorrectAnswer,
                            Name = a.Name,
                            MatchingText = a.MatchingText,
                            

                        };
                        answerResources.Add(answerResource);
                    }
                    
                }

                var questionResource = new QuestionResource()
                {
                    Id = q.Id,
                    Title = q.Title,
                    QuestionType = q.QuestionType,
                    AnswerMaxLength = q.AnswerMaxLength,
                    DifficultyLevel = q.DifficultyLevel,
                    Points = q.Points,
                    RandomizeAnswers = q.RandomizeAnswers,
                    Answers = answerResources,
                    GivenAnswerId = givenAnswerId,
                    Duration = duration
                };
                questionResources.Add(questionResource);
            }

            var quizAttempts = new List<QuizAttemptResource>();

            link.QuizAttempts.ForEach(qa =>
            {
                var quizAnswers = new List<QuizAnswerResource>();

                qa.QuizAnswers.ForEach(qan =>
                {
                    var quizAnswer = new QuizAnswerResource()
                    {
                        Id = qan.Id,
                        QuestionId = qan.Question.Id,
                        Answer = qan.Answer,
                        QuizAttemptId = qan.QuizAttempt.Id,
                        TestId = qan.Id,
                        IsAnswerCorrect = qan.IsAnswerCorrect,
                        MatchingText = qan.MatchingText,
                        Duration = qan.Duration
                    };

                    quizAnswers.Add(quizAnswer);
                });


                var quizAttempt = new QuizAttemptResource()
                {
                    Id = qa.Id,
                    Email = qa.Email,
                    Enrollment = qa.Enrollment,
                    Name = qa.Name,
                    Score = qa.Score,
                    Duration = qa.Duration,
                    Percentage = qa.Percentage,
                    QuizAnswers = quizAnswers,
                    StartDate = qa.StartDate,
                    FinishDate = qa.FinishDate,
                    CorrectQuestions = qa.CorrectQuestions,
                    Submitted = qa.Submitted

                };

                quizAttempts.Add(quizAttempt);
            });

            var linkResource = new LinkResource()
            {
                Id = link.Id,
                Name = link.Name,
                Url = link.Url,
                Password = link.Password,
                TimeLimit = link.TimeLimit,
                Code = link.Code,
                Test = new TestResource()
                {
                    Id = link.Test.Id,
                    Name = link.Test.Name,
                    Introduction = link.Test.Introduction,
                    QuestionsList = questionResources
                },
                User = link.CreatedBy.UserName,
                QuizAttempts = quizAttempts
            };

            return Ok(linkResource);

        }

        // GET api/<QuestionsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var link = await _context.Links
                .Include(c => c.Test)
                .ThenInclude(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                .ThenInclude(q => q.Answers)
                .Include(l => l.CreatedBy)
                .Include(l => l.QuizAttempts)
                .ThenInclude(qa => qa.QuizAnswers)
                .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(l => l.Id == id);



            if (link == null)
                return NotFound("Could not found an Link with this ID");

            var questionResources = new List<QuestionResource>();
            link.Test.TestQuestions.Select(tq => tq.Question).ToList().ForEach(q =>
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

            var quizAttempts = new List<QuizAttemptResource>();

            link.QuizAttempts.ForEach(qa =>
            {
                var quizAnswers = new List<QuizAnswerResource>();

                qa.QuizAnswers.ForEach(qan =>
                {
                    var quizAnswer = new QuizAnswerResource()
                    {
                        Id = qan.Id,
                        QuestionId = qan.Question.Id,
                        Answer = qan.Answer,
                        QuizAttemptId = qan.QuizAttempt.Id,
                        TestId = qan.Id,
                        IsAnswerCorrect = qan.IsAnswerCorrect,
                        MatchingText = qan.MatchingText ?? ""
                    };

                    quizAnswers.Add(quizAnswer);
                });


                var quizAttempt = new QuizAttemptResource()
                {
                    Id = qa.Id,
                    Email = qa.Email,
                    Enrollment = qa.Enrollment,
                    Name = qa.Name,
                    Score = qa.Score,
                    Duration = qa.Duration,
                    Percentage = qa.Percentage,
                    QuizAnswers = quizAnswers,
                    StartDate = qa.StartDate,
                    FinishDate = qa.FinishDate,
                    CorrectQuestions = qa.CorrectQuestions

                };

                quizAttempts.Add(quizAttempt);
            });



            var linkResource = new LinkResource()
            {
                Id = link.Id,
                Name = link.Name,
                Url = link.Url,
                Password = link.Password,
                TimeLimit = link.TimeLimit,
                Code = link.Code,
                Test = new TestResource()
                {
                    Id = link.Test.Id,
                    Name = link.Test.Name,
                    Introduction = link.Test.Introduction,
                    QuestionsList = questionResources
                },
                User = link.CreatedBy.UserName,
                QuizAttempts = quizAttempts,
                
            };

            return Ok(linkResource);
        }

        // POST api/<QuestionsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LinkResource link)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(link.User);
                var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == link.TestId);

                var modelLink = new Link()
                {
                    Name = link.Name,
                    Url = link.Url,
                    Password = link.Password,
                    TimeLimit = link.TimeLimit,
                    Test = test,
                    Code = link.Code,
                    CreatedBy = user
                };


                await _context.Links.AddAsync(modelLink);
                await _context.SaveChangesAsync();

                return Ok(modelLink.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong.");
            }

        }

        // PUT api/<QuestionsController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] LinkResource link)
        {
            try
            {
                var currentLink = await _context.Links.FirstOrDefaultAsync(q => q.Id == id);
                if (currentLink == null)
                    return NotFound("Could not found an Link with this ID");

                currentLink.Name = link.Name;
                currentLink.Url = link.Url;
                currentLink.Password = link.Password;
                currentLink.TimeLimit = link.TimeLimit;
                currentLink.Code = link.Code;

                await _context.SaveChangesAsync();

                return Ok(currentLink.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong.");
            }
        }

        // DELETE api/<QuestionsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentLink =
                    await _context.Links.FirstOrDefaultAsync(q => q.Id == id);
                if (currentLink == null)
                    return NotFound("Could not found an Question with this ID");

                currentLink.IsDeleted = true;

                await _context.SaveChangesAsync();
                return Ok(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong.");
            }

        }
    }
}
