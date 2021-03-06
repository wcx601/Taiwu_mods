﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace Majordomo
{
    public class Common
    {
        // 在非 monospace 字体下对齐用
        public static readonly char BLANK_CN = '\u3000';


        /// <summary>
        /// 半角字串转全角
        /// 半角空格 0x20 对应全角 0x3000
        /// 空格以上，其他字符半角 (0x21 ~ 0x7E) 与全角 (0xFF01 ~ 0xFF5E) 均相差 0xFEE0
        /// </summary>
        /// <param name="text"></param>
        public static string ToFullWidth(string text)
        {
            char[] arrText = text.ToCharArray();
            for (int i = 0; i < arrText.Length; ++i)
            {
                if (arrText[i] == 0x20)
                    arrText[i] = (char)0x3000;
                else if (arrText[i] > 0x20 && arrText[i] <= 0x7E)
                    arrText[i] = (char)(arrText[i] + 0xFEE0);
            }
            return new string(arrText);
        }


        /// <summary>
        /// 删除不需要的子对象（用于部分复制对象时）
        /// 注意，比较的是 GameObject 的名称，而不是 Transform 的
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childrenNames">Name of children GameObjects</param>
        /// <param name="deleteImmediate"></param>
        public static void RemoveChildren(GameObject parent, List<string> childrenNames, bool deleteImmediate = false)
        {
            var parentTrans = parent.transform;
            for (int i = 0; i < parentTrans.childCount; ++i)
            {
                var child = parentTrans.GetChild(i).gameObject;
                if (childrenNames.Contains(child.name))
                {
                    if (deleteImmediate)
                        UnityEngine.Object.DestroyImmediate(child);
                    else
                        UnityEngine.Object.Destroy(child);
                }
            }
        }


        /// <summary>
        /// 删除所有子对象（用于部分复制对象时）
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="deleteImmediate"></param>
        public static void RemoveChildren(GameObject parent, bool deleteImmediate = false)
        {
            var parentTrans = parent.transform;
            for (int i = 0; i < parentTrans.childCount; ++i)
            {
                var child = parentTrans.GetChild(i).gameObject;
                if (deleteImmediate)
                    UnityEngine.Object.DestroyImmediate(child);
                else
                    UnityEngine.Object.Destroy(child);
            }
        }


        /// <summary>
        /// 返回指定名称的子对象
        /// 注意，比较的是 GameObject 的名称，而不是 Transform 的
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childName">Name of child GameObject</param>
        /// <returns></returns>
        public static GameObject GetChild(GameObject parent, string childName)
        {
            var parentTrans = parent.transform;
            for (int i = 0; i < parentTrans.childCount; ++i)
            {
                var child = parentTrans.GetChild(i).gameObject;
                if (child.name == childName) return child;
            }

            return null;
        }


        /// <summary>
        /// 移除指定 Component，可选择是否需要重建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="recreate"></param>
        /// <returns>需要重建时，返回重建的组件；不需要重建时，返回 null</returns>
        public static T RemoveComponent<T>(GameObject gameObject, bool recreate = false) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (recreate)
            {
                if (component != null) UnityEngine.Object.DestroyImmediate(component);
                return gameObject.AddComponent<T>();
            }
            else
            {
                if (component != null) UnityEngine.Object.Destroy(component);
                return null;
            }
        }


        /// <summary>
        /// 平移 UI（操作其 RectTransform 组件）
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void TranslateUI(GameObject gameObject, float x, float y)
        {
            var rectTransform = gameObject.GetComponent<RectTransform>();
            if (rectTransform == null) throw new Exception("GameObject doesn't contain RectTransform component.");

            var pos = rectTransform.localPosition;
            pos.x += x;
            pos.y += y;
            rectTransform.localPosition = pos;
        }


        /// <summary>
        /// 把 List 分为指定长度的多个 List，最后一个 List 的长度可能会小于指定长度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> SplitList<T>(List<T> source, int size)
        {
            for (int i = 0; i < source.Count; i += size)
                yield return source.GetRange(i, Math.Min(size, source.Count - i));
        }
    }
}
