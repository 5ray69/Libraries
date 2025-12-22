using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;
using System.Collections.Generic;

namespace Libraries.ElectricsLib.GroupService
{
    /// <summary>
    /// класс для получения всех ветвлений отходящих цепей от заданной панели
    /// </summary>
    public class PanelElectricalSystemBranches
    {
        private readonly Document _doc;
        private readonly ErrorModel _errorModel;
        public PanelElectricalSystemBranches(Document doc, ErrorModel errorModel)
        {
            _doc = doc;
            _errorModel = errorModel;
        }


        /// <summary>
        /// Получаем все ветвления отходящих цепей от заданной панели
        /// </summary>
        /// <param name="panel">панель, отходящие ветвления от которой нужно получить</param>
        /// <returns>List<ElectricalSystem> все цепи, которые составляют ветвления, отходящие от заданной панели</returns>
        public List<ElectricalSystem> GetAllFrom(FamilyInstance panel)
        {
            List<ElectricalSystem> result = [];

            HashSet<ElementId> processedPanels = [];
            HashSet<ElementId> processedCircuits = [];

            Queue<FamilyInstance> queue = [];

            queue.Enqueue(panel);  // стартовую панель добавляем в начало очереди
            processedPanels.Add(panel.Id);  // id стартовой панели добавляем в HashSet

            // цикл выполняется пока в очереди queue есть элементы
            while (queue.Count > 0)
            {
                FamilyInstance currentPanel = queue.Dequeue();  // получаем панель и одновременно удаляем её из начала очереди

                MEPModel mepModel = currentPanel.MEPModel;
                if (mepModel == null)
                {
                    //уведомляем пользователя и завершаем код
                    _errorModel.UserWarning(new FamilyIsNotMEP().MessageForUser(_doc, panel));
                }

                // получаем отходящие цепи от панели
                ISet<ElectricalSystem> systems = mepModel.GetAssignedElectricalSystems();

                //если у панели нет отходящих цепей
                if (systems == null)
                    continue;

                foreach (ElectricalSystem system in systems)
                {
                    // чтобы не обрабатывать одну и ту же цепь дважды, если system.id встречается НЕ в первый раз, то пропускаем итерацию
                    // если id есть в HashSet то метод .Add вернет false и id не будет добавлен !false = переходим к следующей итерации цикла
                    // если id нет в HashSet то метод .Add вернет true и id будет добавлен !true = не переходим к следующей итерации цикла
                    // метод .Add возвращает true или false, id будет добавлен или нет
                    if (!processedCircuits.Add(system.Id))
                        continue;

                    // добавляем цепь в результирующий список
                    result.Add(system);

                    // перебираем нагрузки цепи
                    foreach (Element element in system.Elements)
                    {
                        //если нагрузка цепи это панель = объект категории Электрооборудование
                        if (element is FamilyInstance fi &&
                            fi.Category != null &&
                            fi.Category.Id.IntegerValue ==
                            (int)BuiltInCategory.OST_ElectricalEquipment)
                        {
                            // если новая панель, если ее не было в HashSet, метод .Add вернет true
                            if (processedPanels.Add(fi.Id))
                            {
                                // добавляем в очередь
                                queue.Enqueue(fi);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
