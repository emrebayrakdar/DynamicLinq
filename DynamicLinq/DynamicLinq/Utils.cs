using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DynamicExpression
{
    public static class Utils
    {
        /// <summary>
        /// Propertysini Döndürmek İçin Kullanılır
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static dynamic GetProperty(this object obj, string propName)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            Type t = obj.GetType();
            PropertyInfo propertyInfo = null;
            while (propertyInfo == null && t != null)
            {
                propertyInfo = t.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                t = t.BaseType;
            }
            if (propertyInfo == null)
                throw new ArgumentOutOfRangeException("propName", string.Format("{0} içerisinde {1} bulunamadı.", obj.GetType().FullName, propName));
            return propertyInfo;
        }
    }

}
