using QDB.Models.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Utils.Readers
{
    public interface IDatabaseReader
    {
        public void LoadQuestions(string filename);
    }
}
