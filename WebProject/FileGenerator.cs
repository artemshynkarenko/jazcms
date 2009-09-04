using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using JazCms.Kernel;
using System.Reflection;

namespace JazCms.WebProject
{
    public static class FileGenerator
    {
        public static CodeCompileUnit BuildJAZContent(string jazNamespace, string JazClassName, string sourceFilePath)
        {

            CodeCompileUnit compileUnit = new CodeCompileUnit();

            CodeNamespace samples = new CodeNamespace(jazNamespace);
            compileUnit.Namespaces.Add(samples);

            samples.Imports.Add(new CodeNamespaceImport("JazCms.Kernel"));

            CodeTypeDeclaration jazClass = new CodeTypeDeclaration(JazClassName);
            jazClass.IsPartial = true;
            jazClass.IsClass = true;

            jazClass.BaseTypes.Add("IPageInstance");
            List<Type> typeCollection = ((typeof(IPageInstance))).UnderlyingSystemType.GetInterfaces().ToList();

            CodeTypeReferenceExpression csSystemConsoleType = new CodeTypeReferenceExpression("throw new System");
            CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
            csSystemConsoleType, "NotImplementedException", new CodePrimitiveExpression());
         
            foreach (Type interf in typeCollection)
            {
                List<string> propertyNames = new List<string>();
                IList<MemberInfo> memberInfoCollection = interf.GetMembers().ToList();
                foreach (MemberInfo member in memberInfoCollection)
                {
                    if (member.MemberType == MemberTypes.Property)
                    {
                        CodeMemberProperty propertyCTM = PropertyGenerator.GenerateProperty(member, cs1, cs1);
                        jazClass.Members.Add(propertyCTM);
                    }
                    else if (member.MemberType == MemberTypes.Method)
                    {
                        List<MemberInfo> setMethodsListForProperty = memberInfoCollection.Where(
                         c => c.MemberType == MemberTypes.Property &&
                             (member.Name.Contains("set_" + c.Name) || member.Name.Contains("get_" + c.Name))
                         ).ToList();
                        if (setMethodsListForProperty.Count() == 0)
                        {
                            CodeMemberMethod methodCTM = MethodGenerator.GenerateMethod(member, cs1);
                            jazClass.Members.Add(methodCTM);
                        }
                    }
                }
            }
            samples.Types.Add(jazClass);

           return compileUnit;
        }

        public static void GenerateCode(CodeDomProvider provider,
           CodeCompileUnit compileunit, string sourceFilePath)
        {
            String sourceFile = sourceFilePath;
            IndentedTextWriter itw = new IndentedTextWriter(new StreamWriter(sourceFile, false), "    ");
            CodeGeneratorOptions cgo = new CodeGeneratorOptions();
            provider.GenerateCodeFromCompileUnit(compileunit, itw, new CodeGeneratorOptions());
            itw.Close();
        }

    }
}
