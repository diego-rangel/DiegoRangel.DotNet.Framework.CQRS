﻿using System;
using System.ComponentModel;

namespace DiegoRangel.DotNet.Framework.CQRS.Infra.CrossCutting.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum element)
        {
            var type = element.GetType();
            var memberInfo = type.GetMember(element.ToString());

            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                    return ((DescriptionAttribute)attributes[0]).Description;
            }

            return element.ToString();
        }
    }
}