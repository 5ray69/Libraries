using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;
using Libraries.ParametersLib.UserWarningParametersLib;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib.GroupService
{
    public class GroupCircuits
    {
        private readonly Autodesk.Revit.DB.Document _doc;
        private readonly ErrorModel _errorModel;

        private readonly ParameterValidatorMissingOrEmpty _parameterValidatorMissingOrEmpty;
        private readonly ParameterDefinition _parameterDefinition;


        //Кэшируем Definition LookupParameter
        private Definition _defGroup;

        public GroupCircuits(Autodesk.Revit.DB.Document doc, ErrorModel errorModel)
        {
            _doc = doc;
            _errorModel = errorModel;

            _parameterValidatorMissingOrEmpty = new(doc, errorModel);
            _parameterDefinition = new(doc, errorModel);

        }


        /// <summary>
        /// Для коллекции цепей, возвращает уникальные значения параметра "БУДОВА_Группа", содержащие подстроку substring
        /// </summary>
        /// <param name="elSystems">список цепей</param>
        /// <param name="substring">подстрока, содержащаяся в БУДОВА_Группа</param>
        /// <returns>HashSet<string></returns>
        public HashSet<string> GetUniqueNamesContain(ICollection<ElectricalSystem> elSystems, string substring)
        {
            ElectricalSystem firstSystem = elSystems.FirstOrDefault();

            string nameParameter = "БУДОВА_Группа";

            //записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defGroup ??= _parameterDefinition.Get(firstSystem, nameParameter);

            HashSet<string> uniqueNames = [];

            foreach (ElectricalSystem elSystem in elSystems)
            {
                Parameter param = elSystem.get_Parameter(_defGroup);

                //если у цепи нет параметра, то выведет предупреждение пользователю и завершит код
                if (param == null)
                {
                    _errorModel.UserWarning(new ParameterIsMissing().MessageForUser(elSystem, nameParameter));
                }

                ////если у цепи параметр только для чтения, то выведет предупреждение пользователю и завершит код
                //if (param.IsReadOnly)
                //{
                //    _errorModel.UserWarning(new ParameterIsReadOnly().MessageForUser(elSystem, nameParameter));
                //}
                string groupName = param.AsString();

                if (string.IsNullOrWhiteSpace(groupName))  //если параметр цепи не заполнен, то выведет предупреждение пользователю и завершит код
                {
                    _errorModel.UserWarning(new ParameterElementAtLevelEmpty().MessageForUser(_doc, elSystem, _defGroup.Name));
                }

                if(groupName.Contains(substring))
                    uniqueNames.Add(groupName);
            }

            //return uniqueNames.Where(name => name.Contains(substring)).ToList();
            return uniqueNames;
        }


        /// <summary>
        /// <para> Формирует словарь ключ: имя группы = значение: список цепей, </para>
        /// <para> если значение параметра цепи "БУДОВА_Группа" содержит подстроку. </para>
        /// <para> Цепи дублирующихся групп здесь есть </para>
        /// </summary>
        /// <param name="elSystems">список цепей</param>
        /// <param name="substring">подстрока, содержащаяся в БУДОВА_Группа</param>
        /// <returns>Dictionary<string, List<ElectricalSystem>></returns>
        public Dictionary<string, List<ElectricalSystem>> GetCircuitsContainedInGroups(ICollection<ElectricalSystem> elSystems, string substring)
        {
            ElectricalSystem firstSystem = elSystems.FirstOrDefault();

            string nameParameter = "БУДОВА_Группа";

            //записываем в поле Definition параметра один раз и проверяем существует ли параметр
            _defGroup ??= _parameterDefinition.Get(firstSystem, nameParameter);


            Dictionary<string, List<ElectricalSystem>> circuitGroups = [];

            foreach (ElectricalSystem elSystem in elSystems)
            {
                Parameter param = elSystem.get_Parameter(_defGroup);
                string groupName = param.AsString();

                if (groupName != null && groupName.Contains(substring))
                {
                    if (!circuitGroups.ContainsKey(groupName))
                        circuitGroups[groupName] = new List<ElectricalSystem>();

                    circuitGroups[groupName].Add(elSystem);
                }
            }

            return circuitGroups;
        }
    }
}
