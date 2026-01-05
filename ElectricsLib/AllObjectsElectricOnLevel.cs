using Autodesk.Revit.DB;
using Libraries.LevelsLib;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib
{
    public class AllObjectsElectricOnLevel(Document doc, List<string> userSelect, ElementId activeViewId)
    {
        private readonly Document _doc = doc;
        private readonly LevelAnyObject _levelAnyObject = new(doc);

        private readonly List<ElementId> _levelIds = LevelCache.GetSortedLevels(doc)
                                                    .Where(lev => userSelect.Contains(lev.Name))
                                                    .Select(lev => lev.Id)
                                                    .ToList();

        public ICollection<ElementId> GetElementId()
        {
            ICollection<ElementId> elementIds = [];


            // HashSet<int> - исключение дублирующихся элементов автоматически
            // проверка пуш
            HashSet<int> annotationCategories =
            [
                (int)BuiltInCategory.OST_TextNotes,  // текстовые примечания
                (int)BuiltInCategory.OST_Lines,  // линии детализации
                (int)BuiltInCategory.OST_ElectricalEquipmentTags,  // марки электрооборудования
                (int)BuiltInCategory.OST_LightingFixtureTags,  // марки осветительных приборов
                (int)BuiltInCategory.OST_RoomTags,  // марки помещений
                (int)BuiltInCategory.OST_MultiCategoryTags,  // марки нескольких категорий
                (int)BuiltInCategory.OST_ConduitTags,  // марки коробов
                (int)BuiltInCategory.OST_GenericAnnotation,  // типовые аннотации (стрелка выносок)
                (int)BuiltInCategory.OST_DetailComponents,  // элементы узлов
                (int)BuiltInCategory.OST_Dimensions,  // размеры
                (int)BuiltInCategory.OST_IOSDetailGroups  // группы элементов узлов
            ];

            ICollection<BuiltInCategory> categories =
            [
                BuiltInCategory.OST_ElectricalFixtures,  // эл.приборы
                BuiltInCategory.OST_ElectricalEquipment,  // электрооборудование
                BuiltInCategory.OST_LightingDevices,  // выключатели
                BuiltInCategory.OST_LightingFixtures,  // осветительные приборы
                BuiltInCategory.OST_ConduitFitting,  // соед.детали коробов
                BuiltInCategory.OST_CableTrayFitting,  // соед.детали кабельных лотков
                BuiltInCategory.OST_FireAlarmDevices,  // пожарная сигнализация
                BuiltInCategory.OST_MechanicalEquipment,  // оборудование
                BuiltInCategory.OST_Conduit,  // короба
                BuiltInCategory.OST_CableTray,  // кабельные лотки
                BuiltInCategory.OST_TextNotes,  // текстовые примечания
                BuiltInCategory.OST_Lines,  // линии детализации
                BuiltInCategory.OST_ElectricalEquipmentTags,  // марки электрооборудования
                BuiltInCategory.OST_LightingFixtureTags,  // марки осветительных приборов
                BuiltInCategory.OST_RoomTags,  // марки помещений
                BuiltInCategory.OST_MultiCategoryTags,  // марки нескольких категорий
                BuiltInCategory.OST_ConduitTags,  // марки коробов
                BuiltInCategory.OST_GenericAnnotation,  // типовые аннотации (стрелка выносок)
                BuiltInCategory.OST_Dimensions,  // размеры
                BuiltInCategory.OST_IOSDetailGroups,  // группы элементов узлов
                BuiltInCategory.OST_DetailComponents,  // элементы узлов
                BuiltInCategory.OST_ElectricalCircuit  // эл.цепи
            ];

            ElementMulticategoryFilter categoryFilter = new(categories);
            FilteredElementCollector collector = new FilteredElementCollector(_doc)
                                                        .WherePasses(categoryFilter)
                                                        .WhereElementIsNotElementType();

            //ICollection<Element> elements = [];  // ICollection<Element> - сохраняются дублирующиеся элементы и порядок добавления элементов
            HashSet<Element> elements = [];  // HashSet<Element> - исключение дублирующихся элементов автоматически и порядок добавления элементов нарушен
            foreach (Element element in collector)
            {
                //Нельзя менять порядок следования условий!!!
                //Семейства принципиальных схем категории Типовые аннатации,
                //тоже являются FamilyInstance и подпадают под другое условие

                int categoryId = element.Category.Id.IntegerValue;
                if (annotationCategories.Contains(categoryId))
                {
                    // Для элементов аннотаций и др находящихся на четрежных видах и разрезах
                    // Проверка на наличие OwnerViewId, принадлежности активному виду и тем, что OwnerView это ViewPlan исключаем возможность,
                    // что аннотация не находится на ViewDrafting(чертеже), ViewSection(разрезе) или ViewSheet(листе)
                    if (element.OwnerViewId == activeViewId && _doc.GetElement(element.OwnerViewId) is ViewPlan)
                    {
                        elements.Add(element);
                    }
                    continue;
                }

                //если семейство не вложенное
                if (element is FamilyInstance familyInstance && familyInstance.SuperComponent == null)
                {
                    elements.Add(element);
                    continue;
                }

                // добавляем остальные элементы
                elements.Add(element);
            }


            foreach (Element element in elements)
            {
                Level level = _levelAnyObject.GetLevel(element);
                if (level == null) continue; // ПРОПУСТИТЬ ЭЛЕМЕНТЫ БЕЗ УРОВНЯ чтоб код не упал (когда в LevelAnyObject не выполнилось ни одно условие)

                ElementId levelIdElement = level.Id;
                if (_levelIds.Contains(levelIdElement))
                    elementIds.Add(element.Id);
            }

            return elementIds;
        }
    }
}
