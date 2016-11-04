using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;

namespace Getequ.CodeStinck.Models
{
    class ControllerStinck
    {
        public string Name;
        public string BaseType;
        //public CodeClass Class;
        public CodeClassWrapper Class;
        public ProjectItem ProjectItem;

        public List<CodeFunctionWrapper> BadPublicMethods = new List<CodeFunctionWrapper>();
        public List<CodeFunctionWrapper> ViewModelMethods = new List<CodeFunctionWrapper>();
        public List<CodeFunctionWrapper> ServiceMethods = new List<CodeFunctionWrapper>();

        public IEnumerable<CodeFunctionWrapper> DomainServiceMethods { get { return BadPublicMethods.Where(x => x.Name == "Update" || x.Name == "Create" || x.Name == "Delete"); } }

        public List<CodePropertyWrapper> NonServiceProps = new List<CodePropertyWrapper>();
        public List<CodeFunctionWrapper> NonPublicMethods = new List<CodeFunctionWrapper>();
        public List<CodeClassWrapper> NestedClasses = new List<CodeClassWrapper>();

        public override string ToString()
        {
            return Name + string.Format(" {0} {1} {2} {3} = {4}", 
                BadPublicMethods.Count, NonPublicMethods.Count, NonServiceProps.Count, NestedClasses.Count, 
                BadPublicMethods.Count + NonPublicMethods.Count + NonServiceProps.Count + NestedClasses.Count);
        }
    }
}
