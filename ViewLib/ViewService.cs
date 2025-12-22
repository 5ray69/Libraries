using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Libraries.ViewLib
{
    public class ViewService
    {
        /// <summary>
        /// Создает список DTO видов из коллекции видов
        /// </summary>
        /// <param name="views"></param>
        /// <returns>List<ViewDto> список DTO видов</returns>
        public List<ViewDto> GetViewDto(ICollection<View> views)
        {
            List<ViewDto> viewDtos = [];

            foreach (View view in views)
            {
                Parameter projSectParam = view.LookupParameter("БУДОВА_Раздел проекта");
                // если параметр есть и он не пустой и не пробел
                string projectSection = projSectParam != null && !string.IsNullOrWhiteSpace(projSectParam.AsString())
                                            ? projSectParam.AsString()
                                            : "Раздела проекта нет";


                Parameter viewPurpParam = view.LookupParameter("БУДОВА_Назначение вида");
                // если параметр есть и он не пустой и не пробел
                string viewPurpose = viewPurpParam != null && !string.IsNullOrWhiteSpace(viewPurpParam.AsString())
                                            ? viewPurpParam.AsString()
                                            : "Назначения вида нет";

                viewDtos.Add(new ViewDto(
                                        projectSection,
                                        viewPurpose,
                                        view.Name,
                                        view.Id
                                        ));
            }

            return viewDtos;
        }
    }
}
