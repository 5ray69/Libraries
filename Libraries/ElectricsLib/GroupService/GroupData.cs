namespace Libraries.ElectricsLib.GroupService
{

    /// <summary>
    /// Величины группы цепей
    /// </summary>
    public class GroupData
    {
        /// <summary>
        /// Активная мощность группы
        /// </summary>
        public double ActivePower
        {
            get; set;
        }

        /// <summary>
        /// CosF группы
        /// </summary>
        public double CosF
        {
            get; set;
        }

        /// <summary>
        /// Полная мощность группы
        /// </summary>
        public double FullPower
        {
            get; set;
        }
    }
}
