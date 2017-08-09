namespace Framework.Common.Extensions
{
    /// <summary>
    /// Object类的扩展
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// 指定对象的复制器
        /// </summary>
        /// <param name="s">源对象</param>
        /// <param name="t">目标对象</param>
        public static void CopyTo(this object s, object t)
        {
            foreach (var pS in s.GetType().GetProperties())
            {
                foreach (var pT in t.GetType().GetProperties())
                {
                    if (pT.Name == pS.Name && pT.CanWrite)
                    {
                        if (pS.GetValue(s) != null)
                            pT.SetValue(t, pS.GetValue(s));
                        break;
                    }
                }
            };
        }

    }
}
