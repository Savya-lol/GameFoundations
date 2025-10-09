using UnityEngine;

namespace Darkmatter.Core.Extensions
{
    public static class StringExtensions 
    {
       public static bool IsNullOrEmpty(this string str)
       {
           return string.IsNullOrEmpty(str);
       }
    }
}
