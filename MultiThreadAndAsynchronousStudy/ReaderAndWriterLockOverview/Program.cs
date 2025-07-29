namespace ReaderAndWriterLockOverview
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Dictionary<int, string> sharedDic = new Dictionary<int, string>();

            ReaderWriterLockSlim readerWriterLockSlim = new ReaderWriterLockSlim();

            ///保守风格,代码会应用在不确定的环境比如这时Library 代码
            /// 使用一个bool 变量作为确认锁获取成功的标记
            /// 将EnterXXXLock()写在try里
            /// 菜鸟阶段就用这种风格
            void AddData(int id, string value)
            {

                bool lockAcquired = false; // 防止ReaderWriterLock在获取权限前发生异常

                try
                {
                    readerWriterLockSlim.EnterWriteLock();
                    lockAcquired = true; // 确认已经获取了Lock, 后续退出锁可以顺利执行
                    sharedDic[id] = value;
                }
                finally
                {
                    if (lockAcquired) // 防止没有获取权限还执行退出Lock的代码
                    {
                        readerWriterLockSlim.ExitWriteLock();
                    }

                }
            }

            string? GetData(int id)
            {
                bool lockAcquired = false; // 防止ReaderWriterLock在获取权限前发生异常

                try
                {
                    readerWriterLockSlim.EnterReadLock();
                    lockAcquired = true;    // 确认已经获取了Lock, 后续退出锁可以顺利执行
                    return sharedDic.TryGetValue(id, out string value) ? value : null;
                }
                finally
                {
                    if (lockAcquired) // 防止没有获取权限还执行退出Lock的代码
                    {
                        readerWriterLockSlim.ExitReadLock();
                    }
                }
            }

            ///这个风格是常规推荐风格,在自己知道获取锁不会失败的情况下
            ///高手阶段之后用这种风格
            void CommonStyle()
            {
                // 如果不确定可以在外面包一层 if 判断,确认各种权限都没有被占用
                if (!readerWriterLockSlim.IsWriteLockHeld && !readerWriterLockSlim.IsReadLockHeld)
                {
                    readerWriterLockSlim.EnterWriteLock(); // 放心进锁
                    try
                    {
                        // 临界区
                    }
                    finally
                    {
                        readerWriterLockSlim.ExitWriteLock();
                    }
                }

            }
        }
    }
}