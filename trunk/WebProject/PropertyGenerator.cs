using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.Reflection;

namespace JazCms.WebProject
{
    public static class PropertyGenerator
    {
        public static CodeMemberProperty GenerateProperty(MemberInfo member,CodeMethodInvokeExpression getStatment, 
                                                           CodeMethodInvokeExpression setStatment)
        {
            PropertyInfo property = member as PropertyInfo;
            CodeMemberProperty propertyCTM = new CodeMemberProperty();
            propertyCTM.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            propertyCTM.Name = member.Name;
            CodeTypeReference typeRef = new CodeTypeReference(((PropertyInfo)(member)).PropertyType);
            propertyCTM.Type = typeRef;
            if (property.CanWrite)
            {
                propertyCTM.SetStatements.Add(setStatment);
            }
            if (property.CanRead)
            {
                propertyCTM.GetStatements.Add(getStatment);
            }
            return propertyCTM;
        }

    }
}
