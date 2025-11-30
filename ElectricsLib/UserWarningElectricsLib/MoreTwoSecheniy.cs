namespace CalculationGroups.MyDll.UserWarningCalculationGroups
{
    public class MoreTwoSecheniy
    {
        public string MessageForUser(string nameGroup)
        {
            string message = $@"
Внимание!
БОЛЬШЕ ДВУХ различных значений В ОДНОЙ ГРУППЕ
у электрических цпей в параметре Тип кабеля.
Состояние цепей, можно смотреть в специцикации 'Инф.Группы'. 
Смотрите цепи у котороых 'БУДОВА_Группа' {nameGroup}";
            return message;
        }
    }
}
