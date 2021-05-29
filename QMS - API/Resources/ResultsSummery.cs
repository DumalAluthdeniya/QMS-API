using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS_API.Resources
{
    public class ResultsSummery
    {
        public int TotalQuestions { get; set; }
        public int TotalMark { get; set; }
        public decimal MarksObtained { get; set; }
        public int CorrectAnswers { get; set; }
        public int IncorrectAnswers { get; set; }
        public List<string> SummeryTexts { get; set; }
    }
}
