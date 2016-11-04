using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

using EnvDTE;
using EnvDTE80;
using VSLangProj;


namespace Getequ.CodeStinck.Domain
{
    using CodeGeneration.CSharp;
    using Models;

    class ControllerDomain
    {
        Project _project;
        Solution _solution;
        Dictionary<string, CodeClass2> _classList = new Dictionary<string, CodeClass2>();
        Dictionary<string, CodeInterface2> _interfaceList = new Dictionary<string, CodeInterface2>();
        Dictionary<string, CodeInterface2> _platformInterfaceList = new Dictionary<string, CodeInterface2>();

        public ControllerDomain(Project project, Solution solution)
        {
            _project = project;
            _solution = solution;
        }

        public IEnumerable<CodeClass2> FindControllers(bool thisProject = false)
        {
            _classList = new Dictionary<string, CodeClass2>();
            _interfaceList = new Dictionary<string, CodeInterface2>();
            if (thisProject)
            {
                if (_project.Object is VSProject)
                {
                    _project.GetProjectCodeElements(_classList, _interfaceList, _project.DefaultNamespace(), new List<string> { "Controller", "ViewModel", "Service", "Module" });
                    _project.GetCodeElements(null, _platformInterfaceList, "Bars.B4", null);
                }
                else
                    throw new ArgumentException(_project.Name.Q("'") + " не является проектом C#");
            }
            else
            {
                _solution.GetSolutionCodeElements(_classList, _interfaceList, "Bars", new List<string> { "Controller", "ViewModel", "Service", "Module" });

                foreach (Project proj in _solution.Projects)
                {
                    if (proj.Object is VSProject)
                    {
                        proj.GetCodeElements(null, _platformInterfaceList, "Bars.B4", null);
                        break;
                    }
                }
            }
            

            return _classList.Where(x => x.Key.EndsWith("Controller")).Select(x => x.Value).ToList();
        }

        public IEnumerable<ControllerStinck> FindStinck(IEnumerable<CodeClass2> controllers)
        {
            List<ControllerStinck> stincks = new List<ControllerStinck>();

            foreach (CodeClass2 ctrl in controllers)
            {
                try
                {
                    // skip classes without accessible ProjectItem with CodeModel
                    var _checkAccessible = ctrl.ProjectItem;
                }
                catch
                {
                    continue;
                }

                var ctl = new CodeClassWrapper(ctrl);

                var stinck = new ControllerStinck{ Name = ctl.Name.Substring(0, ctl.Name.Length - 10), Class = ctl };

                bool good = true;

                string serviceProperty = "";
                string viewModelProperty = "";

                try
                {
                    foreach (CodePropertyWrapper prop in ctl.Properties)
                    {
                        if (IsServiceTypeName(stinck.Name, prop.Type))
                        {
                            serviceProperty = prop.Name;
                        } else
                        if (IsViewModelTypeName(stinck.Name, prop.Type))
                        {
                            viewModelProperty = prop.Name;
                        } else
                        {
                            stinck.NonServiceProps.Add(prop);
                            good = false;
                        }
                    }

                    foreach (CodeFunctionWrapper func in ctl.Methods)
                    {
                        if (func.IsOverride && (func.Name == "List" || func.Name == "Get" || func.Name == "Create" || func.Name == "Update" || func.Name == "Delete"))
                        {
                            stinck.BadPublicMethods.Add(func);
                            good = false;
                        }
                        else
                        {
                            if (!func.IsPublic)
                            {
                                stinck.NonPublicMethods.Add(func);
                                good = false;
                            }
                            else
                            {
                                // пропускаем методы, которые уже задействуют сервис/ВМ
                                if ((serviceProperty != "" && func.Code.ContainsWord(serviceProperty)) ||
                                    (viewModelProperty != "" && func.Code.ContainsWord(viewModelProperty)))
                                    continue;

                                if (func.Name.Contains("Get") || func.Name.Contains("List"))
                                    stinck.ViewModelMethods.Add(func);
                                else
                                    stinck.ServiceMethods.Add(func);
                            }
                        }
                    }

                    foreach (CodeClassWrapper nested in ctl.NestedClasses)
                    {
                        stinck.NestedClasses.Add(nested);
                        good = false;
                    }

                    if (!good && ctl.ProjectItem != null)
                    {
                        stincks.Add(stinck);
                        stinck.ProjectItem = ctl.ProjectItem;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("error: " + ex.Message);
                }
            }

            return stincks;
        }

        private bool IsViewModelTypeName(string ctlName, string typeName)
        {
            if (typeName.StartsWith("I") && typeName.EndsWith("ViewModel"))
            {
                var vmEntityName = typeName.Substring(1, typeName.Length - 10);
                return vmEntityName == ctlName || vmEntityName == "Custom"+ctlName;
            }
            else
                return false;
        }

        private bool IsServiceTypeName(string ctlName, string typeName)
        {
            if (typeName.StartsWith("I") && typeName.EndsWith("Service") && !typeName.EndsWith("DomainService"))
            {
                var vmEntityName = typeName.Substring(1, typeName.Length - 8);
                return vmEntityName == ctlName;
            }
            else
                return false;
        }

        public ClassGenerationResult GenerateRefactoringCode(ControllerStinck ctl)
        {
            var entityName = ctl.Name;
            

            var svcClass = _project.FindClass(entityName + "Service", _classList);
            var svcIf = _project.FindInterface(entityName + "Service", _interfaceList);


            var vmClass = _project.FindClass(entityName + "ViewModel", _classList);
            var vmIf = _project.FindInterface(entityName + "ViewModel", _interfaceList);
            var vmPlatform = _platformInterfaceList.FirstOrDefault(x => x.Key == "IViewModel<T>").Value;
            if (vmClass == null)
                vmClass = _project.FindClass("Custom" + entityName + "ViewModel", _classList);
            if (vmIf == null)
                vmIf = _project.FindInterface("Custom" + entityName + "ViewModel", _interfaceList);


            var dsClass = _project.FindClass(entityName + "DomainService", _classList);
            var dsPlatform = _platformInterfaceList.FirstOrDefault(x => x.Key == "IDomainService<T>").Value;


            var baseClass = ctl.Class.Base;

            var vmBase = "BaseViewModel<" + entityName + ">";
            var dsBase = (baseClass.StartsWith("FileStorage") ? "FileStorage" : "Base") + "DomainService<" + entityName + ">";

            var modification = new ControllerModification();
            var svcCode = ProcessClassCode(ctl, null, "Service", "Services", entityName + "Service", "I" + entityName + "Service", svcClass, svcIf, null, ctl.ServiceMethods, modification);
            if (svcCode.ClassCode != null && !ctl.Class.Properties.Any(x => x.Name == entityName + "Service"))
            {
                modification.AddedNamespaces.Add(_project.DefaultNamespace().Right(".") + ".Services");
                modification.AddedProperties.Add("public I" + entityName + "Service " + entityName + "Service { get; set; }");
            }

            var vmCode = ProcessClassCode(ctl, vmBase, "ViewModel", "ViewModel", "CustomViewModel", ctl.ViewModelMethods.Any() ? "I" + entityName + "ViewModel" : null, vmClass, vmIf, vmPlatform, ctl.BadPublicMethods.Where(x => x.Name == "List" || x.Name == "Get").Union(ctl.ViewModelMethods), modification);
            if (vmCode.InterfaceCode != null && !ctl.Class.Properties.Any(x => x.Name == "CustomViewModel"))
            {
                modification.AddedNamespaces.Add(_project.DefaultNamespace().Right(".") + ".ViewModel");
                modification.AddedProperties.Add("private I" + entityName + "ViewModel CustomViewModel { get { return ViewModel as I" + entityName + "ViewModel; } }");
            }
            //var dsCode = GenerateClassCode(ctl, dsBase, "DomainService", null, dsClass, null, dsPlatform, ctl.DomainServiceMethods, modification);

            var controllerCode = ClearController(ctl.Class, modification);

            var result = new ClassGenerationResult() { EntityName = ctl.Name };
            result.ServiceClass = svcCode.ClassCode;
            result.ServiceInterface = svcCode.InterfaceCode;

            result.ViewModelClass= vmCode.ClassCode;
            result.ViewModelInterface = vmCode.InterfaceCode;

            //result.DomainServiceClass = dsCode.ClassCode;
            result.Controller = controllerCode;

            result.Class = ctl.Class;
            result.KnownTypes = svcCode.KnownTypes.Union(vmCode.KnownTypes).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();

            return result;
        }

        /// <summary>Найти в строке ресолвы из контейнера</summary>
        private string ExtractDependency(ref string line, Dictionary<string, string> iocDeps)
        {
            if (line.Contains("Container.Resolve"))
            {
                var start = line.IndexOf("Container.Resolve");
                var end = line.Substring(start).IndexOf(">()");

                if (end == -1)
                    return "IWindsorContainer Container";
                else
                    end += 3;

                if (line.Substring(start + 17).StartsWith("All<"))
                    return "IWindsorContainer Container";

                var dep = line.Substring(start + 17 + 1, end - (17 + 1) - 3);
                var prop = "";

                var resolve = line.Substring(start, end);

                if (dep == "IDataTransaction")
                    return "IWindsorContainer Container";

                if (dep.Contains("<") && dep.Last() != '>')
                {
                    if (dep.StartsWith("omain<"))
                    {
                        dep = dep.Replace("omain", "IDomainService") + ">";
                    }
                    else
                        if (dep.StartsWith("epository<"))
                        {
                            dep = "IR" + dep + ">";
                        }
                        else
                        {
                            dep = "I" + line[start + 17] + dep + ">";
                        }
                }

                var depPropName = dep.Unwrap("<>").Split('.').Last();

                if (dep.Contains("<"))
                {
                    if (dep.StartsWith("IDomainService<"))
                    {
                        prop = "Ds" + depPropName;
                    }
                    else
                        if (dep.StartsWith("IRepository<"))
                        {
                            prop = "Rs" + depPropName;
                        }
                        else
                        {
                            prop = depPropName;
                        }
                }
                else
                {
                    if (_platformInterfaceList.Any(x => x.Key.Contains(depPropName)))
                        prop = depPropName.Substring(1);
                    else
                        return string.Empty;
                }

                foreach (var iocDep in iocDeps)
                {
                    if (iocDep.Value.Contains(" " + dep + " "))
                    {
                        prop = iocDep.Key;
                        line = line.Replace(resolve, prop);
                        return string.Empty;
                    }
                }

                line = line.Replace(resolve, prop);
                return dep + " " + prop;
            }
            else
                return string.Empty;
        }

        private Dictionary<string, string> ExtractIoCDependencies(ControllerStinck stinck, IList<string> fileCode, IEnumerable<MethodConvertResult> methods, List<string> knownTypes)
        {
            var iocDeps = new Dictionary<string, string>();

            var depLines = new List<string>();
            foreach (var prop in stinck.NonServiceProps)
            {
                if (methods.Any(x => x.Code.Any(y => y.ContainsWord(prop.Name))))
                {
                    var propLine = fileCode[prop.StartLine - 1].Trim();
                    if (prop.Type.Contains("<"))
                    {
                        knownTypes.Add(prop.Type.Unwrap("<>"));
                    }
                    else
                        knownTypes.Add(prop.Type);

                    var parts = propLine.Split(' ');
                    iocDeps.Add(prop.Name, parts[1]);
                }
            }

            var extractedDeps = new List<string>();
            foreach (var method in methods)
            {
                for (int i = 0; i < method.Code.Count; i++)
                {
                    var line = method.Code[i];
                    var dep = ExtractDependency(ref line, iocDeps);
                    var typeNamePair = dep.Split(' ');
                    knownTypes.Add(typeNamePair[0].Unwrap("<>").Split('.').Last());
                    extractedDeps.Add(dep);
                    method.Code[i] = line;
                }
            }
            extractedDeps.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList()
                .ForEach(x =>
                {
                    var parts = x.Split(' ');
                    iocDeps.Add(parts[1], parts[0]);
                });
            /*


            if (iocDeps.Any() || extractedDeps.Any())
            {
                iocDeps.Values.Union(extractedDeps)
                .OrderBy(x => x).ToList()
                .ForEach(depLines.Add);
            }*/
            return iocDeps;
        }

        private void FindNamespaceUsings(IEnumerable<string> fileCode, string defaultNamespace, List<string> outerCode, List<string> innerCode)
        {
            var innerUsings = new List<string>();
            var moveToInner = new List<string>();

            var outerUsings = new List<string>();
            var moveToOuter = new List<string>();
            var skip = 0;

            foreach (var line in fileCode.Select(x => x.Trim().CutLast(1)))
            {
                if (line.Trim().StartsWith("using "))
                {
                    if (line.Contains("Bars."))
                        moveToInner.Add(line);
                    else
                        outerUsings.Add(line);
                }
                else
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        skip++;
                        continue;
                    }
                    else
                        break;
                skip++;
            }

            // skip line "namespace Some.Thing" and next "{"
            skip += 2;

            foreach (var line in fileCode.Skip(skip).Select(x => x.Trim().CutLast(1)))
            {
                if (line.StartsWith("using "))
                {
                    if (line.Contains(" System"))
                        moveToOuter.Add(line);
                    else
                        innerUsings.Add(line);
                }
                else
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    else
                        break;
            }

            foreach (var systemNs in outerUsings.Union(moveToOuter).Where(x => x.Contains(" System")).OrderBy(x => x))
            {
                outerCode.Add(systemNs + ";");
            }

            if (outerCode.Any() && outerUsings.Union(moveToOuter).Where(x => !x.Contains(" System") && !string.IsNullOrWhiteSpace(x.Trim())).Any())
                outerCode.Add("");
            foreach (var usingNs in outerUsings.Union(moveToOuter).Where(x => !x.Contains(" System") && !string.IsNullOrWhiteSpace(x.Trim())).OrderBy(x => x))
            {
                outerCode.Add(usingNs.Trim() + ";");
            }

            var fileProjectNamespace = defaultNamespace.Replace("Bars.", "") + ".";

            // сортировка внутренних using
            var b4 = new List<string>();
            var b4Modules = new List<string>();
            var projNs = new List<string>();
            var other = new List<string>();

            foreach (var usingNs in innerUsings.Union(moveToInner).Select(x => x.Replace("Bars.", "").Trim()))
            {
                if (usingNs.Contains("B4.Modules"))
                    b4Modules.Add(usingNs);
                else
                    if (usingNs.Contains("B4"))
                        b4.Add(usingNs);
                    else
                        if (usingNs.Contains(fileProjectNamespace.Left(".")) && !usingNs.Contains(fileProjectNamespace))
                            projNs.Add(usingNs);
                        else
                            other.Add(usingNs);
            }

            b4.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns + ";"));
            b4Modules.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns + ";"));
            if (innerCode.Any() && (projNs.Any() || other.Any()))
                innerCode.Add("");
            projNs.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns + ";"));
            other.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns.Replace(fileProjectNamespace, "") + ";"));
        }

        private void FindNamespaceUsings2(ProjectItem projectItem, IEnumerable<string> fileCode, string defaultNamespace, List<string> outerCode, List<string> innerCode)
        {
            var innerUsings = new List<string>();
            var moveToInner = new List<string>();

            var outerUsings = new List<string>();
            var moveToOuter = new List<string>();
            var skip = 0;

            foreach (CodeImport import in projectItem.FileCodeModel.CodeElements.OfType<CodeImport>())
            {
                Debug.WriteLine(import.Name + "     " + import.Namespace + "        - " + import.FullName);
            }

            foreach (CodeNamespace ns in projectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>())
            {
                foreach (CodeImport import in ns.Members.OfType<CodeImport>())
                    Debug.WriteLine("ns:  " + import.Name + "     " + import.Namespace + "        - " + import.FullName);
            }

            foreach (var line in fileCode.Select(x => x.Trim().CutLast(1)))
            {
                if (line.Trim().StartsWith("using "))
                {
                    if (line.Contains("Bars."))
                        moveToInner.Add(line);
                    else
                        outerUsings.Add(line);
                }
                else
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        skip++;
                        continue;
                    }
                    else
                        break;
                skip++;
            }

            // skip line "namespace Some.Thing" and next "{"
            skip += 2;

            foreach (var line in fileCode.Skip(skip).Select(x => x.Trim().CutLast(1)))
            {
                if (line.StartsWith("using "))
                {
                    if (line.Contains(" System"))
                        moveToOuter.Add(line);
                    else
                        innerUsings.Add(line);
                }
                else
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    else
                        break;
            }

            foreach (var systemNs in outerUsings.Union(moveToOuter).Where(x => x.Contains(" System")).OrderBy(x => x))
            {
                outerCode.Add(systemNs + ";");
            }

            if (outerCode.Any() && outerUsings.Union(moveToOuter).Where(x => !x.Contains(" System") && !string.IsNullOrWhiteSpace(x.Trim())).Any())
                outerCode.Add("");
            foreach (var usingNs in outerUsings.Union(moveToOuter).Where(x => !x.Contains(" System") && !string.IsNullOrWhiteSpace(x.Trim())).OrderBy(x => x))
            {
                outerCode.Add(usingNs.Trim() + ";");
            }

            var fileProjectNamespace = defaultNamespace.Replace("Bars.", "") + ".";

            // сортировка внутренних using
            var b4 = new List<string>();
            var b4Modules = new List<string>();
            var projNs = new List<string>();
            var other = new List<string>();

            foreach (var usingNs in innerUsings.Union(moveToInner).Select(x => x.Replace("Bars.", "").Trim()))
            {
                if (usingNs.Contains("B4.Modules"))
                    b4Modules.Add(usingNs);
                else
                    if (usingNs.Contains("B4"))
                        b4.Add(usingNs);
                    else
                        if (usingNs.Contains(fileProjectNamespace.Left(".")) && !usingNs.Contains(fileProjectNamespace))
                            projNs.Add(usingNs);
                        else
                            other.Add(usingNs);
            }

            b4.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns + ";"));
            b4Modules.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns + ";"));
            if (innerCode.Any() && (projNs.Any() || other.Any()))
                innerCode.Add("");
            projNs.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns + ";"));
            other.OrderBy(x => x).ToList().ForEach(ns => innerCode.Add("    " + ns.Replace(fileProjectNamespace, "") + ";"));
        }

        private GenerateClassCodeResult ProcessClassCode(
            ControllerStinck stinck, 
            string baseName, 
            string classType, 
            string subSpace, 
            string propName, 
            string interfaceName, 
            CodeClass2 @class = null, 
            CodeInterface @interface = null, 
            CodeInterface platformInterface = null,
            IEnumerable<CodeFunctionWrapper> methods = null, 
            ControllerModification modification = null)
        {
            var result = new GenerateClassCodeResult();
            result.KnownTypes = new List<string> { "ActionResult", "IQueryable", "IDomainService", "IRepository", "DateTime", "IWindsorContainer",
                "BaseParams", "IDataResult", "BaseViewModel", "BaseDataResult", "ListDataResult", "JsonNetResult", "JsonGetResult", "JsonListResult", 
                stinck.Name, "IDataTransaction", stinck.Name + classType};

            var fileCode = ProjectItemHelper.GetFileLines(stinck.ProjectItem);

            var ci = new ControllerConvertInfo();
            FindNamespaceUsings(fileCode, _project.DefaultNamespace(), ci.outerUsings, ci.innerUsings);


            foreach (var method in methods)
            {
                try
                {
                    var converted = ConvertMethod(stinck, fileCode, method, propName, platformInterface);
                    ci.specificMethods.Add(converted);
                }
                catch (Exception e)
                {
                    result.Errors.Add(e);
                }
            }

            if (!ci.specificMethods.Any())
                return new GenerateClassCodeResult();

            // рекурсивно собрать приватные методы
            bool anyConverted = false;
            do {
                anyConverted = false;
                foreach (var method in stinck.NonPublicMethods.Where(x => !ci.otherMethods.Any(y => y.Method == x)))
                {
                    if (!ci.specificMethods.Union(ci.otherMethods).Any(x => x.Code.Any(y => y.ContainsWord(method.Name))))
                        continue;

                    try
                    {
                        var converted = ConvertMethod(stinck, fileCode, method, propName, platformInterface);
                        ci.otherMethods.Add(converted);
                        anyConverted = true;
                    }
                    catch (Exception e)
                    {
                        result.Errors.Add(e);
                    }
                }
            }
            while (anyConverted);
             
            // рекурсивно собрать вложенные классы
            var anyNested = false;
            do {
                anyNested = false;
                foreach (var nested in stinck.NestedClasses.Where(x => !ci.nestedClasses.Any(y => y == x)))
                {
                    if (!ci.specificMethods.Union(ci.otherMethods).Any(x => x.Code.Any(y => y.ContainsWord(nested.Name)))
                        && !ci.nestedClasses.Any(x => x.FileCode.Any(y => y.ContainsWord(nested.Name))))
                        continue;

                    ci.nestedClasses.Add(nested);
                    result.KnownTypes.Add(nested.Name);
                    anyNested = true;
                }
            }
            while (anyNested);


            if (ci.specificMethods.Union(ci.otherMethods).Any(x => x.Code.ContainsWord("ActionResult")) && !ci.outerUsings.ContainsString("System.Web.Mvc"))
            {
                for (int i = 0; i < ci.outerUsings.Count; i++)
                    if (!ci.outerUsings[i].StartsWith("using System"))
                    {
                        ci.outerUsings.Insert(i, "using System.Web.Mvc;  // methods return ActionResult!");
                        break;
                    }
            }

            ci.deps = ExtractIoCDependencies(stinck, fileCode, ci.specificMethods.Union(ci.otherMethods), result.KnownTypes);
            if (ci.deps.Values.Any(x => x == "IWindsorContainer"))
            {
                ci.outerUsings.Add("using Castle.Windsor;");
            }
            
            if (@class == null)
            {
                result.ClassCode = GenerateClassCode(stinck, baseName, classType, subSpace, interfaceName, modification, ci, fileCode);
            }
            else
            {
                result.ClassCode = UpdateClassCode(stinck, @class, methods, modification, ci);
            }

            if (ci.specificMethods.Any(x => !x.IsOverride))
            {
                if (@interface == null)
                    result.InterfaceCode = GenerateInterfaceCode(interfaceName, subSpace, ci.specificMethods.Where(x => !x.IsOverride));
                else
                    result.InterfaceCode = UpdateInterfaceCode(@interface, ci.specificMethods.Where(x => !x.IsOverride));
                result.KnownTypes.Add(interfaceName);
            }
            return result;
        }

        private string UpdateClassCode(ControllerStinck stinck, CodeClass2 @class, IEnumerable<CodeFunctionWrapper> methods, ControllerModification modification,
            ControllerConvertInfo ci)
        {
            var wrapped = new CodeClassWrapper(@class);
            var code = wrapped.FileCode.ToList();
            var endLine = wrapped.EndLine - 1;
            var propLine = wrapped.Properties.Any() ? wrapped.Properties.Max(x => x.EndLine) : 0;
            if (propLine == 0)
            {
                propLine = wrapped.Methods.Min(x => x.StartLine);
                for (int i = propLine; i > 0; i--)
                {
                    if (!code[i].Trim().StartsWith("//"))
                    {
                        propLine = i;
                        break;
                    }
                }
            }

            var nsStartLine = @class.ProjectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>().First().StartPoint.Line;
            if (methods.Any(x => x.Code.ContainsWord("ActionResult")) && !code.Any(x => x.Trim() == "using System.Web.Mvc;"))
            {
                int lineIndex = -1;

                for (var i = 0; i < code.Count; i++)
                {
                    if (code[i].Trim().StartsWith("namespace "))
                        break;

                    if (code[i].Trim().StartsWith("using System"))
                        lineIndex = i;
                }
                code.Insert(lineIndex + 1, "using System.Web.Mvc;");
                endLine++;
                nsStartLine++;
            }

            if (ci.specificMethods.Union(ci.otherMethods).Any(x => x.Code.ContainsAnyWord(new[] { "IDataResult", "BaseParams" })) &&
                !code.Any(x => x.Trim() == "using B4;") && !code.Any(x => x.Trim() == "using Bars.B4;"))
            {
                code.Insert(nsStartLine + 1, "    using B4;");
                endLine++;
            }

            if (ci.deps.Any())
            {
                foreach (var dep in ci.deps)
                {
                    if (wrapped.Properties.Any(x => x.Code.ContainsWord(dep.Value)))
                        continue;

                    code.Insert(propLine++, ("public " + dep.Value + " " + dep.Key + " { get; set; }").Ind(2));
                    endLine++;
                    modification.UsedProperties.Add(dep.Key);
                }
            }

            foreach (var converted in ci.specificMethods.Union(ci.otherMethods))
            {
                code.Insert(endLine++, "");
                if (!string.IsNullOrEmpty(converted.Comment))
                    code.Insert(endLine++, converted.Comment);
                code.InsertRange(endLine, converted.Code);
                endLine += converted.Code.Count;
            }
            modification.ConvertedMethods.AddRange(ci.specificMethods.Union(ci.otherMethods));

            foreach (var nested in stinck.NestedClasses)
            {
                if (ci.nestedClasses.Contains(nested))
                {
                    code.Insert(endLine++, "");
                    code.InsertRange(endLine, nested.Code);
                    endLine += nested.Code.Count;
                }
            }
            modification.UsedClasses.AddRange(ci.nestedClasses);

            return string.Join(Environment.NewLine, code);
        }

        private string GenerateClassCode(ControllerStinck stinck, string baseName, string classType, string subSpace, string interfaceName, 
            ControllerModification modification, ControllerConvertInfo ci, IList<string> fileCode)
        {
            var code = new List<string>();

            // составить код класса
            if (ci.outerUsings.Any())
            {
                ci.outerUsings.ForEach(code.Add);
                code.Add("");
            }
            code.Add("namespace " + _project.DefaultNamespace() + "." + subSpace);
            code.Add("{");
            if (ci.innerUsings.Any())
            {
                ci.innerUsings.ForEach(code.Add);
                code.Add("");
            }

            var classBase = baseName + (!string.IsNullOrEmpty(baseName) && !string.IsNullOrEmpty(interfaceName) ? ", " : "") + interfaceName;

            code.Add("    public class " + stinck.Name + classType + " : " + classBase);
            code.Add("    {");


            if (ci.deps.Any())
            {
                code.Add("#region IoC dependencies".Ind(2));
                foreach (var dep in ci.deps)
                {
                    code.Add(("public " + dep.Value + " " + dep.Key + " { get; set; }").Ind(2));
                }
                code.Add("#endregion".Ind(2));
                code.Add("");
            }
            modification.UsedProperties.AddRange(ci.deps.Keys);

            foreach (var converted in ci.specificMethods.Union(ci.otherMethods))
            {
                if (!string.IsNullOrEmpty(converted.Comment))
                    code.Add(converted.Comment);
                code.AddRange(converted.Code);
                code.Add("");
            }
            modification.ConvertedMethods.AddRange(ci.specificMethods.Union(ci.otherMethods));

            foreach (var nested in stinck.NestedClasses)
            {
                if (ci.nestedClasses.Contains(nested))
                {
                    for (int i = nested.StartLine - 1; i < nested.EndLine; i++)
                        code.Add(fileCode[i]);
                    code.Add("");
                }
            }
            modification.UsedClasses.AddRange(ci.nestedClasses);


            if (code.Last().Trim() == "")
            {
                code.RemoveAt(code.Count - 1);
            }
            code.Add("    }");
            code.Add("}");

            return string.Join(Environment.NewLine, code);
        }

        /// <summary>
        /// Сгенерировать код интерфнейса
        /// </summary>
        /// <param name="interfaceName">название</param>
        /// <param name="subSpace">подпространство относительно модуля</param>
        /// <param name="methods">методы</param>
        /// <returns>код интерфейса строкой</returns>
        private string GenerateInterfaceCode(string interfaceName, string subSpace, IEnumerable<MethodConvertResult> methods)
        {
            List<string> code = new List<string>();
            var ns = new NamespaceInfo(_project.DefaultNamespace() + "." + subSpace);

            if (methods.Any(x => x.Code.ContainsWord("ActionResult")))
            {
                ns.OuterUsing.Add("System.Web.Mvc");
            }

            ns.InnerUsing.Add("B4");
            ns.InnerUsing.Add("Entities");

            var iface = new InterfaceInfo(interfaceName);
            ns.NestedValues.Add(iface);

            foreach (var method in methods)
            {
                var ifm = new MethodInfo() { SignatureOnly = true };
                
                if (!string.IsNullOrEmpty(method.Comment))
                    ifm.Summary = method.Comment;

                var signature = method.Code[0].Replace("public ", "").Trim().Replace("  ", " ");
                var parts = signature.Split(' ');
                ifm.Type = parts[0];
                ifm.Name = parts[1].Left("(");
                ifm.Params = signature.Unwrap("()");
                ifm.SignatureParams = signature.Right(")");

                iface.AddMethod(ifm);
            }
            
            return string.Join(Environment.NewLine, ns.Generate());
        }

        /// <summary>
        /// Добавить в интерфейс новые сигнатуры
        /// </summary>
        /// <param name="interface">интерфейс</param>
        /// <param name="methods">новые методы</param>
        /// <returns>код файла строкой</returns>
        private string UpdateInterfaceCode(CodeInterface @interface, IEnumerable<MethodConvertResult> methods)
        {
            IList<string> code = ProjectItemHelper.GetFileLines(@interface.ProjectItem).ToList();

            var endLine = @interface.EndPoint.Line-1;
            var nsStartLine = @interface.ProjectItem.FileCodeModel.CodeElements.OfType<CodeNamespace>().First().StartPoint.Line;

            if (methods.Any(x => x.Code.ContainsWord("ActionResult")) && !code.Any(x => x.Trim() == "using System.Web.Mvc;"))
            {
                int lineIndex = -1;

                for (var i = 0; i< code.Count; i++)
                {
                    if (code[i].Trim().StartsWith("namespace "))
                        break;

                    if (code[i].Trim().StartsWith("using System"))
                        lineIndex = i;
                }
                code.Insert(lineIndex+1, "using System.Web.Mvc;");
                endLine++;
                nsStartLine++;
            }

            if (methods.Any(x => x.Code.ContainsAnyWord(new[] { "IDataResult", "BaseParams" })) &&
                !code.Any(x => x.Trim() == "using B4;") && !code.Any(x => x.Trim() == "using Bars.B4;"))
            {
                code.Insert(nsStartLine + 1, "    using B4;");
                endLine++;
            }
            
            foreach (var method in methods)
            {
                var ifm = new MethodInfo() { SignatureOnly = true };

                if (!string.IsNullOrEmpty(method.Comment))
                    ifm.Summary = method.Comment;

                var signature = method.Code[0].Replace("public ", "").Trim().Replace("  ", " ") + ";";
                code.Insert(endLine++, "");
                code.Insert(endLine++, signature.Ind(2));
            }

            return string.Join(Environment.NewLine, code);
        }

        /// <summary>Преобразовать метод контроллера в метод сервиса/вьюМодели</summary>
        private MethodConvertResult ConvertMethod(ControllerStinck stinck, IList<string> fileCode, CodeFunctionWrapper method, string propName, CodeInterface platform)
        {
            var body = new List<string>();
            bool isOverride = method.IsOverride;
            var platformMethod = platform != null ? platform.Members.OfType<CodeFunction2>().FirstOrDefault(x => x.Name == method.Name) : null;

            string xComment = null;
            if (method.DocComment != null && !string.IsNullOrEmpty(method.DocComment.Untag("doc").Trim()))
            {
                xComment = "        ///" + method.DocComment.Untag("doc").Trim().Replace(Environment.NewLine, Environment.NewLine + "        ///");
            }

            var paramStart = Int32.MaxValue;
            foreach (CodeElement param in method.Parameters)
            {
                if (param.StartPoint.LineCharOffset < paramStart)
                    paramStart = param.StartPoint.LineCharOffset;
            }

            body.AddRange(method.Code);

            // проверить что метод не будет вызывать себя же
            if (platformMethod != null)
            {
                var signature = (platform.Name.Substring(1) + "." + method.Name + "(" +
                                    string.Join(", ", platformMethod.Parameters.OfType<CodeParameter>().Select(x => x.Name).ToArray()) + ")").ToUpper();

                if (body.Any(x => x.ToUpper().Contains(signature)))
                    throw new Exception("Метод " + method.Name + " будет вызывать себя же: "+ Environment.NewLine + body.First(x => x.ToUpper().Contains(signature)));
            }

            // ДС требуется в коде метода явно
            var dsRequired = body.Any(x => x.ContainsWord("DomainService")
                                || x.Contains(".Resolve<IDomainService<" + stinck.Name + ">>")
                                || x.Contains(".ResolveDomain<" + stinck.Name + ">"));

            // если это override-метод платформы проверить ДС в сигнатуре
            if (platformMethod != null)
            {
                foreach (CodeParameter param in platformMethod.Parameters)
                {
                    if (param.Type.CodeType.Name == "IDomainService")
                    {
                        dsRequired = true;
                        break;
                    }
                }
            }
            
            var requiredPrivates = new List<string>();
            var dsRequiredPrivates = new List<string>();
            foreach(CodeFunctionWrapper closed in stinck.NonPublicMethods.Where(x => x.Name != method.Name && body.ContainsWord(x.Name)))
            {
                var depCheck = CheckDomainServiceDependency(stinck.Name, closed, fileCode);

                if (depCheck == DependencyCheckResult.Yes)
                    dsRequiredPrivates.Add(closed.Name);

                // ДС требуется т.к. используется в вызываемых методах 1 уровня
                if (depCheck == DependencyCheckResult.Yes || depCheck == DependencyCheckResult.NowResolved)
                    dsRequired = true;

                if (body.ContainsWord(closed.Name))
                    requiredPrivates.Add(closed.Name);
            }

            if (dsRequired)
            {
                body[0] = body[0].Insert(paramStart - 1, "IDomainService<" + stinck.Name + "> domainService, ");
            }

            var wordPairs = new Dictionary<string, string>{
                { "JsonListResult", "ListDataResult" },
                { "JsonGetResult", "BaseDataResult" },
                { "JsonNetResult", "BaseDataResult" },
                { "DomainService", "domainService" }
            };

            var stringPairs = new Dictionary<string, string>
            {
                {"Container.Resolve<IDomainService<" + stinck.Name + ">>()", "domainService"},
                {"Container.ResolveDomain<" + stinck.Name + ">()", "domainService"},
                {"JsSuccess();", "new BaseDataResult{ Success = true };"},
                {"JsonNetResult.Success;", "new BaseDataResult{ Success = true };"},
                {"JsonNetResult.Failure", "BaseDataResult.Error"},
                {"JsFailure", "BaseDataResult.Error"}
            };
            foreach (var prive in dsRequiredPrivates)
            {
                stringPairs.Add(prive + "(", prive + "(domainService, ");
            }

            body = body.Replace(stringPairs).ReplaceWords(wordPairs).ToList();

            if (body.ContainsAnyWord(new[] { "ListDataResult", "BaseDataResult" }))
                body = body.ReplaceWord("ActionResult", "IDataResult").ToList();

            MethodConvertResult result = new MethodConvertResult { 
                Code = body, 
                Method = method,
                DomainServiceRequired = dsRequired,
                IsOverride = isOverride
            };
            result.ReplaceCode = GenerateMethodReplacing(result, fileCode[method.StartLine - 1], propName);
            if (xComment != null)
                result.Comment = xComment;

            return result;
        }

        /// <summary>
        /// Сгенерировать код, которым будет заменён имеющийся в методе код
        /// </summary>
        /// <param name="method">сконвертированный метод</param>
        /// <param name="signature">сигнатура метода</param>
        /// <param name="propName">имя свойства, метод которого вызывается в контроллере</param>
        /// <returns>код замены с диапазоном строк, который заменяется</returns>
        private Tuple<Point, List<string>> GenerateMethodReplacing(MethodConvertResult method, string signature, string propName)
        {
            Point range = new Point { X = method.Method.StartLine, Y = method.Method.EndLine };
            
            if (method.IsOverride || !method.Method.IsPublic)
                return new Tuple<Point, List<string>>(range, null);

            var body = new List<string>();

            var @params = new List<string>();
            foreach (CodeParameter param in method.Method.Parameters)
            {
                @params.Add(param.Name);
            }
            if (method.DomainServiceRequired)
                @params.Insert(0, "DomainService");

            body.Add(signature);
            body.Add("        {");
            body.Add("            var data = " + propName + "." + method.Name + "(" + string.Join(", ", @params) + ");");
            body.Add("            return new JsonNetResult(data);");
            body.Add("        }");

            return new Tuple<Point, List<string>>(range, body);
        }

        /// <summary>
        /// Требуется ли включить ДС в параметры метода, так как код метода работает с домен-сервисом
        /// </summary>
        private DependencyCheckResult CheckDomainServiceDependency(string entityClass, CodeFunctionWrapper method, IList<string> fileCode)
        {
            foreach (CodeParameter param in method.Parameters)
            {
                if (param.Name == "domainService")
                    return DependencyCheckResult.NowResolved;
            }

            if (method.Code.Any(x => x.ContainsWord("DomainService")
                              || x.Contains("ontainer.Resolve<IDomainService<" + entityClass + ">>")
                              || x.Contains("ontainer.ResolveDomain<" + entityClass + ">")))
                return DependencyCheckResult.Yes;
            else
                return DependencyCheckResult.No;
        }

        private enum DependencyCheckResult
        { 
            // Зависит, но не указан
            Yes,

            // Не зависит
            No,

            // Зависит и уже указан
            NowResolved
        }

        /// <summary>
        /// Вычистить из контроллера всё, что выносится в отдельные классы
        /// </summary>
        /// <param name="cls">класс контроллера</param>
        /// <param name="mod">набор изменений</param>
        /// <returns>Новый код контроллера</returns>
        private string ClearController(CodeClassWrapper cls, ControllerModification mod)
        {
            var usedProps = new List<CodePropertyWrapper>();
            var usedMethods = new List<CodeFunctionWrapper>();

            IList<string> fileCode = cls.FileCode;

            var clsProps = cls.Properties;
            var clsMethods = cls.Methods;
            var clsClasses = cls.NestedClasses;

            var propLine = clsProps.Any() ? clsProps.Max(x => x.EndLine) : clsMethods.Min(x => x.StartLine) - 1;

            // collect used in remain members
            foreach (var member in cls.Methods.Where(x => !mod.ConvertedMethods.Any(y => x == y.Method)))
            {
                var lines = member.Code;

                foreach (var prop in clsProps)
                {
                    if (lines.Any(x => x.ContainsWord(prop.Name)))
                        usedProps.Add(prop);
                }

                foreach (var method in cls.Methods.Where(x => !mod.ConvertedMethods.Any(y => x == y.Method) && x != member))
                {
                    if (lines.Any(x => x.ContainsWord(method.Name)))
                        usedMethods.Add(method);
                }
            }

            Dictionary<Point, List<string>> skipRegions = new Dictionary<Point, List<string>>();

            foreach (var prop in clsProps)
            {
                if (!usedProps.Contains(prop))
                {
                    skipRegions.Add(prop.Range, null);
                }
            }

            foreach (var @class in clsClasses)
            {
                if (!mod.UsedClasses.Contains(@class))
                {
                    skipRegions.Add(@class.Range, null);
                }
            }
            
            foreach (var ncls in clsClasses)
            {
                bool used = false;
                foreach (var method in clsMethods.Where(x => !mod.ConvertedMethods.Any(y => y.Method == x)))
                {
                    if (method.Code.Any(x => x.ContainsWord(ncls.Name)))
                        used = true;
                }

                if (!used)
                    skipRegions.Add(ncls.Range, null);
            }

            var leftMethods = clsMethods.Where(x => !mod.ConvertedMethods.Any(y => y.Method == x));
            foreach (var method in mod.ConvertedMethods)
            {
                if (leftMethods.Any(x => x.Code.Any(y => y.ContainsWord(method.Name))))
                    continue;

                if (skipRegions.ContainsKey(method.ReplaceCode.Item1))
                    continue;
                else
                    skipRegions.Add(method.ReplaceCode.Item1, method.ReplaceCode.Item2);
            }

            List<string> newCode = new List<string>();
            bool propInserted = false;
            for (int i = 1; i <= fileCode.Count; i++)
            {
                // добавить новые namespace
                if (fileCode[i - 1] == cls.Code.First() && mod.AddedNamespaces.Any())
                {
                    for (int u = newCode.Count - 1; u > -1; u--)
                    {
                        if (newCode[u].Trim().StartsWith("using ") || newCode[u].Trim().StartsWith("{"))
                        {
                            mod.AddedNamespaces.ForEach(x => newCode.Insert(u + 1, "    using " + x + ";"));

                            if (newCode.Count == u + mod.AddedNamespaces.Count + 1)
                                newCode.Add("");
                            else
                            if (newCode[u + mod.AddedNamespaces.Count+1].Trim() != "")
                                newCode.Insert(u + mod.AddedNamespaces.Count+1, "");

                            break;
                        }
                    }
                }

                if (skipRegions.Keys.Any(range => range.X <= i && range.Y >= i))
                {
                    Point range = skipRegions.Keys.First(r => r.X <= i && r.Y >= i);

                    if (skipRegions[range] != null)
                    {
                        newCode.AddRange(skipRegions[range]);
                    }
                    else
                    {
                        while (true)
                        {
                            if (newCode.Last().Trim().StartsWith("//"))
                                newCode.RemoveAt(newCode.Count - 1);
                            else
                                break;
                        }
                    }

                    i += range.Y - range.X;
                }
                else
                    newCode.Add(fileCode[i - 1]);

                if (i >= propLine && !propInserted)
                {
                    foreach(var prop in mod.AddedProperties)
                        newCode.Add("        " + prop + Environment.NewLine);
                    propInserted = true;
                }
            }

            #region clear empty spaces
            for (int i = newCode.Count; i > 1; i--)
            {
                if (newCode[i - 1].Trim() == "" && newCode[i - 2].Trim() == "")
                    newCode.RemoveAt(i - 1);
            }

            for (int i = newCode.Count; i > 1; i--)
            {
                if (newCode[i - 1].Trim() == "#endregion" && newCode[i - 2].Trim().StartsWith("#region"))
                {
                    newCode.RemoveAt(i - 1);
                    newCode.RemoveAt(i - 2);
                    i--;
                }
            }
            #endregion

            return string.Join(Environment.NewLine, newCode);
        }

        /// <summary>
        /// Зарегистрировать в Module.cs новые классы
        /// </summary>
        /// <param name="deps">список зависимотей (Тип, Интерфейс, Класс)</param>
        private void AddModuleDependencies(IEnumerable<Tuple<string, string, string>> deps)
        {
            var module = new CodeClassWrapper(_classList["Module"]);

            var lineDeps = new Dictionary<int, Tuple<string, string, string>>();

            foreach (var dep in deps)
            {
                var method = module.Methods.FirstOrDefault(x => x.Name.Contains("Register" + dep.Item1));
                if (method == null)
                {
                    method = module.Methods.FirstOrDefault(x => x.Name == "Install");
                }
                lineDeps.Add(method.EndLine - 1, dep);
            }

            for (int i = 0; i < lineDeps.Count; i++)
            {
                var dep = lineDeps.OrderByDescending(x => x.Key).Skip(i).First();
                module.FileCode.Insert(dep.Key, ("Container.RegisterTransient<" + dep.Value.Item2 + ", " + dep.Value.Item3 + ">();").Ind(3));
            }

            module.ProjectItem.UpdateItem(module.FileCode);
        }

        /// <summary>
        /// Создать в проекте сгенерированные классы
        /// </summary>
        /// <param name="generationResult"></param>
        public void AddToProject(ClassGenerationResult generationResult)
        {
            var IoCdeps = new List<Tuple<string, string, string>>();

            if (!string.IsNullOrEmpty(generationResult.ServiceClass))
            {
                _project.AddItem("Services", generationResult.EntityName + "Service.cs", generationResult.ServiceClass, new Dictionary<string, object>());
                _project.AddItem("Services", "I" + generationResult.EntityName + "Service.cs", generationResult.ServiceInterface, new Dictionary<string, object>());
                IoCdeps.Add(new Tuple<string, string, string>("Service", "I" + generationResult.EntityName + "Service", generationResult.EntityName + "Service"));
            }

            if (!string.IsNullOrEmpty(generationResult.ViewModelClass))
            {
                _project.AddItem("ViewModel", generationResult.EntityName + "ViewModel.cs", generationResult.ViewModelClass, new Dictionary<string, object>());
                IoCdeps.Add(new Tuple<string, string, string>("ViewModel", "IViewModel<" + generationResult.EntityName + ">", generationResult.EntityName + "ViewModel"));
            }

            if (!string.IsNullOrEmpty(generationResult.ViewModelInterface))
            {
                _project.AddItem("ViewModel", "I" + generationResult.EntityName + "ViewModel.cs", generationResult.ViewModelInterface, new Dictionary<string, object>());
            }

            if (!string.IsNullOrEmpty(generationResult.ServiceClass) || !string.IsNullOrEmpty(generationResult.ViewModelClass))
            {
                generationResult.Class.ProjectItem.UpdateItem(generationResult.Controller);
            }

            AddModuleDependencies(IoCdeps);
        }

        #region classes
        private class GenerateClassCodeResult
        {
            public string ClassCode;
            public string InterfaceCode;
            public List<string> KnownTypes = new List<string>();
            public ControllerModification Modification;
            public List<Exception> Errors = new List<Exception>();
        }

        private class ControllerModification
        {
            public List<MethodConvertResult> ConvertedMethods = new List<MethodConvertResult>();
            public List<string> UsedProperties = new List<string>();
            public List<CodeClassWrapper> UsedClasses = new List<CodeClassWrapper>();
            public List<string> AddedProperties = new List<string>();

            public List<string> RemovedNamespaces = new List<string>();
            public List<string> AddedNamespaces = new List<string>();
        }

        private class MethodConvertResult
        {
            public List<string> Code = new List<string>();
            public Tuple<Point, List<string>> ReplaceCode;
            public string Comment;
            public bool DomainServiceRequired;
            public bool IsOverride;
            public CodeFunctionWrapper Method;
            public List<string> UsedProperties;

            public string Name { get { return Method.Name; } }
        }

        private class ControllerConvertInfo
        {
            public List<string> outerUsings = new List<string>();
            public List<string> innerUsings = new List<string>();
            public List<MethodConvertResult> specificMethods = new List<MethodConvertResult>();
            public List<MethodConvertResult> otherMethods = new List<MethodConvertResult>();
            public List<CodeClassWrapper> nestedClasses = new List<CodeClassWrapper>();
            public Dictionary<string, string> deps = new Dictionary<string, string>();
        }
        #endregion
    }
}
