using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QDB.Utils.Generator
{
    public class SelectedIndexes
    {
        public Dictionary</*ChapterId*/int, /*Section Ids*/List<int>> Indexes { get; set; }
        public int Count { get => Indexes.Count; }
        public SelectedIndexes(int count = 0)
        {
            Indexes = new(count);
        }
    }
}
