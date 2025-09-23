using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libraries.LinksLib
{
    public class LinksDto
    {
        public ICollection<string> LinksNames
        {
            get;
        }

        public LinksDto(ICollection<string> linksNames)
        {
            LinksNames = linksNames;
        }
    }
}
