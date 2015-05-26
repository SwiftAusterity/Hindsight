using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divergence.DataAccess.DataClasses
{
    public enum Origin : short
    {
        Unvetted = 0,
        Private = 1,
        Vetted = 2,
        System = 3
    }
}
