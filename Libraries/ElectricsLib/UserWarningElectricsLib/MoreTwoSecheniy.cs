namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// БОЛЬШЕ ДВУХ различных значений В ОДНОЙ ГРУППЕ
    /// </summary>
    public class MoreTwoSecheniy
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="nameGroup"></param>
        /// <returns></returns>
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
