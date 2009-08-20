using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

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

            samples.Types.Add(jazClass);

            // Declare a new code entry point method.
            //CodeEntryPointMethod start = new CodeEntryPointMethod();

            // Create a type reference for the System.Console class.
            //CodeTypeReferenceExpression csSystemConsoleType = new CodeTypeReferenceExpression("System.Console");

            //CodeMethodInvokeExpression cs1 = new CodeMethodInvokeExpression(
            //    csSystemConsoleType, "WriteLine",
            //    new CodePrimitiveExpression("Hello World!"));
            //start.Statements.Add(cs1);

            // Add the code entry point method to
            // the Members collection of the type.
            //jazClass.Members.Add(start);

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
