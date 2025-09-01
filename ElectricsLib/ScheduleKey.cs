using Autodesk.Revit.DB;
using ElectricsLib.MyDll.UserWarningStrings;
using ElectricsLib.UserWarningElectricsLib;
using ElectricsLib.UserWarningStrings;
using ErrorModelLib;
using System.Collections.Generic;
using System.Linq;

namespace ElectricsLib
{
    public class ScheduleKey(Document doc, string nameSchedule)
    {
        private readonly Document _doc = doc;
        private readonly string _nameSchedule = nameSchedule;

        private ViewSchedule _viewSchedule;
        private ICollection<Element> _elementsKey;
        private ICollection<string> _nameElements;

        private readonly ErrorModel _errorModel = new();


        /// <summary>
        /// объект ключевой спецификации
        /// </summary>
        /// <param name="nameSchedule"></param>
        /// <returns></returns>
        public ViewSchedule GetView()
        {
            if (_viewSchedule == null)
            {
                _viewSchedule = new FilteredElementCollector(_doc)
                                    .OfClass(typeof(ViewSchedule))
                                    .Cast<ViewSchedule>()
                                    .FirstOrDefault(vs => vs.Name == _nameSchedule);
                if (_viewSchedule == null)
                {
                    _errorModel.UserWarning(new NoScheduleKey().MessageForUser(_nameSchedule));
                }
            }
            return _viewSchedule;
        }

        /// <summary>
        /// ключевые элементы спецификации
        /// </summary>
        /// <returns></returns>
        public ICollection<Element> ElementsKey()
        {
            if (_elementsKey == null)
            {
                _elementsKey = [];
                FilteredElementCollector collectorSchedule = new(_doc, GetView().Id);
                _elementsKey = collectorSchedule.ToElements();
            }
            return _elementsKey;
        }

        /// <summary>
        /// имена ключевых элементов в виде строк
        /// </summary>
        /// <returns></returns>
        public ICollection<string> NameElements()
        {
            if (_nameElements == null)
            {
                _nameElements = ElementsKey().Select(elem => elem.Name).ToList();
            }
            return _nameElements;
        }

        /// <summary>
        /// Id элемента ключевой спецификации по имени
        /// </summary>
        /// <param name="nameElement"></param>
        /// <returns></returns>
        public ElementId GetIdElementKey(string nameElement)
        {
            Element elem = ElementsKey().FirstOrDefault(e => e.Name == nameElement);
            if (elem == null)
            {
                _errorModel.UserWarning(new NoElementKey().MessageForUser(nameElement, _nameSchedule));
            }
            return elem.Id;
        }



        /// <summary>
        /// имя ключевой спецификации
        /// </summary>
        /// <returns></returns>
        public string GetNameSchedule()
        {
            return _nameSchedule;
        }


        /// <summary>
        /// Имя элемента ключевой спецификации по Id
        /// </summary>
        /// <param name="elementId"></param>
        /// <returns></returns>
        public string GetNameElementKey(ElementId elementId)
        {
            Element element = ElementsKey().FirstOrDefault(el => el.Id == elementId);
            if (element == null)
            {
                _errorModel.UserWarning(new NoElementKeyId().MessageForUser(elementId, _nameSchedule));
            }
            return element.Name;
        }
    }
}

