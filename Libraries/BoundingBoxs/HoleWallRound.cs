using Autodesk.Revit.DB;
using Libraries.BoundingBoxs.InterfaceBoundingBoxs;
using Libraries.BoundingBoxs.UserWarningBoundingBoxs;
using Libraries.ErrorModelLib;
using Libraries.ParametersLib;

namespace Libraries.BoundingBoxs
{
    /// <summary>
    /// <para> Обработчик круглого отверстия в стене </para>
    /// <para> для получения баундингбокса по видимой геометрии </para>
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="errorModel"></param>
    public class HoleWallRound(Document doc, ErrorModel errorModel) : IHoleHandler
    {
        private readonly Document _doc = doc;
        private readonly ErrorModel _errorModel = errorModel;

        /// <summary>
        /// Изменяет размер баундингбокска по видимой геометрии
        /// </summary>
        /// <param name="familyInstance"></param>
        /// <param name="distance"></param>
        /// <param name="linkTransform"></param>
        /// <returns></returns>
        public BoundingBoxXYZ ResizeBoundingBox(FamilyInstance familyInstance, double distance, Transform linkTransform = null)
        {
            if (familyInstance.Location is not LocationPoint location)
            {
                // уведомляем пользователя и завершаем код
                _errorModel.UserWarning(new FamilyInstanceNoLocation(_doc, _errorModel).MessageForUser(familyInstance));
                return null;
            };

            XYZ center = location.Point;

            // Если параметра нет в семействе то null, и вместо  null присваиваем 0.0
            double depth = familyInstance.get_Parameter(ParameterGuids.Depth)?.AsDouble() ?? 0.0;  // БУДОВА_Глубина
            double diameter = familyInstance.get_Parameter(ParameterGuids.Diameter)?.AsDouble() ?? 0.0;  // БУДОВА_Диаметр

            // HandOrientation — локальная ось X семейства (вправо/влево)
            XYZ hand = familyInstance.HandOrientation.Normalize();
            
            // FacingOrientation — локальная ось Y семейства (вперед/назад, перпендикулярно стене)
            XYZ facing = familyInstance.FacingOrientation.Normalize(); 

            // Вычисляем вертикальный вектор (локальная ось Z) через векторное произведение
            XYZ up = hand.CrossProduct(facing).Normalize();  

            // Защита: если вдруг векторы выродились, подстрахуемся мировой вертикалью
            if (up.IsZeroLength())
            {
                up = XYZ.BasisZ;
            }

            // Получаем матрицу трансформации имеющуюся у семейства, а затем применим ее к баундингбоксу,
            // который создаем в точке (0,0,0) как в редакторе семейств.
            // В итоге получаем баундингбокс в мировых координатах.
            Transform localTransform = Transform.Identity;  // Создаем пустую (единичную) матрицу
            localTransform.Origin = center;  // Перенесли из (0,0,0) в точку вставки экземпляра
            localTransform.BasisX = hand;  // Повернули ось X вдоль стены
            localTransform.BasisY = facing;  // Повернули ось Y перпендикулярно стене
            localTransform.BasisZ = up;  // Повернули ось Z вверх

            // КЛЮЧЕВОЙ МОМЕНТ: если передан трансформ связи, переводим матрицу в координаты основного файла
            if (linkTransform != null && !linkTransform.IsIdentity)
            {
                // Умножаем матрицу связи на локальную матрицу экземпляра
                localTransform = linkTransform.Multiply(localTransform);
            }

            // Задаем локальные Min и Max (В ЛОКАЛЬНЫХ координатах относительно центра)
            // в точке (0,0,0) как в редакторе семейств
            double halfD = depth / 2.0;  // половина глубины
            double halfW = diameter / 2.0; // половина ширины
            double halfH = halfW;  // половина высоты
            // Переводим все 8 точек в глобальные мировые координаты с помощью Transform
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

            // Локальная функция для обновления минимумов и максимумов одной точкой.
            // Компилятор C# оптимизирует её так, что она работает молниеносно.
            // Скорость ее выполнения равна нулю, в то время как на создание массив
            // получаем тормоза на создание объекта и удаление его сборщиком мусора.
            // Всесто создания массива используем локальную функцию.
            void ProcessCorner(double x, double y, double z)
            {
                // трансформация переносит точки из точки (0, 0, 0) в точку размещения экземпляра,
                // а также поворачивает их в соответствии с ориентацией семейства
                XYZ worldCorner = localTransform.OfPoint(new XYZ(x, y, z));

                // Код спрашивает: «Координата X текущего угла меньше, чем минимальная из тех, что мы уже нашли до этого ?»
                // Если да(worldCorner.X < minX), то мы обновляем minX новым, еще более меньшим значением. Если нет — оставляем как есть.
                if (worldCorner.X < minX) minX = worldCorner.X;
                if (worldCorner.Y < minY) minY = worldCorner.Y;
                if (worldCorner.Z < minZ) minZ = worldCorner.Z;

                if (worldCorner.X > maxX) maxX = worldCorner.X;
                if (worldCorner.Y > maxY) maxY = worldCorner.Y;
                if (worldCorner.Z > maxZ) maxZ = worldCorner.Z;
            }

            // Поочередно проверяются 8 углов параллелепипеда от низких к высоким.
            // Точка размещения семействакруглого отверстия в стене расположена в центре цилиндра.
            ProcessCorner(-halfW, -halfD, -halfH); // левый,  передний, нижний
            ProcessCorner( halfW, -halfD, -halfH); // правый, передний, нижний
            ProcessCorner(-halfW,  halfD, -halfH); // левый,  задний,   нижний
            ProcessCorner( halfW,  halfD, -halfH); // правый, задний,   нижний

            ProcessCorner(-halfW, -halfD, halfH);  // левый,  передний, верхний
            ProcessCorner( halfW, -halfD, halfH);  // правый, передний, верхний
            ProcessCorner(-halfW,  halfD, halfH);  // левый,  задний,   верхний
            ProcessCorner( halfW,  halfD, halfH);  // правый, задний,   верхний


            // Создаем финальный в мировых координатах (координатах основногофайла) BoundingBoxXYZ (без кастомного Transform, чистый AABB)
            BoundingBoxXYZ worldBbox = new BoundingBoxXYZ
            {
                Transform = Transform.Identity, // Важно: единичная матрица, чтобы Revit не игнорировал бокс
                Min = new XYZ(minX - distance, minY - distance, minZ - distance),
                Max = new XYZ(maxX + distance, maxY + distance, maxZ + distance)
            };

            return worldBbox;
        }
    }
}

//BoundingBoxXYZ bbox = familyInstance.get_BoundingBox(null);
//if (bbox == null)
//{
//    // уведомляем пользователя и завершаем код
//    _errorModel.UserWarning(new FamilyInstanceNoLocation(_doc, _errorModel).MessageForUser(familyInstance));
//    return null;
//};
