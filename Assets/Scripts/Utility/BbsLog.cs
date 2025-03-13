
public static class BbsLog
{
   [System.Diagnostics.Conditional("ENABLE_BBS_LOG")]
   public static void Log(object message)
   {
        UnityEngine.Debug.Log(message);
   }

   [System.Diagnostics.Conditional("ENABLE_BBS_LOG")]
   public static void LogError(object message)
   {
        UnityEngine.Debug.LogError(message);
   }
}
