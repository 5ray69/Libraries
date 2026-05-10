using System.Collections.Generic;

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
