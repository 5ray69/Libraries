namespace Libraries.ElectricsLib.UserWarningElectricsLib
{
    public class NotSelectedByUser
    {
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
