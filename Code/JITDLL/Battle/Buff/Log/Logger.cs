using UnityEngine;

namespace BUFF
{
    public class Logger
    {
        private const string TAG = LogTag.SKILL; 

        private const string DEFAULT_COLOR = "grey";

        private const string ERROR_COLOR = "red";

        public const string ATTACK_COLOR = DEFAULT_COLOR;

        public const string BUFF_COLOR = "yellow";

        public const string CUBE_COLOR = "green";

        public const string BEHAVIOR_COLOR = "cyan";

        public static void Assert(bool condition, string message)
        {
#if UNITY_EDITOR
            Debug.Assert(condition, message);
#endif
        }

        public static void Log(string message, string color = DEFAULT_COLOR)
        {
#if UNITY_EDITOR
            string format = "<color=" + color + ">" + TAG + " " + message + "</color>";
            Debug.Log(format);
#endif
        }

        public static void LogError(string message)
        {
#if UNITY_EDITOR
            string format = "<color=" + ERROR_COLOR + ">" + TAG + " " + message + "</color>";
            Debug.Log(format);
#endif
        }
    }
}
