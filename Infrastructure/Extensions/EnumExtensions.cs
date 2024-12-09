using System;
using System.ComponentModel;
using System.Reflection;

namespace TGBot_TW_Stock_Webhook.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 獲取枚舉成員的 Description 屬性值
        /// </summary>
        /// <param name="value">枚舉值</param>
        /// <returns>Description 屬性值，如果未設置則返回枚舉成員的名稱</returns>
        public static string GetDescription(this System.Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));

            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
