using QDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Database
{
    public class DifficultiesExtensions
    {
        public static List<QDbDifficulty> GetAll()
        {
            List<QDbDifficulty> diffs = new List<QDbDifficulty>();
            using (QDbContext ctx = QDbContext.GetInstance())
            {
                diffs = ctx.Difficulties.ToList();
            }
            return diffs;
        }
    }
}
