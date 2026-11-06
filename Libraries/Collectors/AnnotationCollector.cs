using Autodesk.Revit.DB;
using Libraries.LevelsLib;

namespace Libraries.Collectors
{
    /// <summary>
    /// Коллектор аннотаций
    /// </summary>
    /// <param name="doc"></param>
    public class AnnotationCollector(Document doc)
    {
        private readonly Document _doc = doc;
        private readonly LevelAnyObject _levelAnyObject = new(doc);

        /// <summary>
        /// Возвращает ElementId аннотаций на округленнойвысотной отметке уровня long
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="roundElevationLevel"></param>
        /// <returns></returns>
        public ICollection<ElementId> GetByLevel(Document doc, long roundElevationLevel)
        {
            ICollection<ElementId> elementIds = [];

            ICollection<BuiltInCategory> cats =
                            [
                                                    BuiltInCategory.OST_TextNotes,  // текстовые примечания
                                                    BuiltInCategory.OST_Lines,  // линии детализации
                                                    BuiltInCategory.OST_RoomTags,  // марки помещений
                                                    BuiltInCategory.OST_MultiCategoryTags,  // марки нескольких категорий
                                                    BuiltInCategory.OST_MechanicalEquipmentTags,  // марки оборудования (заглушек)
                                                    BuiltInCategory.OST_GenericAnnotation,  // типовые аннотации (стрелка выносок)
                                                    BuiltInCategory.OST_Dimensions,  // размеры
                                                    BuiltInCategory.OST_IOSDetailGroups,  // группы элементов узлов
                                                    ];

            ElementMulticategoryFilter filter = new(cats);
            FilteredElementCollector collector = new(doc);
            IEnumerable<Element> elems = collector.WherePasses(filter)
                                                  .WhereElementIsNotElementType();

            foreach (Element elem in elems)
            {
                Level lev = _levelAnyObject.GetLevel(elem);
                if (lev == null) continue; // ПРОПУСТИТЬ ЭЛЕМЕНТЫ БЕЗ УРОВНЯ чтоб код не упал (когда в LevelAnyObject не выполнилось ни одно условие)

                // округление во избежание погрешностей float/double
                long roundElevationLev = (long)Math.Round(lev.Elevation * 100);
                if (roundElevationLev == roundElevationLevel)
                {
                    elementIds.Add(elem.Id);
                }
            }

            return elementIds;
        }
    }
}
