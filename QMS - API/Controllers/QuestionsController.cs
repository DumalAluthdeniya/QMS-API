using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class QuestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuestionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/<QuestionsController>
        [HttpGet("all/{user}")]
        public async Task<IActionResult> Get(string user)
        {
            var loggedUser = await _userManager.FindByNameAsync(user);

            var questions = await _context.Questions.Include(c => c.Answers).Where(q => !q.IsDeleted && q.CreatedBy == loggedUser).ToListAsync();


            var questionResources = new List<QuestionResource>();

            questions.ForEach(q =>
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

            return Ok(questionResources);

        }

        // GET api/<QuestionsController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {

            var question = await _context.Questions.Include(c => c.Answers).FirstOrDefaultAsync(q => q.Id == id);
            if (question == null)
                return NotFound("Could not found an Question with this ID");

            var answerResources = new List<AnswerResource>();
            question.Answers.ForEach(a =>
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
                Id = question.Id,
                Title = question.Title,
                Topic = question.Topic,
                QuestionType = question.QuestionType,
                AnswerMaxLength = question.AnswerMaxLength,
                DifficultyLevel = question.DifficultyLevel,
                Points = question.Points,
                RandomizeAnswers = question.RandomizeAnswers,
                Answers = answerResources
            };

            return Ok(questionResource);
        }

        // POST api/<QuestionsController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] QuestionResource question)
        {
            try
            {
                var modelQuestion = new Question()
                {
                    Title = question.Title,
                    Points = question.Points,
                    Topic = question.Topic,
                    AnswerMaxLength = question.AnswerMaxLength,
                    DifficultyLevel = question.DifficultyLevel,
                    QuestionType = question.QuestionType,
                    RandomizeAnswers = question.RandomizeAnswers,
                    CreatedBy = await _userManager.FindByNameAsync(question.User)
                };



                var modelAnswers = new List<Answer>();
                question.Answers.ForEach(answer =>
                {
                    var answerAdd = new Answer()
                    {
                        Name = answer.Name,
                        Question = modelQuestion,
                        IsCorrectAnswer = answer.IsCorrectAnswer,
                        MatchingText = answer.MatchingText
                    };

                    modelAnswers.Add(answerAdd);

                });

                modelQuestion.Answers = modelAnswers;
                await _context.Questions.AddAsync(modelQuestion);
                await _context.Answers.AddRangeAsync(modelAnswers);
                await _context.SaveChangesAsync();

                return Ok(modelQuestion.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong.");
            }

        }

        // PUT api/<QuestionsController>/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] QuestionResource question)
        {
            try
            {
                var currentQuestion = await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id);
                if (currentQuestion == null)
                    return NotFound("Could not found an Question with this ID");



                if (currentQuestion.Answers != null)
                    _context.Answers.RemoveRange(currentQuestion.Answers);

                currentQuestion.Title = question.Title;
                currentQuestion.Topic = question.Topic;
                currentQuestion.Points = question.Points;
                currentQuestion.AnswerMaxLength = question.AnswerMaxLength;
                currentQuestion.DifficultyLevel = question.DifficultyLevel;
                currentQuestion.QuestionType = question.QuestionType;
                currentQuestion.RandomizeAnswers = question.RandomizeAnswers;


                var modelAnswers = new List<Answer>();
                question.Answers.ForEach(answer =>
                {
                    var answerAdd = new Answer()
                    {
                        Name = answer.Name,
                        Question = currentQuestion,
                        IsCorrectAnswer = answer.IsCorrectAnswer,
                        MatchingText = answer.MatchingText
                    };

                    modelAnswers.Add(answerAdd);

                });


                currentQuestion.Answers = modelAnswers;

                await _context.AddRangeAsync(modelAnswers);
                await _context.SaveChangesAsync();

                return Ok(currentQuestion.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong.");
            }
        }

        // DELETE api/<QuestionsController>/5
        [HttpPost("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var currentQuestion =
                    await _context.Questions.Include(c => c.Answers).FirstOrDefaultAsync(q => q.Id == id);
                if (currentQuestion == null)
                    return NotFound("Could not found an Question with this ID");

                currentQuestion.IsDeleted = true;

                await _context.SaveChangesAsync();
                return Ok(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong.");
            }
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetQuestionTypes()
        {
            var types = await _context.QuestionTypes.ToListAsync();
            return Ok(types);
        }
        [HttpGet("difficultyLevels")]
        public async Task<IActionResult> GetQuestionDifficultyLevels()
        {
            var types = await _context.QuestionDifficulties.ToListAsync();
            return Ok(types);
        }
    }
}
