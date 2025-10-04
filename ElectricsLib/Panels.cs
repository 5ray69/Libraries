using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using Libraries.LevelsLib;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// После использования класса нужно отписаться от события
    /// </summary>
    public class Panels
    {
        private readonly Document _doc;
        private readonly UIApplication _uiApp;
        private List<FamilyInstance> _cachedPanels;

        public Panels(Document doc, UIApplication uiApp)
        {
            _doc = doc;
            _uiApp = uiApp;

            // Подписываемся на событие изменения документа
            _uiApp.Application.DocumentChanged += OnDocumentChanged;
        }

        /// <summary>
        /// После использования класса нужно отписаться от события, чтобы избежать утечек памяти.
        /// </summary>
        /// <param name="uiApp"></param>
        public void UnsubscribeEvent()
        {
            _uiApp.Application.DocumentChanged -= OnDocumentChanged;
        }

        /// <summary>
        /// Возвращает список всех панелей (с кэшированием).
        /// </summary>
        public IEnumerable<FamilyInstance> GetAll()
        {
            if (_cachedPanels == null) // первый вызов или сброшенный кэш
            {
                _cachedPanels = new FilteredElementCollector(_doc)
                    .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
                    .WhereElementIsNotElementType()
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .ToList();
            }

            return _cachedPanels;
        }

        /// <summary>
        /// Найти панель по имени.
        /// </summary>
        public FamilyInstance GetByName(string name)
        {
            return GetAll().FirstOrDefault(p => p.Name == name);
        }

        /// <summary>
        /// Вернет первую панель, у которой значение параметра Имя панели равно строке.
        /// </summary>
        public FamilyInstance GetByParameterNamePanel(string valueParameter)
        {
            return GetAll()
                .FirstOrDefault(p =>
                    p.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME)?.AsString() == valueParameter);
        }

        /// <summary>
        /// Вернет первую панель, у которой значение параметра Имя панели содержит подстроку.
        /// </summary>
        public FamilyInstance GetByParameterNamePanelContain(string valueParameterContain)
        {
            return GetAll()
                .FirstOrDefault(p =>
                    (bool)(p.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME)?.AsString().Contains(valueParameterContain)));
        }

        /// <summary>
        /// Вернет первую панель, у которой подстрока до последней точки выделенная из значение параметра Имя панели равна подстроке.
        /// </summary>
        public FamilyInstance GetByParameterNamePanelLastDot(string valueToLastDot)
        {
            FamilyInstance panelResult = null;
            foreach (FamilyInstance panel in GetAll())
            {
                string valParam = panel.get_Parameter(BuiltInParameter.RBS_ELEC_PANEL_NAME)?.AsString();
                //Если подстрока содержится в имени панели
                //'точка подключений' тоже содержится в 'точка подключений2', поэтому дальше будет проверка на строгое равенство
                if (valParam != null && valParam.Contains(valueToLastDot))
                {
                    int dotIndex = valParam.LastIndexOf('.');
                    string valAfterLastDot = dotIndex >= 0
                                                ? valParam.Substring(dotIndex + 1)  // подстрока после последней точки U1 (этаж)
                                                : valParam;

                    LevelAnyObject levelAnyObject = new LevelAnyObject(_doc);
                    string levelName = levelAnyObject.GetLevel(panel).Name;
                    string levelAfterDotNumber = levelName.Substring(1, 2);  // подстрока из имени уровня размещения семейства U1 (этаж)

                    //Проверяем есть ли 'точка этаж' в имени панели и соответствует ли она уровню размещения семейства
                    if (levelAfterDotNumber != valAfterLastDot
                        || dotIndex == 0)
                    {
                        ErrorModel errorModel = new();
                        errorModel.UserWarning(new NoDotLevelInNamePanel().MessageForUser(_doc, panel));
                    }

                    string valParamToLastDot = valParam.Substring(0, dotIndex);  // подстрока после последней точки U1 (этаж)
                    if (valParamToLastDot == valueToLastDot) // строгое равенство
                    {
                        panelResult = panel;
                        break;
                    }
                }
            }

            return panelResult;
        }



        /// <summary>
        /// Сброс кэша
        /// </summary>
        public void InvalidateCache()
        {
            _cachedPanels = null;
        }

        /// <summary>
        /// Автоматический сброс кэша при изменении документа
        /// </summary>
        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            // Проверяем изменилось ли что-то в документе (изменены/добавлены/удалены)
            // Собираем все Id в одну коллекцию
            var changedIds = e.GetModifiedElementIds()
                              .Concat(e.GetAddedElementIds())
                              .Concat(e.GetDeletedElementIds());

            // Проверяем только изменения в ElectricalEquipment
            foreach (var id in changedIds)
            {
                Element elem = _doc.GetElement(id);
                if (elem != null && elem.Category != null &&
                    elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_ElectricalEquipment)
                {
                    InvalidateCache(); // сбрасываем кэш
                    break; // достаточно найти хотя бы один
                }
            }
        }
    }
}


//private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
//{
//    // Проверяем, изменились ли панели (добавлены/удалены/изменены)
//    bool hasChanges = e.GetAddedElementIds().Any()
//                   || e.GetDeletedElementIds().Any()
//                   || e.GetModifiedElementIds().Any();

//    if (hasChanges)
//    {
//        InvalidateCache();
//    }
//}



//private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
//{
//    var doc = sender as Document;

//    var added = e.GetAddedElementIds()
//                 .Select(id => doc.GetElement(id))
//                 .OfType<FamilyInstance>()
//                 .Where(fi => fi.Category.Id.IntegerValue ==
//                              (int)BuiltInCategory.OST_ElectricalEquipment);

//    var deleted = e.GetDeletedElementIds(); // тут уже элементов нет, только Id
//    var modified = e.GetModifiedElementIds()
//                    .Select(id => doc.GetElement(id))
//                    .OfType<FamilyInstance>()
//                    .Where(fi => fi.Category.Id.IntegerValue ==
//                                 (int)BuiltInCategory.OST_ElectricalEquipment);

//    if (added.Any() || deleted.Any() || modified.Any())
//    {
//        // Сбрасываем кэш панелей
//        InvalidateCache();
//    }
//}

