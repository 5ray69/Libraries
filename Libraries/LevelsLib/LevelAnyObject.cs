using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Libraries.ElectricsLib.UserWarningElectricsLib;
using Libraries.ErrorModelLib;

namespace Libraries.LevelsLib
{
    /// <summary>
    /// Извлекает уровень любого объекта электрика
    /// </summary>
    /// <param name="doc"></param>
    public class LevelAnyObject(Document doc)
    {
        private static readonly ElementId _detailComponentsId = new(BuiltInCategory.OST_DetailComponents);
        private static readonly ElementId _genericAnnotationId = new(BuiltInCategory.OST_GenericAnnotation);

        private readonly Document _doc = doc;


        /// <summary>
        /// <para>извлекает уровень, на котором находится любой объект электрика,</para>
        /// <para>включая семейства на грани (не привязанные к уровню) и аннотации</para>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Level GetLevel(Element element)
        {
            //НЕЛЬЗЯ МЕНЯТЬ ПОРЯДОК СЛЕДОВАНИЯ УСЛОВИЙ if
            //некоторые if подпадают под условия друг друга (семейства имеющие Host и HostFace)

            ElementId levelId = element.LevelId;

            // Проверка на наличие свойства LevelId и извлечение его значения
            //familyinstance, соед.детали кабельных лотков, соед.детали коробов
            // LevelId у тех, у кого семейство НЕ на основе рабочей плоскости/НЕ на грани
            //if (element.LevelId != null && element.LevelId.IntegerValue != -1 )
            if (levelId is not null && levelId != ElementId.InvalidElementId)
            {
                return _doc.GetElement(levelId) as Level;
            }



            // HostFace is null у тех семейств, у которых рабочая плоскость это уровень, Размещены на уровне, а не на грани
            // Светильники, выключатели, розетки... и здесь полоса заземления, которая размещена на уровне
            if (
                element is FamilyInstance familyInstance
                && familyInstance.HostFace is null
                && familyInstance.Host is not null
                && element.Category is not null
                && element.Category.Id != _detailComponentsId         // элементы узлов
                && element.Category.Id != _genericAnnotationId        // типовые аннотации (стрелка выносок)
                )
            {
                return familyInstance.Host as Level;
            }


            // короба свойство ReferenceLevel
            if (element is Conduit conduit && conduit.ReferenceLevel is Level refLevel)
            {
                return refLevel;
            }


            // кабельные лотки свойство ReferenceLevel
            if (element is CableTray cableTray && cableTray.ReferenceLevel is Level referenceLevel)
            {
                return referenceLevel;
            }


            if (element is ElectricalSystem electricalSystem)
            {
                FamilyInstance baseEquipment = electricalSystem.BaseEquipment;
                if (baseEquipment is null) // цепь не подключена "код завершил работу" с предупреждением пользователю
                {
                    ErrorModel errorModel = new();
                    errorModel.UserWarning(new NoConnectCircuit().MessageForUser(electricalSystem));
                    return null;
                }

                // Делегируем определение уровня самому щиту: сработает и для щита на уровне,
                // и для щита на рабочей плоскости, и для щита на грани стены (HostFace != null)
                return GetLevel(baseEquipment);

                // Единая логика для щита и обычных семейств: baseEquipment имеет тип FamilyInstance.
                // Когда он попадает в GetLevel(baseEquipment), код последовательно проверяет:
                // - привязку через LevelId;
                // - размещение на уровне(HostFace is null && Host is Level);
                // - размещение на грани(HostFace: Reference) — и автоматически вызывает GetСlosestLevel(baseEquipment) по его нижней точке.
                // Нет риска бесконечной рекурсии: electricalSystem.BaseEquipment возвращает именно FamilyInstance(оборудование),
                // а не другую ElectricalSystem, поэтому повторного входа в ветку if (element is ElectricalSystem) не произойдёт.
                // Меньше дублирования кода: больше не нужно отдельно поддерживать копию проверок baseEquipment.LevelId
                // и baseEquipment.Host внутри ветки электрической цепи:

                //ElementId baseLevelId = baseEquipment.LevelId;
                //if (baseLevelId is not null && baseLevelId != ElementId.InvalidElementId)
                //    return _doc.GetElement(baseLevelId) as Level;
                //if (baseEquipment?.Host is Level baseHostLevel)
                //    return baseHostLevel;
            }


            // полоса заземления и другие семейства, размещенные на грани
            // если element это FamilyInstance и его свойство HostFace возвращает тип Reference (не null) - если возвращает тип Reference значит размещено на грани
            // синтаксис if использует шаблоны с условиями C# (8.0 и выше)
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/patterns#property-patterns
            if (element is FamilyInstance { HostFace: Reference })
            {
                return GetСlosestLevel(element);
            }


            // Проверка на наличие OwnerViewId, принадлежности виду и тем, что OwnerView это ViewPlan исключаем возможность,
            // что аннотация не находится на ViewDrafting, ViewSection или ViewSheet

            // текстовые примечания, линии детализации, марки электрооборудования, марки осветительных приборов, марки помещений
            // марки нескольких категорий, марки коробов, типовые аннотации (стрелка выносок), элементы узлов, размеры
            if (element.OwnerViewId != null && _doc.GetElement(element.OwnerViewId) is ViewPlan ownerView)
            {
                return ownerView.GenLevel;
            }

            return null;
        }


        /// <summary>
        /// <para>над каким уровнем находится элемент</para>
        /// <para>у вертикальных объектов берется нижняя точка</para>
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>

        public Level GetСlosestLevel(Element element)
        {
            double zElement = 0;

            if (element.Location is LocationCurve locationCurve)
            {
                // нижняя точка линии, та у которой значение высотной отметки меньше
                Curve curve = locationCurve.Curve;
                double minZ = Math.Round(
                                        Math.Min(curve.GetEndPoint(0).Z, curve.GetEndPoint(1).Z),
                                        3, MidpointRounding.AwayFromZero
                                        );
                zElement = minZ;
            }
            else if (element.Location is LocationPoint locationPoint)
            {
                double zPoint = Math.Round(locationPoint.Point.Z, 3, MidpointRounding.AwayFromZero);
                zElement = zPoint;
            }

            return new ElevationDouble(_doc, zElement).AboveLevel();
        }
    }
}
