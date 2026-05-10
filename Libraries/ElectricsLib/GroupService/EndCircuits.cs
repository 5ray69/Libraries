using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System.Collections.Generic;

namespace Libraries.ElectricsLib.GroupService
{
    /// <summary>
    /// Конечные цепи
    /// </summary>
    public class EndCircuits
    {
        /// <summary>
        /// <para> Извлекает цепи являющиеся конечными ветками в группе. </para>
        /// <para> Цепи дублирующихся групп здесь есть </para>
        /// </summary>
        /// <param name="groupCircuits"></param>
        /// <returns></returns>
        public Dictionary<string, List<ElectricalSystem>> GetEndCircuitsInGroup(Dictionary<string, List<ElectricalSystem>> groupCircuits)
        {
            //groupCircuits.Count - это начальная ёмкость словаря endCircuits,
            //чтобы при заполнении он не расширялся динамически(что вызывает дополнительные аллокации и копирование).
            //Например, если groupCircuits содержит 200 групп, то сразу резервируется место под 200 записей в endCircuits.
            //Count берётся не у значений (List<ElectricalSystem>), а у самого словаря
            var endCircuits = new Dictionary<string, List<ElectricalSystem>>(groupCircuits.Count);

            foreach (KeyValuePair<string, List<ElectricalSystem>> kvp in groupCircuits)
            {

                string groupName = kvp.Key;
                List<ElectricalSystem> circuits = kvp.Value;
                var endCircuitsInGroup = new List<ElectricalSystem>(circuits.Count);

                foreach (ElectricalSystem circuit in circuits)
                {
                    bool hasOutgoingCircuits = false;  // отходящие цепи
                    bool containsSwitch = false;  // выключатель

                    //перебираем нагрузки цепи
                    foreach (object element in circuit.Elements)
                    {
                        if (element is not FamilyInstance fi)
                            continue;

                        // если нагрузка цепи это выключатель
                        if (fi.Category?.Name == "Выключатели")
                        {
                            containsSwitch = true;
                            break;
                        }

                        // если нагрузка цепи это панель (объект категории Электрооборудование)
                        // Если это панель, а если панелей в цепи нет,
                        // то список остается пустой и выходит сразу два условия:
                        // 1.в цепи нет панелей 2.у панелей имеющихся в цепи нет отходящих цепей(ниже по коду)
                        if (fi.MEPModel is ElectricalEquipment equipment)
                        {
                            // Проверяем отходящие цепи
                            ISet<ElectricalSystem> assigned = equipment.GetAssignedElectricalSystems();

                            //Если есть отходящие от панели цепи, значит circuit не конечная цепь
                            if (assigned != null && assigned.Count > 0)
                            {
                                hasOutgoingCircuits = true;
                                break;
                            }
                        }
                    }

                    //Если в нагрузке цепи от панели нет отходящих цепей или нагрузка это не выключатель,
                    //значит circuit это конечная цепь
                    if (!hasOutgoingCircuits && !containsSwitch)
                        endCircuitsInGroup.Add(circuit);
                }

                //Если список конечных цепей в группе не пуст, то добавляем его в результирующий словарь,
                //так мы избегаем добавления пустых списков. Не будет ключей с пустыми значениями, например, "гр.1" → [].
                if (endCircuitsInGroup.Count > 0)
                    endCircuits[groupName] = endCircuitsInGroup;
            }

            return endCircuits;

            //конечная цепь если:
            //1. если в элементах цепи нет панелей - словарь ключ:группа значение:[панели] - если у какой-то группы список пуст, то она конечная
            //2. если если у всех панелей содержращихся в цепи нет цепей нагрузок - словарь ключ:группа значение:[цепи нагрузок], если хотя бы одна цепь есть в списке, то цепь не конечная
            //3. если нагрузка цепи не выключатель путь до выключателя как до конечной цепи не нужен
        }
    }
}
