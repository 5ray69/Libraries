using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;
using Libraries.ElectricsLib.UserWarningElectricsLib;

namespace Libraries.ElectricsLib
{
    public class CircuitNamingSchemeChecker(Document doc, ErrorModel errorModel)
    {
        private readonly Document _doc = doc;
        private readonly ErrorModel _errorModel = errorModel;

        /// <summary>
        /// <para>Проверяет настройку "По проекту – схема обозн. цепей"</para>
        /// <para>В Управление - Настройки МЕР - Настройки</para>
        /// <para>электротехнических систем - Обозначение цепей</para>
        /// </summary>
        public bool ValidateCircuitNamingScheme()
        {
            CircuitNamingSchemeSettings settings = GetCircuitNamingSchemeSettings();
            if (settings == null || settings.CircuitNamingSchemeId == ElementId.InvalidElementId)
            {
                _errorModel.UserWarning(new CircuitNamingSchemeSettingseError().MessageForUser());
                return false; // Ошибка: схема не найдена или неверна
            }

            //По проекту - схема обозн. цепей: Имя нагрузки эт_цепь
            //в русском шрифте с учетом регистров и пробелов
            Element namingElement = _doc.GetElement(settings.CircuitNamingSchemeId);
            if (namingElement == null || namingElement.Name != "Имя нагрузки эт_цепь")
            {
                _errorModel.UserWarning(new CircuitNamingSchemeSettingseError().MessageForUser());
                return false; // Ошибка: неверное имя схемы
            }

            return true; // Всё в порядке
        }

        /// <summary>
        /// Получает настройки именования цепей
        /// </summary>
        private CircuitNamingSchemeSettings GetCircuitNamingSchemeSettings()
        {
            FilteredElementCollector collector = new(_doc);
            collector.OfClass(typeof(CircuitNamingSchemeSettings));

            foreach (Element elem in collector)
            {
                if (elem is CircuitNamingSchemeSettings settings)
                    return settings;
            }
            return null; // Если не найдено
        }
    }
}


//ПРОВЕРКА, ЧТО В "ПО ПРОЕКТУ" СХЕМА ОБОЗНАЧЕНИЯ ЦЕПЕЙ ПО ИМЕНИ НАГРУЗКИ
//FilteredElementCollector collectorCircuitNaming = new FilteredElementCollector(doc).OfClass(typeof(CircuitNamingSchemeSettings));
//foreach (Element element in collectorCircuitNaming)
//{
//    CircuitNamingSchemeSettings circuitNamingSchemeSettings = element as CircuitNamingSchemeSettings;
//    string nameScheme = doc.GetElement(circuitNamingSchemeSettings.CircuitNamingSchemeId).Name;
//    //elementIdSchE.Add(nameScheme);
//    if (nameScheme == "Имя нагрузки эт_цепь")
//        elementIdSchE.Add(nameScheme);
//    else
//        elementIdSchE.Add("не равно");

//}
//MessageBox.Show($"список имен цепей: {string.Join(", ", elementIdSchE)}");

