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
                    if (member.MemberType.ToString() == "Property")
                    {
                        List<MemberInfo> setMethodsList = memberInfoCollection.Where(
                            c => c.MemberType.ToString() == "Method" && c.Name.Contains("set_" + member.Name)
                            ).ToList();

                        CodeMemberProperty propertyCTM = new CodeMemberProperty();
                        propertyCTM.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                        propertyCTM.Name = member.Name;
                        CodeTypeReference typeRef = new CodeTypeReference(((PropertyInfo)(member)).PropertyType);
                        propertyCTM.Type = typeRef;
                        if (setMethodsList.Count > 0)
                        {
                            propertyCTM.SetStatements.Add(cs1);
                        }
                        propertyCTM.GetStatements.Add(cs1);
                        jazClass.Members.Add(propertyCTM);
                    }
                    else if (member.MemberType.ToString() == "Method")
                    {
                        List<MemberInfo> setMethodsListForProperty = memberInfoCollection.Where(
                         c => c.MemberType.ToString() == "Property" &&
                             (member.Name.Contains("set_" + c.Name) || member.Name.Contains("get_" + c.Name))
                         ).ToList();
                        if (setMethodsListForProperty.Count() == 0)
                        {
                            CodeMemberMethod methodCTM = new CodeMemberMethod();
                            methodCTM.Name = member.Name;
                            methodCTM.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                            CodeTypeReference methodTypeRef = new CodeTypeReference(((MethodInfo)(member)).ReturnType);
                            methodCTM.ReturnType = methodTypeRef;

                            List<ParameterInfo> methodParameters = ((System.Reflection.MethodInfo)(member)).GetParameters().ToList();
                            CodeParameterDeclarationExpressionCollection parameterCollection = new CodeParameterDeclarationExpressionCollection();
                            foreach (ParameterInfo info in methodParameters)
                            {
                                CodeParameterDeclarationExpression expresion = new CodeParameterDeclarationExpression(info.ParameterType, info.Name);
                                parameterCollection.Add(expresion);
                            }
                            methodCTM.Parameters.AddRange(parameterCollection); 
                            jazClass.Members.Add(methodCTM);
                            methodCTM.Statements.Add(cs1);
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
