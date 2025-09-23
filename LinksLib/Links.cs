using Autodesk.Revit.DB;
using Libraries.ErrorModelLib;
using Libraries.LinksLib.UserWarningLinksLib;
using System.Collections.Generic;

namespace Libraries.LinksLib
{
    public class Links(Document doc)
    {
        private readonly Document _doc = doc;
        private ICollection<string> _instancesNames = null;
        private Dictionary<string, RevitLinkInstance> _linkInstancesMap = null;

        public ICollection<string> GetLinksNames()
        {
            if (_instancesNames == null)
                CollectLinkData();

            return _instancesNames;
        }

        public RevitLinkInstance GetLinkByName(string linkName)
        {
            if (_linkInstancesMap == null)
                CollectLinkData();

            _linkInstancesMap.TryGetValue(linkName, out var linkInstance);
            return linkInstance;
        }

        private void CollectLinkData()
        {
            _instancesNames = new List<string>();
            _linkInstancesMap = new Dictionary<string, RevitLinkInstance>();

            var collector = new FilteredElementCollector(_doc).OfClass(typeof(RevitLinkInstance));
            foreach (var elem in collector)
            {
                if (elem is RevitLinkInstance inst &&
                    _doc.GetElement(inst.GetTypeId()) is RevitLinkType typeLink &&
                    RevitLinkType.IsLoaded(_doc, typeLink.Id))
                {
                    var linkName = inst.Name;
                    _instancesNames.Add(linkName);

                    if (!_linkInstancesMap.ContainsKey(linkName))
                    {
                        _linkInstancesMap[linkName] = inst;
                    }
                }
            }

            if (_instancesNames.Count == 0)
            {
                ErrorModel errorModel = new();
                errorModel.UserWarning(new NoLinks().MessageForUser());
            }
        }
    }
}
