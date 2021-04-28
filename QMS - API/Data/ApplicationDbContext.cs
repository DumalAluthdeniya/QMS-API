using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using QMS_API.Models;

namespace QMS_API.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<QuestionDifficulty> QuestionDifficulties { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestQuestion> TestQuestions { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<TestQuestion>()
            //    .HasOne<Test>(sc => sc.Test)
            //    .WithMany(s => s.TestQuestions);

            //modelBuilder.Entity<TestQuestion>()
            //    .HasOne<Question>(sc => sc.Question)
            //    .WithMany(s => s.TestQuestions);

        }
    }
}
