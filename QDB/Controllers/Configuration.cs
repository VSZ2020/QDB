using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Controllers
{
    public class Configuration
    {
        public static int DefaultAnswersCount { get; set; } = 3;
        public const int ServiceFieldId_Add = -1;
        public const int ServiceFieldId_EditAll = -5;
        public const int MAX_ANSWERS_COUNT = 10;

    }
}
