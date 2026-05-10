using Autodesk.Revit.DB;

namespace Libraries.ViewLib
{
    public class ViewDto
    {
        public string ProjectSection
        {
            get;
        }
        public string ViewPurpose
        {
            get;
        }
        public string Name
        {
            get;
        }
        public ElementId Id
        {
            get;
        }



        public ViewDto(string projectSection, string viewPurpose, string name, ElementId id)
        {
            ProjectSection = projectSection;
            ViewPurpose = viewPurpose;
            Name = name;
            Id = id;
        }
    }
}
