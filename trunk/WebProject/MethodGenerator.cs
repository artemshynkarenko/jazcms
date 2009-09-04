using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace JazCms.WebProject
{
    public static class MethodGenerator
    {
        public static CodeMemberMethod GenerateMethod(MemberInfo member, CodeMethodInvokeExpression statment)
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
            methodCTM.Statements.Add(statment);
            return methodCTM;
        }
    }
}