using UnityEngine;

namespace Sunadokei
{
    public static class InputExtension
    {
        /// <summary>
        /// GetKeyを短くした拡張関数
        /// GetKeyDown(value)
        /// </summary>
        /// <param name="value">参照するキー</param>
        /// <returns>キーを押したかBoolで返す</returns>
        public static bool IsPress(this KeyCode value)
            => Input.GetKeyDown(value);

        /// <summary>
        /// GetKeyを短くした拡張関数
        /// GetKey(value)
        /// </summary>
        /// <param name="value">参照するキー</param>
        /// <returns>キーを押しているかBoolで返す</returns>
        public static bool IsHold(this KeyCode value)
            => Input.GetKey(value);

        /// <summary>
        /// GetKeyを短くした拡張関数
        /// GetKeyUp(value)
        /// </summary>
        /// <param name="value">参照するキー</param>
        /// <returns>キーを離したかBoolで返す</returns>
        public static bool IsRelease(this KeyCode value)
            => Input.GetKeyUp(value);
    }
}