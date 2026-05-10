using Autodesk.Revit.DB;
using Libraries.LevelsLib;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib
{
    /// <summary>
    /// одна группа доз одного типа (одного семейства) с одинаковым именем до точки
    /// </summary>
    public class DozaGroup
    {
        private readonly Document _doc;
        public string Name
        {
            get; private set;
        }
        public List<FamilyInstance> Elements
        {
            get; private set;
        }

        public DozaGroup(Document doc)
        {
            _doc = doc;
            Name = string.Empty;
            Elements = new List<FamilyInstance>();
        }


        /// <summary>
        /// <para>Инициализация группы данными</para>
        /// <para>Собираем по имени дозы сортируя по уровням сверху вниз</para>
        /// <para>Elements[0] → самый верхний (с максимальной высотой уровня),</para>
        /// <para>Elements[^1] (последний) → самый нижний.</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="elements"></param>
        public void Initialize(string name, IEnumerable<FamilyInstance> elements)
        {
            LevelAnyObject levelAnyObject = new(_doc);
            Name = name;
            Elements = elements
                .OrderByDescending(fi => levelAnyObject.GetLevel(fi) != null ? levelAnyObject.GetLevel(fi).Elevation : double.MinValue)  // сортируем по уровням сверху вниз (по убыванию)
                .ToList();
        }

        /// <summary>
        /// Получить верхний элемент группы (группы доз)
        /// </summary>
        /// <returns></returns>
        public FamilyInstance GetTopElement() => Elements.FirstOrDefault();


        /// <summary>
        /// Получить нижний элемент группы (группы доз)
        /// </summary>
        /// <returns></returns>
        public FamilyInstance GetBottomElement() => Elements.LastOrDefault();
    }
}
