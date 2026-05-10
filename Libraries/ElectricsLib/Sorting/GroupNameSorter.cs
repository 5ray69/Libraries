using System;
using System.Collections.Generic;
using System.Linq;

namespace Libraries.ElectricsLib.Sorting
{


    /// <summary>
    /// Сонтировка имён групп
    /// </summary>
    public class GroupNameSorter
    {
        /// <summary>
        /// <para> Сортирует список групп (например, "гр.1", "гр.2А", "гр.А", "гр.блок контроля" и т.п.) </para>
        /// <para> в "человеческом" порядке: сначала гр.1А, гр.12А, ..., затем гр.1, гр.3, ..., потом текстовые. </para>
        /// <para> Сортировка в порядке размещения семейств групп освещения на принципиальной схеме </para>
        /// </summary>
        public List<string> SortLightingGroupsInShem(List<string> listNamesGroup)
        {
            var groupAInt = new List<int>();
            var groupNoAInt = new List<int>();
            var groupAStrings = new List<string>();
            var groupNoAStrings = new List<string>();

            foreach (string stri in listNamesGroup)
            {
                if (stri == null) continue;

                //русская А или английская A
                //IndexOf('A') — находит индекс первого вхождения буквы A
                //LastIndexOf('A') — находит индекс последнего вхождения
                //если оба индекса равны и не равны - 1, значит в строке ровно одна такая буква
                //IndexOf - более быстрый способ, чем bool hasA = (stri.Count(ch => ch == 'A' || ch == 'А') <= 1) не более чем одна буква A или А
                bool hasA =
                        ((stri.IndexOf('А') != -1 && stri.IndexOf('А') == stri.LastIndexOf('А')) ||
                         (stri.IndexOf('A') != -1 && stri.IndexOf('A') == stri.LastIndexOf('A')));
                var digits = new List<char>();

                foreach (char ch in stri)
                {
                    if (char.IsDigit(ch))
                        digits.Add(ch);
                }

                if (digits.Count > 0)  //если список цифр не пуст
                {
                    int number = int.Parse(new string(digits.ToArray()));
                    if (hasA)
                        groupAInt.Add(number);
                    else
                        groupNoAInt.Add(number);
                }
                else
                {
                    // если цифр нет, берём хвост после "гр."
                    string tail = stri.Length > 3 ? stri.Substring(3) : string.Empty;
                    if (hasA)
                        groupAStrings.Add(tail);
                    else
                        groupNoAStrings.Add(tail);
                }
            }

            var sortList = new List<string>();

            // Сначала "гр." с "А" — сначала числа, потом строки
            // OrderBy(x => x) - сортировака по возрастанию чисел, натуральный (человеческий) порядок
            foreach (int n in groupAInt.OrderBy(x => x))
                sortList.Add($"гр.{n}А");  // в отсортированный список чисел добавляем "гр." + число + "А"

            foreach (string s in groupAStrings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                sortList.Add($"гр.{s}А");  // в отсортированный список чисел добавляем "гр." + число + "А"

            // Затем обычные "гр." без "А" — сначала числа, потом строки
            foreach (int n in groupNoAInt.OrderBy(x => x))
                sortList.Add($"гр.{n}");

            foreach (string s in groupNoAStrings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
                sortList.Add($"гр.{s}");

            return sortList;
        }


    }
}
