using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Utils.Configurations
{
    public class GeneratorBufferConfig: IConfig
    {
        public int ChaptersCount { get; set; }
        public int SectionsCount { get; set; }
    }
}
