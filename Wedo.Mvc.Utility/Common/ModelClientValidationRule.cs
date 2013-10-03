using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace System.Web.Mvc
{
    public class ModelClientValidationEmailRule : ModelClientValidationRule
    {
        public ModelClientValidationEmailRule(string errorMessage)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "email";
        }
    }

    public class ModelClientValidationNumberRule : ModelClientValidationRule
    {
        public ModelClientValidationNumberRule(string errorMessage, float min, float max, int precision)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationParameters.Add("min", min);
            base.ValidationParameters.Add("max", max);
            base.ValidationParameters.Add("precision", precision);
            base.ValidationType = "dnumber";
        }
    }

    public class ModelClientValidationUrlRule : ModelClientValidationRule
    {
        public ModelClientValidationUrlRule(string errorMessage)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "url";
        }
    }
    public class ModelClientValidationDateRule : ModelClientValidationRule
    {
        public ModelClientValidationDateRule(string errorMessage)
        {
            base.ErrorMessage = errorMessage;
            base.ValidationType = "date";
        }
    }
    /// <summary>
    /// 验证字段为 Email
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class EmailAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "字段 '{0}' 必须为电子邮件格式。";
        private static Regex m_regex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

        public EmailAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name);
        }

        public override bool IsValid(object value)
        {
            return m_regex.IsMatch(value.ToString());
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationEmailRule(FormatErrorMessage(metadata.GetDisplayName()))
            };
        }
    }
    /// <summary>
    /// 验证数字
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class NumberAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "字段 '{0}' 必须大于 {1} 小于 {2} 且小数点后最多为 {3} 位。";

        /// <summary>
        /// 允许的最大值 缺省为 decimal.MaxValue
        /// </summary>
        public float Max { get; set; }
        /// <summary>
        /// 允许的最小值 缺省为 decimal.MinValue
        /// </summary>
        public float Min { get; set; }
        /// <summary>
        /// 小数点的位数
        /// </summary>
        public int Precision { get; set; }

        public NumberAttribute()
            : this(float.MinValue, float.MaxValue, 0)
        {
        }
        private NumberAttribute(float min, float max, int precision)
            : base(_defaultErrorMessage)
        {
            Max = max;
            Min = min;
            Precision = precision;
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, Min, Max, Precision);
        }

        public override bool IsValid(object value)
        {
            float? v = value.ChangeType<float>();
            if (v.HasValue)
            {
                int prec = 0;
                if (v.ToString().Contains("."))
                    prec = v.ToString().Split('.')[1].Length;
                if (v >= Min && v <= Max && prec <= Precision)
                    return true;
            }
            return false;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationNumberRule(FormatErrorMessage(metadata.GetDisplayName()),Min,Max,Precision)
            };
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class UrlAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "字段 '{0}' 必须为有效的网址。";
        private static Regex m_regex = new Regex(@"[a-zA-z]+://[^\s]*");
        public UrlAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name);
        }

        public override bool IsValid(object value)
        {
            return m_regex.IsMatch(value.ToString());
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationUrlRule(FormatErrorMessage(metadata.GetDisplayName()))
            };
        }
    }
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DateAttribute : ValidationAttribute, IClientValidatable
    {
        private const string _defaultErrorMessage = "字段 '{0}' 必须为有效的网址。";
        public DateAttribute()
            : base(_defaultErrorMessage)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name);
        }

        public override bool IsValid(object value)
        {
            return value is DateTime;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            return new[]{
                new ModelClientValidationDateRule(FormatErrorMessage(metadata.GetDisplayName()))
            };
        }
    }
}
