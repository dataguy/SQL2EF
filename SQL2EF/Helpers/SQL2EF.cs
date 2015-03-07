using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SQL2EF.Helpers {
    public static class SQL2EF {
        public static string SQLtoEntity(string sqlCreateTo, bool withAnnotations) {
            string sql = sqlCreateTo;
            List<fieldDef> FieldDefs = new List<fieldDef>();
            StringBuilder sb = new StringBuilder();
            string className = StringBefore(StringAfter(sql, "CREATE TABLE [dbo].["), "](");
            string fieldText = StringBefore(StringAfter(sql, "]("), "GO");
            fieldText = StringBefore(fieldText, "constraint");
            string[] fields = fieldText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            foreach (string f in fields) {
                fieldDef field = new fieldDef();

                field.DataType = GetDateType(f);

                if (field.DataType != "Unknown") {
                    field.IsKey = IsKey(f);
                    field.FieldName = StringBefore(StringAfter(f, "["), "]");
                    field.Nullable = IsNullable(f);
                    field.DataType = GetDateType(f);
                    field.MaxLength = MaxLength(f);
                    FieldDefs.Add(field);
                }
            }

            sb.Append("\tpublic class " + className + " {\r\n");

            foreach (fieldDef f in FieldDefs) {
                if (withAnnotations)
                    sb.Append(f.MaxLength + "\t\tpublic " + SetNullable(f.DataType, f.Nullable) + " " + f.FieldName + " { get; set; }\r\n"); 
                else
                    sb.Append("\t\tpublic " + SetNullable(f.DataType, f.Nullable) + " " + f.FieldName + " { get; set; }\r\n"); 
            }

            sb.Append("\t}\r\n");

            return sb.ToString();
        }
        private static string MaxLength(string sText) {
            string guess = StringBefore(StringAfter(sText, "]("), ")");

            if (IsNumeric(guess)) {
                string dtype = GetDateType(sText);

                if (dtype == "string" && sText.ToLower().IndexOf("(max)") == -1) {
                    return "\t\t[MaxLength(" + guess + ")]\r\n";
                }
            }
            return "";
        }
        private static bool IsKey(string sText) {
            return false;
        }
        private static bool IsNullable(string sText) {
            string dtype = GetDateType(sText);

            if (dtype == "string")
                return false;

            return (sText.ToLower().IndexOf("not null") == -1);
        }
        private static bool EndsIn(string sText, string sEndingText) {
            string thisText = sText.Substring(sText.Length - sEndingText.Length);

            if (thisText.ToLower() == sEndingText.ToLower())
                return true;
            else
                return false;        
        }
        private static string SetNullable(string dataType, bool nullable) {
            if (nullable) {
                return String.Format("Nullable<{0}>", dataType);
            } else {
                return dataType;
            }
        }
        private static Boolean IsNumeric(System.Object Expression) {
            if (Expression == null || Expression is DateTime)
                return false;

            if (Expression is Int16 || Expression is Int32 || Expression is Int64 || Expression is Decimal || Expression is Single || Expression is Double || Expression is Boolean)
                return true;

            try {
                if (Expression is string)
                    Double.Parse(Expression as string);
                else
                    Double.Parse(Expression.ToString());
                return true;
            } catch { } 
            return false;
        }
        private static string StringAfter(string sText, string sAfter) {
            if (sText.ToLower().IndexOf(sAfter.ToLower()) > -1) {
                int i = sText.ToLower().IndexOf(sAfter.ToLower()) + sAfter.Length;
                return sText.Substring(i);
            }
            return sText;
        }
        private static string StringBefore(string sText, string sBefore) {
            if (sText.ToLower().IndexOf(sBefore.ToLower()) > -1) {
                int i = sText.ToLower().IndexOf(sBefore.ToLower());
                return sText.Substring(0, i);
            }
            return sText;
        }
        private static string GetDateType(string sText) {
            if (sText.ToLower().IndexOf("[bigint]") > -1)
                return "Int64";

            if (sText.ToLower().IndexOf("binary]") > -1)
                return "Byte[]";

            if (sText.ToLower().IndexOf("[bit]") > -1)
                return "Boolean";

            if (sText.ToLower().IndexOf("[char]") > -1)
                return "string";

            if (sText.ToLower().IndexOf("[nchar]") > -1)
                return "string";

            if (sText.ToLower().IndexOf("[date]") > -1)
                return "DateTime";

            if (sText.ToLower().IndexOf("[datetime]") > -1)
                return "DateTime";

            if (sText.ToLower().IndexOf("[datetime2]") > -1)
                return "DateTime";

            if (sText.ToLower().IndexOf("[decimal]") > -1)
                return "Decimal";

            if (sText.ToLower().IndexOf("[float]") > -1)
                return "Double";

            if (sText.ToLower().IndexOf("[float]") > -1)
                return "Double";

            if (sText.ToLower().IndexOf("[int]") > -1)
                return "int";

            if (sText.ToLower().IndexOf("[money]") > -1)
                return "Decimal";

            if (sText.ToLower().IndexOf("[numeric]") > -1)
                return "Decimal";

            if (sText.ToLower().IndexOf("[real]") > -1)
                return "Single";

            if (sText.ToLower().IndexOf("[smallint]") > -1)
                return "Int16";

            if (sText.ToLower().IndexOf("[smallmoney]") > -1)
                return "Decimal";

            if (sText.ToLower().IndexOf("[sql_variant]") > -1)
                return "Object";

            if (sText.ToLower().IndexOf("[time]") > -1)
                return "TimeSpan";

            if (sText.ToLower().IndexOf("[tinyint]") > -1)
                return "Byte";

            if (sText.ToLower().IndexOf("[uniqueidentifier]") > -1)
                return "Guid";

            if (sText.ToLower().IndexOf("[varbinary]") > -1)
                return "Byte[]";          

            if (sText.ToLower().IndexOf("[varchar]") > -1)
                return "string";

            if (sText.ToLower().IndexOf("[nvarchar]") > -1)
                return "string";

            if (sText.ToLower().IndexOf("[text]") > -1)
                return "string";

            if (sText.ToLower().IndexOf("[ntext]") > -1)
                return "string";

            return "Unknown";
        }
    }
    public class fieldDef {
        public bool IsKey { get; set; }
        public string DataType { get; set; }
        public string FieldName { get; set; }
        public bool Nullable { get; set; }
        public string MaxLength { get; set; }
    }
}