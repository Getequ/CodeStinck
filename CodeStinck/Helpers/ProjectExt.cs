using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

using EnvDTE;
using EnvDTE80;
using VSLangProj;

namespace Getequ
{
    public static class ProjectExt
    {
        public static bool HasReference(this Project project, string referenceName)
        {
            var vsproject = project.Object as VSProject;

            foreach (Reference reference in vsproject.References)
            {
                if (reference.Name == referenceName)
                    return true;
            }

            return false;
        }

        public static string RootFolder(this Project project)
        {
            Property prop = null;

            try
            {
                prop = project.Properties.Item("FullPath");
            }
            catch (ArgumentException)
            {
                try
                {
                    prop = project.Properties.Item("ProjectDirectory");
                }
                catch (ArgumentException)
                {
                    prop = project.Properties.Item("ProjectPath");
                }
            }

            if (prop != null)
            {
                string value = prop.Value.ToString();

                if (File.Exists(value))
                {
                    return Path.GetDirectoryName(value);
                }
                else if (Directory.Exists(value))
                {
                    return value;
                }

                throw new ArgumentException("Не найден путь проекта " + project.Name);
            }
            else
            {
                throw new ArgumentException("Не найден путь проекта " + project.Name);
            }
        }

        public static string DefaultNamespace(this Project project)
        {
            return project.Properties.Item("DefaultNamespace").Value.ToString();
        }

        public static void GetSolutionCodeElements(this Solution solution, IDictionary<string, CodeClass2> classes, IDictionary<string, CodeInterface2> interfaces, string filter, IEnumerable<string> nameFilter = null)
        {
            if (classes != null)
            {
                classes.Clear();
            }

            if (interfaces != null)
            {
                interfaces.Clear();
            }

            var allProjects = new List<Project>();
            foreach (Project proj in solution.Projects)
            { 
                if (proj.Object is VSProject)
                    allProjects.Add(proj);
                else
                if (proj.ProjectItems != null)
                {
                    foreach (ProjectItem subProj in proj.ProjectItems)
                    {
                        if (subProj.SubProject != null && subProj.SubProject.Object is VSProject)
                            allProjects.Add(subProj.SubProject);
                    }
                }
            }

            foreach (Project project in allProjects)
            {
                var pcl = classes != null ? new Dictionary<string, CodeClass2>() : null;
                var pil = interfaces  != null ? new Dictionary<string, CodeInterface2>() : null;
                GetProjectCodeElements(project, pcl, pil, filter, nameFilter);

                if (classes != null)
                {
                    foreach (var pair in pcl)
                        classes.Add(pair.Key, pair.Value);
                }

                if (interfaces != null)
                {
                    foreach (var pair in pil)
                        interfaces.Add(pair.Key, pair.Value);
                }
            }
        }

        public static void GetProjectCodeElements(this Project project, IDictionary<string, CodeClass2> classes, IDictionary<string, CodeInterface2> interfaces, string filter, IEnumerable<string> nameFilter = null)
        {
            if (classes != null)
            {
                classes.Clear();
            }

            if (interfaces != null)
            {
                interfaces.Clear();
            }

            Action<CodeNamespace> enumClasses = null;
            enumClasses = (ns) =>
            {
                foreach (CodeNamespace ens in ns.Members.OfType<CodeNamespace>())
                {
                    enumClasses(ens);
                }

                if (classes != null)
                {
                    foreach (CodeClass2 @class in ns.Members.OfType<CodeClass2>())
                    {
                        try // skip partial
                        {
                            if (nameFilter != null)
                            {
                                if (nameFilter.Any(@class.Name.Contains) || !nameFilter.Any())
                                    classes.Add(@class.FullName, @class);
                            }
                            else
                                classes.Add(@class.FullName, @class);
                        }
                        catch
                        {}
                    }
                }

                if (interfaces != null)
                {
                    foreach (CodeInterface2 @interface in ns.Members.OfType<CodeInterface2>())
                    {
                        if (nameFilter != null)
                        {
                            if (nameFilter.Any(@interface.Name.Contains) || !nameFilter.Any())
                                interfaces.Add(@interface.FullName, @interface);
                        }
                        else
                            interfaces.Add(@interface.FullName, @interface);
                    }
                }
            };

            Action<ProjectItems> enumItems = null;
            enumItems = (items) =>
            {
                foreach (ProjectItem pi in items)
                {
                    if (pi.Name.EndsWith(".cs") && pi.FileCodeModel != null)
                    {
                        foreach (CodeNamespace element in pi.FileCodeModel.CodeElements.OfType<CodeNamespace>())
                        {
                            enumClasses(element);
                        }
                    }
                    else
                    {
                        if (pi.ProjectItems != null)
                            enumItems(pi.ProjectItems);
                    }
                }
            };

            enumItems(project.ProjectItems);
        }

        public static void GetCodeElements(this Project project, IDictionary<string, CodeClass2> classes, IDictionary<string, CodeInterface2> interfaces, string filter, IEnumerable<string> nameFilter)
        {
            if (classes != null)
            {
                classes.Clear();
            }

            if (interfaces != null)
            {
                interfaces.Clear();
            }

            Action<CodeNamespace, int> enumClasses = null;
            enumClasses = (ns, level) =>
            {
                var nsList = filter.Split('.').ToList();
                var nsFilter = level < nsList.Count ? string.Join(".", nsList.Take(level)) : filter;

                foreach (CodeNamespace ens in ns.Members.OfType<CodeNamespace>().Where(x => x.FullName.StartsWith(nsFilter)))
                {
                    enumClasses(ens, level + 1);
                }

                if (classes != null)
                {
                    foreach (CodeClass2 @class in ns.Members.OfType<CodeClass2>())
                    {
                        if (nameFilter != null)
                        {
                            if (nameFilter.Any(@class.Name.Contains) || !nameFilter.Any() )
                                classes.Add(@class.FullName.Substring(filter.Length + 1), @class);
                        }
                        else
                            classes.Add(@class.FullName.Substring(filter.Length + 1), @class);
                    }
                }

                if (interfaces != null)
                {
                    foreach (CodeInterface2 @interface in ns.Members.OfType<CodeInterface2>())
                    {
                        if (nameFilter != null)
                        {
                            if (nameFilter.Any(@interface.Name.Contains) || !nameFilter.Any())
                                interfaces.Add(@interface.FullName.Substring(filter.Length + 1), @interface);
                        }
                        else
                            interfaces.Add(@interface.FullName.Substring(filter.Length + 1), @interface);
                    }
                }
            };

            var filterPrefix = filter;
            if (filterPrefix.Contains('.'))
                filterPrefix = filter.Substring(0, filterPrefix.IndexOf('.'));

            foreach (CodeNamespace element in project.CodeModel.CodeElements.OfType<CodeNamespace>().Where(x => x.FullName.StartsWith(filterPrefix)))
            {
                enumClasses(element, 2);
            }
        }

        public static CodeClass2 FindClass(this Project project, string name, IDictionary<string, CodeClass2> classes)
        {
            if (classes != null && classes.Keys.Any(x => x.EndsWith(name)))
                return classes.First(x => x.Key.EndsWith(name)).Value;
            else
                return null;
        }

        public static CodeInterface2 FindInterface(this Project project, string name, IDictionary<string, CodeInterface2> interfaces)
        {
            if (interfaces != null && interfaces.Any(x => x.Key.EndsWith("I" + name) || (x.Value.IsGeneric && x.Key.Left("<").EndsWith("I" + name))))
                return interfaces.First(x => x.Key.EndsWith("I" + name) || (x.Value.IsGeneric && x.Key.Left("<").EndsWith("I" + name))).Value;
            else
                return null;
        }

        public static ProjectItem AddItem(this Project project, string filePath, string fileName, string fileBody, Dictionary<string, object> properties)
        {
            if (!Directory.Exists(Path.Combine(project.RootFolder(), filePath ?? string.Empty)))
            {
                Directory.CreateDirectory(Path.Combine(project.RootFolder(), filePath ?? string.Empty));
            }

            string fullPath = Path.Combine(project.RootFolder(), filePath ?? string.Empty, fileName);

            File.WriteAllText(fullPath, fileBody, Encoding.UTF8);

            var projectItem = project.ProjectItems.AddFromFile(fullPath);

            foreach (Property prop in projectItem.Properties)
            {
                if (properties.ContainsKey(prop.Name))
                    prop.Value = properties[prop.Name];
            }
            return projectItem;
        }

        public static void UpdateItem(this ProjectItem projectItem, string content)
        {
            File.WriteAllText(projectItem.get_FileNames(0), content, Encoding.UTF8);
        }

        public static void UpdateItem(this ProjectItem projectItem, IEnumerable<string> content)
        {
            File.WriteAllLines(projectItem.get_FileNames(0), content, Encoding.UTF8);
        }
    }
}