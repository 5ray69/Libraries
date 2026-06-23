using Autodesk.Revit.DB;

namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// У семейства нет электрического соединителя
    /// </summary>
    public class NoConnector
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <returns></returns>
        public string MessageForUser(FamilyInstance familyInstance)
        {
            string message = $@"
У семейства
с именем {familyInstance.Name}
категории {familyInstance.Category.Name}
с Id {familyInstance.Id}
нет электрического соединителя.
Оно не может быть подключено.

Либо уберите это семейство из вида разреза,
чтоб его не было, в виде разреза.
Либо добавьте в семейство электрический соединитель
и згрузите семейство в проект.";

            return message;
        }
    }
}
