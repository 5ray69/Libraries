using Autodesk.Revit.DB;
using Libraries.Collectors.UserWarningCollectors;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;

namespace Libraries.Collectors
{
    /// <summary>
    /// <para> Двери из связанного файла Revit </para>
    /// </summary>
    /// <param name="linkDoc"></param>
    /// <param name="errorModel"></param>
    public class DoorsCollector(Document linkDoc, ErrorModel errorModel)
    {
        private readonly Document _linkDoc = linkDoc;
        private readonly ErrorModel _errorModel = errorModel;
        private readonly ParameterValidatorIsMissing parameterValidatorIsMissing = new(errorModel);


        /// <summary>
        /// <para> Возвращает список всех дверей </para>
        /// <para> и словарь дверей по высотным отметкам уровней, приведенных к координатам основного файла </para>
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public DoorsResult GetCollections(Transform transform)
        {
            DoorsResult doorsSortingResult = new();


            // Собираем элементы из категории Оборудование
            FilteredElementCollector collector = new FilteredElementCollector(_linkDoc)
                                .OfCategory(BuiltInCategory.OST_Doors)
                                .OfClass(typeof(FamilyInstance));

            // Получаем количество элементов «нативно» без загрузки в память C#
            int collectorSize = collector.GetElementCount();

            // Если в связанном файле не размещено ни одной двери завершаем код с уведомлением пользователя
            if (collectorSize == 0)
            {
                _errorModel.UserWarning(new NoDoorsInLink().MessageForUser());
                //return result;
            }

            // Локальный кэш для отметки уровня, чтобы не вызывать level.Elevation для каждого FamilyInstance повторно
            Dictionary<ElementId, long> levelKeyCache = [];

            foreach (Element elem in collector)
            {
                if (elem is not FamilyInstance familyInstance) continue;

                if (familyInstance.SuperComponent != null) continue;

                Level level = _linkDoc.GetElement(familyInstance.LevelId) as Level;

                if (level == null)
                {
                    _errorModel.UserWarning(new FamilyHasNoLevel().MessageForUser(familyInstance)); // завершаем код
                    //return result;
                }

                ElementId levelId = level.Id;

                // Получаем или вычисляем ключ уровня через локальный кэш, что бы для каждого FamilyInstance не вызывать level.Elevation повторно
                if (!levelKeyCache.TryGetValue(levelId, out long levelKey))  // если ключа/ElementId уровня еще нет в словаре, то создаем его
                {
                    // Берем отметку уровня в локальных координатах связи и формируем точку
                    XYZ localPoint = new(0, 0, level.Elevation);

                    // Трансформируем точку в глобальные координаты основного файла
                    XYZ hostPoint = transform.OfPoint(localPoint);

                    // Получаем реальную высоту в координатах основного файла (берем Z-координату)
                    double hostElevation = hostPoint.Z;

                    // Теперь округляем для сопоставления c высотной отметкой уровня в основном файле
                    // округление во избежание погрешностей float/double
                    levelKey = (long)Math.Round(hostElevation * 100);

                    levelKeyCache[levelId] = levelKey;
                }


                doorsSortingResult.DoorsAll.Add(familyInstance);

                if (!doorsSortingResult.DoorsByLevels.TryGetValue(levelKey, out HashSet<FamilyInstance> allSet))
                {
                    allSet = [];
                    doorsSortingResult.DoorsByLevels[levelKey] = allSet;
                }
                allSet.Add(familyInstance);
            }
            return doorsSortingResult;
        }
    }


    /// <summary>
    /// Класс-контейнер для хранения результатов сортировки дверей по уровням
    /// </summary>
    public class DoorsResult
    {
        /// <summary>
        /// Спсок всех дверей
        /// </summary>
        public List<FamilyInstance> DoorsAll { get; } = [];


        /// <summary>
        /// Ключ - высотная отметка Elevation уровня, значение - HashSet дврей на этом уровне.
        /// </summary>
        public Dictionary<long, HashSet<FamilyInstance>> DoorsByLevels { get; } = [];
    }
}
