using Autodesk.Revit.DB;
using Libraries.Collectors.UserWarningCollectors;
using Libraries.ErrorModelLib;

namespace Libraries.Collectors
{
    /// <summary>
    /// Коллектор типоразмера
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="errorModel"></param>
    public class SymbolCollector(Document doc, ErrorModel errorModel)
    {
        private readonly Document _doc = doc;
        private readonly ErrorModel _errorModel = errorModel;

        /// <summary>
        /// Возвращает типоразмер семейства
        /// </summary>
        /// <param name="builtInCategory">например, BuiltInCategory.OST_MechanicalEquipment</param>
        /// <param name="familyName"></param>
        /// <returns></returns>
        public FamilySymbol GetFamilySymbol(BuiltInCategory builtInCategory, string familyName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(_doc)
                                                                                .OfCategory(builtInCategory)
                                                                                .OfClass(typeof(FamilySymbol));

            foreach (Element elem in collector)
            {
                if (elem is FamilySymbol symbol && symbol.Family != null && symbol.Family.Name == familyName)
                {
                    return symbol; // Мгновенный выход при первом же совпадении
                }
            }

            // Если цикл завершился, а ничего не найдено — показываем предупреждение и завершаем код
            string enumBuiltInCategory = builtInCategory.ToString();
            _errorModel.UserWarning(new NoSymbolInProject().MessageForUser(familyName, enumBuiltInCategory));

            return null; // Если ничего не найдено
        }
    }
}
