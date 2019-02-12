// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Workbeat.Newtonsoft.Json
{
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class JObjectValueInspector
    {
        public static object JObjectToObject(JObject obj, JsonSerializerSettings settings, Dictionary<int, Type> typesDict = null)
        {
            Type type;
            typesDict = typesDict ?? new Dictionary<int, Type>();
            var jObjType = obj["type"];

            // var strObj = global::Newtonsoft.Json.JsonConvert.SerializeObject(obj["properties"], settings);
            var strType = $"{jObjType["namespace"]}.{jObjType["name"]}";
            if (strType == ".")
            {
                var refId = (int)jObjType["$ref"];
                type = typesDict[refId];
            }
            else
            {
#if !NETSTANDARD1_X
                type = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName.Equals(strType));
#else
                type = Type.GetType(strType);
#endif
            }

            var isGeneric = jObjType["isGenericType"]?.Value<bool?>() ?? false;
            if (isGeneric)
            {
                var id = (int)jObjType["$id"];
                Type[] types = jObjType["genericArguments"].Select(t =>
                {
                    var s1 = $"{t["namespace"]}.{t["name"]}";
                    var id1 = (int)t["$id"];
#if !NETSTANDARD1_X
                    var t1 = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetTypes()).FirstOrDefault(t2 => t2.FullName.Equals(s1));
#else
                    var t1 = Type.GetType(s1);
#endif
                    if (!typesDict.ContainsKey(id1))
                    {
                        typesDict.Add(id1, t1);
                    }

                    return t1;
                }).ToArray();
                type = type.MakeGenericType(types);
                if (!typesDict.ContainsKey(id))
                {
                    typesDict.Add(id, type);
                }
            }

            object newObj = Activator.CreateInstance(type);
            foreach (var prop in obj["properties"])
            {
                var p = type.GetProperty(prop["name"].ToString());
                var value = prop["value"];

                if (p.PropertyType.IsValueType || p.PropertyType == typeof(string))
                {
                    var pt = Nullable.GetUnderlyingType(p.PropertyType);
                    p.SetValue(newObj, Convert.ChangeType(JTokenToPrimitive(value), pt ?? p.PropertyType));
                }
                else if (p.PropertyType.IsArray && value is JArray)
                {
                    Array newArray = Array.CreateInstance(p.PropertyType.GetElementType(), ((JArray)value).Count());
                    for (int i = 0; i < ((JArray)value).Count(); i++)
                    {
                        var v = value[i];
                        newArray.SetValue(JObjectToObject((JObject)v, settings, typesDict), i);
                    }

                    p.SetValue(newObj, newArray);
                }
                else
                {
                    p.SetValue(newObj, JObjectToObject((JObject)value, settings, typesDict));
                }
            }

            // var newObj = global::Newtonsoft.Json.JsonConvert.DeserializeObject(strObj, type, settings);
            return newObj;
        }

        private static object JTokenToPrimitive(JToken obj)
        {
            var tp = obj.Type;
            object obj1 = null;
            switch (tp)
            {
                case JTokenType.Boolean:
                    obj1 = obj.ToObject(typeof(bool));
                    break;
                case JTokenType.String:
                    obj1 = obj.ToObject(typeof(string));
                    break;
                case JTokenType.Float:
                    obj1 = obj.ToObject(typeof(float));
                    break;
                case JTokenType.Guid:
                    obj1 = obj.ToObject(typeof(Guid));
                    break;
                case JTokenType.Integer:
                    obj1 = obj.ToObject(typeof(int));
                    break;
                case JTokenType.TimeSpan:
                    obj1 = obj.ToObject(typeof(TimeSpan));
                    break;
                case JTokenType.Uri:
                    obj1 = obj.ToObject(typeof(Uri));
                    break;
                case JTokenType.Date:
                    obj1 = obj.ToObject(typeof(DateTime));
                    break;
                default:
                    obj1 = obj;
                    break;
            }

            return obj1;
        }
    }
}
