using System;
using System.Collections.Generic;
using System.Linq;

using EnvDTE;
using EnvDTE80;

namespace Getequ.CodeStinck.Domain
{
    class NamespaceFinder
    {
        Solution solution;

        public NamespaceFinder(Solution solution)
        {
            this.solution = solution;
        }

        public IEnumerable<NamespaceItem> FindSpaces()
        {
            var result = new List<NamespaceItem>();

            var classes = new Dictionary<string, CodeClass2>();
            var ifaces = new Dictionary<string, CodeInterface2>();

            solution.GetSolutionCodeElements(classes, ifaces, "Bars");

            foreach (CodeClass2 @class in classes.Values)
            {
                var item = new NamespaceItem
                {
                    Class = @class.Name,
                    Namespace = @class.Namespace.FullName,
                    Project = @class.ProjectItem.ContainingProject.Name
                };
                result.Add(item);
            }

            foreach (CodeInterface2 iface in ifaces.Values)
            {
                var item = new NamespaceItem
                {
                    Class = iface.Name,
                    Namespace = iface.Namespace.FullName,
                    Project = iface.ProjectItem.ContainingProject.Name
                };
                result.Add(item);
            }

            return result;
        }
    }

    class NamespaceItem
    {
        public string Namespace;
        public string Class;
        public string Project;
    }
}
