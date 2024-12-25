using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public static partial class StaticUtils
{
	private static BindingFlags flagGetInstanceThing = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
	private static BindingFlags flagGetStaticThing = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	#region call function

	public static object CallStaticFunction(Type type, string funcName, object[] parameters)
	{
		return CallFunction(type, null, funcName, parameters, flagGetStaticThing);
	}

	public static object CallInstanceFunction(object target, string funcName, object[] parameters)
	{
		return CallFunction(target.GetType(), target, funcName, parameters, flagGetInstanceThing);
	}

	private static object CallFunction(Type type, object target, string funcName, object[] parameters, BindingFlags bindingFlags)
	{
		if (parameters == null)
		{
			parameters = new object[0];
		}

		Type[] paramTypes = new Type[parameters.Length];
		for (var i = 0; i < parameters.Length; i++)
		{
			paramTypes[i] = parameters[i].GetType();
		}

		var method = type.GetMethod(funcName, bindingFlags, null, paramTypes, null);
		return method.Invoke(target, parameters);
	}

	#endregion

	#region get/set prop/field

	public static T GetProperty<T>(object target, string name)
	{
		var type = target.GetType();
		var property = type.GetProperty(name, flagGetInstanceThing);
		return (T)property.GetValue(target);
	}

	public static T GetField<T>(object target, string name)
	{
		var type = target.GetType();
		var field = type.GetField(name, flagGetInstanceThing);
		return (T)field.GetValue(target);
	}

	public static void SetProperty(object target, string name, object val)
	{
		var type = target.GetType();
		var property = type.GetProperty(name, flagGetInstanceThing);
		property.SetValue(target, val);
	}

	public static void SetField(object target, string name, object val)
	{
		var type = target.GetType();
		var field = type.GetField(name, flagGetInstanceThing);
		field.SetValue(target, val);
	}

	#endregion

	#region get type

	public static Type GetType(string typeNameSpace, string typeName, string dllName, string dllVersion)
	{
		var name = $"{typeNameSpace}.{typeName}, {dllName}, Version={dllVersion}, Culture=neutral, PublicKeyToken=null";
		return Type.GetType(name);
	}

	public static Type GetFieldType(Type type, string fieldName)
	{
		return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public).FieldType;
	}

	#endregion

	#region list field/prop etc name

	public static List<string> ListFieldName(Type type, Type attributeType)
	{
		var fields = type.GetFields(flagGetInstanceThing);
		var result = new List<string>();
		foreach (var i in fields)
		{
			if (i.CustomAttributes.Any(x => x.AttributeType == attributeType))
			{
				result.Add(i.Name);
			}
		}
		return result;
	}

	#endregion
}