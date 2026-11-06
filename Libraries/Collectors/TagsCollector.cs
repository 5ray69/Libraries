using Autodesk.Revit.DB;

namespace Libraries.Collectors
{
    /// <summary>
    /// Коллектор марок
    /// </summary>
    public class TagsCollector
    {
        private readonly Document _doc;

        // Главный кэш: Поиск конкретного типоразмера O(1)
        // Ключ: (Категория, Имя семейства, Имя типа)
        private readonly Dictionary<(BuiltInCategory Category, string FamilyName, string SymbolName), FamilySymbol> _symbolMap;

        // Дополнительный кэш: Если нужно получить ВСЕ типоразмеры одного семейства
        // Ключ: (Категория, Имя семейства)
        private readonly Dictionary<(BuiltInCategory Category, string FamilyName), List<FamilySymbol>> _familyMap;



        // Происходит следующий порядок событий:
        // 1.Срабатывает params: C# автоматически упаковывает аргументы OST_Doors и OST_Walls в массив BuiltInCategory[] categories.
        // 2.Перенаправление через this(...): Конструктор не заходит в свое тело { }, а перенаправляет управление во 2-й (главный) конструктор.
        // 3.Приведение типов (IEnumerable<...>): Массив categories явно приводится к интерфейсу IEnumerable<BuiltInCategory>,
        // чтобы компилятор точно понял, какой именно из перегруженных конструкторов нужно вызвать.
        // 4.Выполнение основного конструктора: Запускается код главного конструктора — выделяется память, вызывается LoadSymbols(categories) и создаются словари.
        // 5.Возврат: Когда главный конструктор завершает работу, управление возвращается в пустые фигурные скобки { } первого конструктора, и объект считается созданным.


        /// <summary>
        /// <para> 1-й конструктор принимающий одну или несколько категорий </para>
        /// <para> BuiltInCategory можно перечислять через запятую, </para>
        /// <para> без создания коллекции, например  </para>
        /// <para> TagsCollector(doc, BuiltInCategory.OST_RoomTags, BuiltInCategory.OST_ConduitTags); </para>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="categories"></param>
        public TagsCollector(Document doc, params BuiltInCategory[] categories)
            : this(doc, (IEnumerable<BuiltInCategory>)categories) { }


        /// <summary>
        /// <para> 2-й конструктор принимающий коллекцию </para>
        /// <para> IEnumerableBuiltInCategory — позволяет передавать уже готовые коллекции </para>
        /// <para> ListBuiltInCategory, HashSet, результаты LINQ-запросов и т.д. </para>
        /// <para> без необходимости конвертировать их в массив через.ToArray(): </para>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="categories"></param>
        public TagsCollector(Document doc, IEnumerable<BuiltInCategory> categories)
        {
            _symbolMap = new Dictionary<(BuiltInCategory, string, string), FamilySymbol>();
            _familyMap = new Dictionary<(BuiltInCategory, string), List<FamilySymbol>>();

            LoadSymbols(categories);
        }

        /// <summary>
        /// Однократная выгрузка всех элементов указанных категорий из Revit DB
        /// </summary>
        private void LoadSymbols(IEnumerable<BuiltInCategory> categories)
        {
            var categoryList = new List<BuiltInCategory>(categories);
            if (categoryList.Count == 0) return;

            // 1. Создаем коллектор
            var collector = new FilteredElementCollector(_doc)
                                                        .WhereElementIsElementType()
                                                        .OfClass(typeof(FamilySymbol));

            // 2. Оптимизация нативного фильтра C++:
            // Если категория одна — используем OfCategory, если несколько — ElementMulticategoryFilter
            if (categoryList.Count == 1)
            {
                collector.OfCategory(categoryList[0]);
            }
            else
            {
                var multiCategoryFilter = new ElementMulticategoryFilter(categoryList);
                collector.WherePasses(multiCategoryFilter);
            }

            // 3. Заполнение кэша в C# .NET
            foreach (FamilySymbol symbol in collector)
            {
                if (symbol.Category == null) continue;

                BuiltInCategory cat = symbol.Category.BuiltInCategory;
                string familyName = symbol.FamilyName;
                string symbolName = symbol.Name;

                // Ключ для точного поиска типоразмера
                var symbolKey = (cat, familyName, symbolName);
                _symbolMap[symbolKey] = symbol;

                // Ключ для группировки по семействам
                var familyKey = (cat, familyName);
                if (!_familyMap.TryGetValue(familyKey, out var familyList))
                {
                    familyList = new List<FamilySymbol>();
                    _familyMap[familyKey] = familyList;
                }
                familyList.Add(symbol);
            }
        }

        // <summary>
        // Мгновенное получение конкретного типоразмера O(1)
        // </summary>


        /// <summary>
        /// <para> Возвращает один типоразмер </para>
        /// <para> по имени семейства и имени типоразмера </para>
        /// </summary>
        /// <param name="category"></param>
        /// <param name="familyName"></param>
        /// <param name="symbolName"></param>
        /// <returns></returns>
        public FamilySymbol GetSymbol(BuiltInCategory category, string familyName, string symbolName)
        {
            if (_symbolMap.TryGetValue((category, familyName, symbolName), out var symbol))
            {
                return symbol;
            }
            return null;
        }


        /// <summary>
        /// <para> Возвращает все типоразмеры семейства </para>
        /// <para> по имени семейства и категории </para>
        /// </summary>
        /// <param name="category"></param>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public List<FamilySymbol> GetSymbolsByFamily(BuiltInCategory category, string familyName)
        {
            if (_familyMap.TryGetValue((category, familyName), out var list))
            {
                return list;
            }
            return [];  // пустой список
        }


    }
}


////Пример использования
//// 1. Инициализируем кэш один раз в начале команды, передав нужные категории
//TagsCollector tagsCollector = new(
//                                    doc,
//                                    BuiltInCategory.OST_MechanicalEquipmentTags,
//                                    BuiltInCategory.OST_PipeAccessoryTags,
//                                    BuiltInCategory.OST_DuctAccessoryTags
//                                    );

//// 2. Теперь в любых циклах и алгоритмах вызов происходит мгновенно
//FamilySymbol symbol1 = tagsCollector.GetSymbol(
//                                        BuiltInCategory.OST_MechanicalEquipmentTags,
//                                        "BDV_E000_Маркировка_отверстий",
//                                        "Тип 1"
//                                        );

//// 3. Можно легко вытащить все типы конкретного семейства
//var allTypes = tagsCollector.GetSymbolsByFamily(
//                                        BuiltInCategory.OST_MechanicalEquipmentTags,
//                                        "BDV_E000_Маркировка_отверстий"
//                                        );