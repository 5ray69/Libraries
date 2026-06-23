namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    /// <summary>
    /// Пользователь не сделал выбор
    /// </summary>
    public class NotSelectedByUser
    {
        /// <summary>
        /// Сообщение пользователю
        /// </summary>
        /// <param name="nameGroup"></param>
        /// <returns></returns>
        public string MessageForUser(string nameGroup)
        {
            string message = $@"
Пользователь не сделал выбор,
например, для группы {nameGroup}

Если для этой группы не должен строиться короб,
на этом этаже, то выберите 'нет'.
Иначе выберите соответсвующий группе короб.
Запустите код заново и сделайте выбор.
";

            return message;
        }
    }
}
