using Autodesk.Revit.DB;

namespace Libraries.ViewLib
{
    /// <summary>
    /// Dto вида
    /// </summary>
    public class ViewDto
    {
        /// <summary>
        /// Раздел проекта
        /// </summary>
        public string ProjectSection
        {
            get;
        }
        
        /// <summary>
        /// Назначение проекта
        /// </summary>
        public string ViewPurpose
        {
            get;
        }
        
        /// <summary>
        /// Имя вида
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// ElementId вида
        /// </summary>
        public ElementId Id
        {
            get;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="projectSection"></param>
        /// <param name="viewPurpose"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public ViewDto(string projectSection, string viewPurpose, string name, ElementId id)
        {
            ProjectSection = projectSection;
            ViewPurpose = viewPurpose;
            Name = name;
            Id = id;
        }
    }
}
