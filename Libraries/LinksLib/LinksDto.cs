namespace Libraries.LinksLib
{
    /// <summary>
    /// Dto связей
    /// </summary>
    public class LinksDto
    {
        /// <summary>
        /// Имена связей
        /// </summary>
        public ICollection<string> LinksNames
        {
            get;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="linksNames"></param>
        public LinksDto(ICollection<string> linksNames)
        {
            LinksNames = linksNames;
        }
    }
}
