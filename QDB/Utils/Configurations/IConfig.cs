using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QDB.Utils.Configurations
{
    public interface IConfig
    {
        public string Name => this.GetType().Name;

    }
}
