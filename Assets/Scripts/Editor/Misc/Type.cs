using System.Collections.Generic;
namespace YooAssetFrame.Editor
{
    internal static class Type
    {
        /// <summary>
        /// 在运行时或编辑器程序集中获取指定基类的所有子类的名称。
        /// </summary>
        /// <param name="typeBase">基类类型。</param>
        /// <returns>指定基类的所有子类的名称。</returns>
        internal static List<string> GetRuntimeOrEditorTypeNames(System.Type typeBase)
        {
            List<string> typeNames = new List<string>();
            System.Type parentType = typeBase;
            var assembly = parentType.Assembly;//获取当前父类所在的程序集``
            var assemblyAllTypes = assembly.GetTypes();//获取该程序集中的所有类型
            foreach (var itemType in assemblyAllTypes)//遍历所有类型进行查找
            {
                var baseType = itemType.BaseType;//获取元素类型的基类
                if (baseType != null)//如果有基类
                {
                    if (baseType.Name == parentType.Name)//如果基类就是给定的父类
                    {
                        typeNames.Add(itemType.FullName);
                    }
                }
            }
            return typeNames;
        }
    }
}