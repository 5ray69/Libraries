using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ParametersLib;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// объединяет обработку короба и соед.детали короба, возвращает список коннекторов
    /// </summary>
    public class ConduitAndFittingProcessor
    {
        /// <summary>
        /// Имя группы последнего обработанного элемента
        /// </summary>
        public string GroupName { get; private set; } = string.Empty;


        /// <summary>
        /// Обрабатывает элемент (короб или соед.деталь короба) и возвращает список коннекторов.
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="doc"></param>
        /// <param name="paramValidator"></param>
        /// <param name="connectorExtractor"></param>
        /// <param name="uniqGroupsShields"></param>
        /// <returns></returns>
        public List<Connector> ProcessElement(
                                                Element elem,
                                                Document doc,
                                                ParameterValidatorMissingOrEmpty paramValidator,
                                                ConnectorExtractor connectorExtractor,
                                                IEnumerable<string> uniqGroupsShields)
        {
            List<Connector> results = [];

            GroupName = paramValidator
                                    .ValidateAndWarning(elem, "БУДОВА_Группа")
                                    .AsString();

            //Возвращает пустой список, если группа не в списке уникальных групп.
            //Это быстрый способ отфильтровать ненужные элементы,
            //чтобы не выполнять лишние проверки и не искать коннекторы у тех объектов,
            //которые не принадлежат к нужным «щитам» или «группам».
            //Здесь мы сразу отсекаем ненужные случаи.
            //Дальше в коде остаётся только логика для "правильных" элементов.
            //Это делает метод плоским и легко читаемым.
            if (!uniqGroupsShields.Contains(GroupName))
                return results;

            switch (elem)
            {
                case Conduit conduit:
                    ProcessConduit(conduit, results, doc, paramValidator, connectorExtractor);
                    break;

                case FamilyInstance familyInstance:
                    ProcessFamilyInstance(familyInstance, results, connectorExtractor);
                    break;
            }

            return results;
        }

        /// <summary>
        /// обработка короба
        /// </summary>
        /// <param name="conduit"></param>
        /// <param name="results"></param>
        /// <param name="doc"></param>
        /// <param name="paramValidator"></param>
        /// <param name="connectorExtractor"></param>
        private void ProcessConduit(
                                    Conduit conduit,
                                    List<Connector> results,
                                    Document doc,
                                    ParameterValidatorMissingOrEmpty paramValidator,
                                    ConnectorExtractor connectorExtractor)
        {
            Parameter paramStyleConduit = paramValidator.ValidateAndWarning(conduit, "Стиль коробов");
            string nameStyle = doc.GetElement(paramStyleConduit.AsElementId())?.Name ?? string.Empty;

            if (!nameStyle.Contains("гофра"))
                return;

            results.AddRange(connectorExtractor.GetConnectors(conduit));
        }

        /// <summary>
        /// обработка соед.детали короба
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <param name="results"></param>
        /// <param name="connectorExtractor"></param>
        private void ProcessFamilyInstance(
                                            FamilyInstance familyInstance,
                                            List<Connector> results,
                                            ConnectorExtractor connectorExtractor)
        {
            results.AddRange(connectorExtractor.GetConnectors(familyInstance));
        }
    }
}
