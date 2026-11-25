using Autodesk.Revit.DB;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Libraries.LevelsLib
{
    /// <summary>
    /// Уровени в виде double, для сравнения отметок элементов с отметками уровней
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="zElement"></param>
    public class ElevationDouble(Document doc, double zElement)
    {
        /// <summary>
        /// Над каким уровнем высотная отметка объекта в виде double
        /// </summary>
        /// <returns></returns>
        public Level AboveLevel()
        {
            // Получаем уровни из кэша вместе с уже округлёнными отметками
            List<CachedLevelInfo> levels = LevelCache.GetSortedLevelInfos(doc);

            if (levels.Count == 0)
            {
                string message = $@"
В активном документе отсутствуют уровни.

Или активный документ без уровней,
или вы не создали уровни в проекте.

После исправления ошибки
запустите код заново.";

                TaskDialog.Show("Ошибка", message);
                return null;
            }

            // Округляем отметку элемента ТОЧНО ТАК ЖЕ, как округлялись отметки уровней в LevelCache.
            // Это убирает микропогрешности double (на уровне 1e-15) при совпадении высот.
            double roundedZElement = Math.Round(
                                                zElement,
                                                LevelCache.ElevationPrecisionDigits,
                                                MidpointRounding.AwayFromZero
                                                );


            // Самый нижний уровень
            CachedLevelInfo lowLevelInfo = levels[0];

            // Проверка, если элемент ниже самого нижнего уровня
            if (roundedZElement < lowLevelInfo.RoundedProjectElevation)
            {
                return lowLevelInfo.Level;
            }

            // Так как уровни отсортированы по возрастанию, идём с конца (от верхних уровней к нижним).
            // Первый же уровень, чья отметка <= отметки элемента, и есть искомый (включая точное совпадение).
            for (int i = levels.Count - 1; i >= 0; i--)
            {
                if (levels[i].RoundedProjectElevation <= roundedZElement)
                {
                    return levels[i].Level;
                }
            }

            return lowLevelInfo.Level;
        }
    }
}


//ElevationDouble стал readonly struct: При создании new ElevationDouble(_doc, zElement)
//в цикле вообще не выделяется память в куче (Heap) — сборщик мусора (GC) не нагружается.
//using Autodesk.Revit.DB;

//namespace Libraries.LevelsLib
//{
//    public readonly struct ElevationDouble(Document doc, double zElement)
//    {
//        public Level AboveLevel()
//        {
//            List<CachedLevelInfo> levels = LevelCache.GetSortedLevels(doc);
//            if (levels.Count == 0)
//                return null;

//            // Если элемент ниже самого нижнего уровня
//            if (zElement < levels[0].RoundedProjectElevation)
//                return levels[0].Level;

//            Level closestLevel = levels[0].Level;

//            // Так как список уже отсортирован по возрастанию, идем снизу вверх,
//            // пока отметка уровня <= отметки элемента
//            for (int i = 0; i < levels.Count; i++)
//            {
//                if (levels[i].RoundedProjectElevation <= zElement)
//                {
//                    closestLevel = levels[i].Level;
//                }
//                else
//                {
//                    break; // Дальше уровни только выше, проверять нет смысла
//                }
//            }

//            return closestLevel;
//        }
//    }
//}


