using System.Collections.Generic;


namespace ATools
{
    /// <summary>
    /// DictionaryExtension 字典扩展
    /// </summary>
    public static class DictionaryExtension
    {

        /// <summary>
        /// 合并两个字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionaries1">字典类型一</param>
        /// <param name="dictionaries2">字典类型二</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> MergeExtension<TKey, TValue>(this Dictionary<TKey, TValue> dictionaries1, Dictionary<TKey, TValue> dictionaries2)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries1)      
                    result[dict.Key] = dict.Value;
            foreach (var dict in dictionaries2)
                 result[dict.Key] = dict.Value;
            return result;
        }


        /// <summary>
        /// 合并两个字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionaries1">字典类型一</param>
        /// <param name="dictionaries2">字典类型二</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(Dictionary<TKey, TValue> dictionaries1, Dictionary<TKey, TValue> dictionaries2)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries1)
                result[dict.Key] = dict.Value;
            foreach (var dict in dictionaries2)
                result[dict.Key] = dict.Value;
            return result;
        }


        /// <summary>
        /// 合并字典多个字典
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionaries"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var dict in dictionaries)
                foreach (var x in dict)
                    result[x.Key] = x.Value;
            return result;
        }
    }
}