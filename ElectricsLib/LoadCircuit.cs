using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// Нагрузки цепи
    /// </summary>
    public class LoadCircuit
    {
        /// <summary>
        /// Вспомогательный метод. Не для использования.<br/>
        /// Перечисляет все FamilyInstance из системы нагрузок как поток<br/>
        /// </summary>
        public IEnumerable<FamilyInstance> EnumerateLoads(ElectricalSystem electricalSystem)
        {
            foreach (Element el in electricalSystem.Elements)
            {
                if (el is FamilyInstance fi)
                    yield return fi;
            }
        }


        /// <summary>
        /// Возвращает все элементы нагрузок цепи
        /// </summary>
        /// <param name="electricalSystem"></param>
        /// <returns></returns>
        public List<FamilyInstance> GetAllLoads(ElectricalSystem electricalSystem)
            => EnumerateLoads(electricalSystem).ToList();


        /// <summary>
        /// Возвращает первый элемент нагрузок цепи
        /// </summary>
        /// <param name="electricalSystem"></param>
        /// <returns></returns>
        public FamilyInstance GetFirstLoad(ElectricalSystem electricalSystem)
            => EnumerateLoads(electricalSystem).FirstOrDefault();


        /// <summary>
        /// Возвращает первый элемент нагрузок цепи соответствующий имени
        /// </summary>
        /// <param name="electricalSystem"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public FamilyInstance FirstWithName(ElectricalSystem electricalSystem, string name)
        {
            return EnumerateLoads(electricalSystem).FirstOrDefault(x => x.Name == name);
        }


        /// <summary>
        /// Соответсвуют ли элементы нагрузок цепи<br/>
        /// указанной категории<br/>
        /// </summary>
        /// <param name="electricalSystem"></param>
        /// <returns></returns>
        public bool IsCategory(ElectricalSystem electricalSystem, BuiltInCategory builtInCategory)
        {
            int targetCategoryId = (int)builtInCategory;

            foreach (var load in EnumerateLoads(electricalSystem))
            {
                if (load.Category.Id.IntegerValue != targetCategoryId)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Возвращает список элементов нагрузок цепи,<br/>
        /// которые не соответствуют указанной категории.<br/>
        /// Возвращается список не более 10 элементов<br/>
        /// </summary>
        /// <param name="electricalSystem"></param>
        /// <returns></returns>
        public List<FamilyInstance> LoadsMismatchCategory(ElectricalSystem electricalSystem, BuiltInCategory builtInCategory)
        {
            List<FamilyInstance> mismatchLoads = [];

            int targetCategoryId = (int)builtInCategory;

            foreach (var load in EnumerateLoads(electricalSystem))
            {
                if (load.Category.Id.IntegerValue != targetCategoryId)
                    mismatchLoads.Add(load);
            }
            return mismatchLoads.Count < 10 ? mismatchLoads : mismatchLoads.Take(10).ToList();
        }


        //Принимает в качестве параметра коллекцию условий в виде предикатов(Func<string, bool>).
        //Это позволит передавать любые условия для проверки.
        //bool isFound = CheckElectricalSystem(electricalSystem,
        //                                    name => name.Contains("Щит этажный"),                   // Условие 1
        //                                    name => name.Contains("Щит") && name.Contains("офис"));  // Условие 2
        //if (isFound)
        //    Console.WriteLine("Подходящая электрическая система найдена!");
        /// <summary>
        /// возвращает true,<br/>
        /// если у нагрузки цепи,<br/>
        /// имя типоразмера семейства, удовлетворяет условиям,<br/>
        /// иначе false
        /// </summary>
        public bool FamilyNameContain(ElectricalSystem electricalSystem, params Func<string, bool>[] conditions)
        {
            ElementSetIterator iterator = electricalSystem.Elements.ForwardIterator();
            while (iterator.MoveNext())
            {
                if (iterator.Current is FamilyInstance familyInstance)
                {
                    string familyName = familyInstance.Symbol.FamilyName;
                    foreach (var condition in conditions)
                    {
                        if (condition(familyName))
                        {
                            return true; // Условие выполнено
                        }
                    }
                }
            }
            return false; // Если ни одно условие не выполнено
        }
    }
}
