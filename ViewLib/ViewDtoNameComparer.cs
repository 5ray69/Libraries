using System;
using System.Collections;
using System.Collections.Generic;

namespace Libraries.ViewLib
{
    public class ViewDtoNameComparer : IComparer, IComparer<ViewDto>
    {
        /// <summary>
        /// Основной метод сравнения двух ViewDto
        /// </summary>
        public int Compare(ViewDto x, ViewDto y)
        {
            // Если оба объекта отсутствуют — считаем равными
            if (x == null && y == null)
                return 0;

            // null считаем "больше", чтобы он ушёл в конец списка
            if (x == null)
                return 1;

            if (y == null)
                return -1;

            // Сравниваем именно имена видов
            return CompareNames(x.Name, y.Name);
        }

        /// <summary>
        /// Необобщённый метод Compare
        /// Именно его вызывает WPF при CustomSort
        /// </summary>
        public int Compare(object x, object y)
        {
            // Приводим object к ViewDto и вызываем основной метод
            return Compare(x as ViewDto, y as ViewDto);
        }

        /// <summary>
        /// Сравнение двух строк — имён видов
        /// </summary>
        private int CompareNames(string x, string y)
        {
            // Разбираем оба имени на логические части
            ParsedName px = ParseName(x);
            ParsedName py = ParseName(y);

            // ===== 1. Сравнение по приоритету группы =====
            // LU (0) → L (1) → LR (2)
            //Строки содержащие символы LU - первые в списке
            //Строки содержащие символы L - вторые в списке
            //Строки содержащие символы LR - третьи в списке
            int result = px.Priority.CompareTo(py.Priority);
            if (result != 0)
                return result;

            // ===== 2. Сравнение по числу после префикса =====
            //сортировка после LU, L, LR по числам в порядке возрастания
            result = px.Number.CompareTo(py.Number);
            if (result != 0)
                return result;

            // ===== 3. Сравнение по оставшейся части имени =====
            return string.Compare(
                px.Tail,
                py.Tail,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Разбор имени вида на составные части
        /// </summary>
        private ParsedName ParseName(string name)
        {
            // Защита от null
            name ??= string.Empty;

            // Ищем позиции обязательных сочетаний символов
            int posLU = name.IndexOf("LU", StringComparison.Ordinal);
            int posLR = name.IndexOf("LR", StringComparison.Ordinal);
            int posL = name.IndexOf("L", StringComparison.Ordinal);

            // Инициализируем "лучшую" позицию большим числом
            int bestPos = int.MaxValue;
            string prefix = null;
            int priority = int.MaxValue;

            //Важно не менять порядок проверок, тк L содержится в LU и LR
            // Проверяем LU
            if (posLU >= 0 && posLU < bestPos)
            {
                bestPos = posLU;
                prefix = "LU";
                priority = 0;
            }

            // Проверяем LR
            if (posLR >= 0 && posLR < bestPos)
            {
                bestPos = posLR;
                prefix = "LR";
                priority = 2;
            }

            // Проверяем L
            if (posL >= 0 && posL < bestPos)
            {
                bestPos = posL;
                prefix = "L";
                priority = 1;
            }

            // Если ни один маркер не найден
            if (prefix == null)
            {
                return new ParsedName
                {
                    Priority = int.MaxValue,
                    Number = int.MaxValue,
                    Tail = name
                };
            }

            // Позиция, где начинается число (сразу после LU / LR / L)
            int index = bestPos + prefix.Length;

            // Читаем число после маркера
            string numberText = string.Empty;

            while (index < name.Length && char.IsDigit(name[index]))
            {
                numberText += name[index];
                index++;
            }

            int number = numberText.Length > 0
                ? int.Parse(numberText)
                : int.MaxValue;

            // Хвост — всё, что осталось после числа
            string tail = name.Substring(index);

            return new ParsedName
            {
                Priority = priority,
                Number = number,
                Tail = tail
            };
        }

        /// <summary>
        /// Вспомогательная структура —
        /// результат разбора имени вида
        /// </summary>
        private struct ParsedName
        {
            // Приоритет группы (LU / L / LR)
            public int Priority;

            // Число после префикса
            public int Number;

            // Остальная часть имени (например "-Освещение")
            public string Tail;
        }
    }
}
