# MultiThreads and Asynchronous programming
## Basic Concepts

### Program - 程序
是开发者写完存储在计算机硬盘上的静态代码。

### Process - 进程
是程序运行后在内存中的实例，是资源的分配单位（拥有独立的地址空间，文件句柄等）。
一个进程中可以包含多个线程，并共享进程的资源(内存，文件等)。

### Thread - 线程
线程是CPU实际调度的执行单元，是CPU调度的最小的单位。CPU执行的是*线程*的指令流。
它使用它所在的进程的资源。

#### 线程的认识澄清误区
1. CPU调动的是线程；
2. 线程可以在多个CPU核之间迁移；
3. 实际上如果没有多核或者CPU密集型任务，多线程反而有开销。
最后一点自己已经通过基础运算测试了。

CPU 视角
CPU 实际调度的是 线程的上下文（CPU 寄存器+程序计数器等），即：
1. CPU 不关心“进程”，它调度的是“线程”；
2. 一个 CPU 核心在某个时间点只执行一个线程的代码（超线程技术除外）；
3.多核 CPU 可以同时执行多个线程（物理并行）；

### Thread Affinity - 线程关联性
是指在多线程编程中，某些操作或者资源必须有创建它们的原始线程访问或者修改的特性。

### Thread Synchronization - 线程同步
是指在`多线程编程`中， **系欸套多个线程对共享资源的访问**，以确保程序运行的正确性、数据一致性，防止`Race  Condition` 和 `DeadLock` 等问题的机制和技术

### Race Condition
是指多个线程同时访问共享资源时，因为执行顺序的不确定性导致的程序行为不符合预期的现象。

### Dead Lock
是指两个或多个线程互相持有对方锁需要的资源，并无限期等待对方释放，从而导致所有相关线程永久阻塞的现象。
当出现 deadlock 时，程序就相当于"卡死"，无法继续执行。

### Critical Section
是一段操作  `共享资源`  的代码片段。 也是可以理解为最容易收到Race Condition影响的代码段。 Thread Synchronization 的机制就需要围绕这段代码进行设计。例如使用 `Lock`或者`Monitor`等方法锁定资源。

### atomic operation - 原子操作
是指一个不可中断的执行单元。要么完全执行，要么不执行。必须确保数据的一致性，在操作期间，其他线程无法观察到中间状态（如读取到部分修改的值）。核心就是数据的`读取-修改-写入`过程是连续完成的。

## 基础API
### Thread静态类
#### Thread.CurrentThread
获取当前的线程实例。
##### Thread.CurrentThread.ManagedThreadID
获取当前线程的ID。
#### Thread.Sleap(int duration)
当前线程休眠 `duration` 毫秒。
### Thread 实例
#### 声明一个线程
```c#
    Thread t = new Thread( Action callback  );
    t.Start();
```
#### Start() - 启动线程
启动线程实例。

#### Name属性 - string
可以获取或为这个线程命名。

#### IsBackgroundThread 属性 - bool

### CancellationTokenSource - 取消Token
一个`取消遥控器`用来生成一个`取消Token`并控制这个Token的状态

```C#
    CancellationTokenSource cts = new CancellationTokenSource();
```

#### CancellationToken
可通过`CancellationTokenSource`获得
```c#
    CancellationToken token= cts.Token;
```

##### .IsCancellationRequested 属性 - bool
```C#
        if(token.IsCancellationRequested) // 如果token状态是 true - 即被要求取消
        {            
            // 执行代码
        }
``` 
#### Cancel() 方法
设置 CTS 生成的 Token的实例状态 为 `true`
```C#
    cts.Cancel();
```

### Lock - 语法
`Lock` 是 `Monitor.Enter()`与`Monitor.Exit()`执行组合的高级封装
```C#
    object myLock = new object(); // 声明一个空变量 代表资源权限或者理解为钥匙

    // 启用权限，将Lock代码体中的代码进行隔离 

    Lock(myLock) 
    {
        // critical section code...
        // manipulate shared data
    }   
    // 其他线程只能等待当前线程走完 Lock代码体后才能访问代码提中访问的共享数据
```

### Monitor - 资源监控器
#### Monitor.Enter() 与 Monitor.Exit()
这个组合通常配合`try.. finally` 语句块搭配使用
```c#
    object myLock = new object(); // 声明一个空变量 代表资源权限或者理解为钥匙

    Monitor.Enter(myLock)  // 获取权限
    try
    {
        // critical section
    }
    finally  // 通过finally 语句确保权限释放
    {
        Monitor.Exit(myLock);  // 释放权限
    }
```

### Mutex - 跨进程锁
**注意：这是用来跨进程的锁**
`Mutex` 的性质与 `Lock` 很像，但是更重 - 性能开销更大，更慢。
它的主要作用是当一个程序运行时，它内部有更多的进程需要共享资源的时候，为了保证 Synchronization，就可以使用`Mutex`。 否则进程内的线程之间做同步管理就考虑 `Lock`或者`Monitor`。

```C#
    /// 语法与Monitor很像， 但是不需要一个空object作为权限标识符
    /// 构造函数的第一个参数 bool initiallyOwned 代表是否在创建这个Mutex的进程上立刻获取资源权限，如果 true 则可以省略mtx.WaitOne(),但这一般为了实现单例控制
    /// 如果是false，就需要手动调用 .WaitOne()
    /// 一般如果是为了挂进程管理，都是false
    /// 第二个参数是给这个Mutex实例命名，后面可以根据这个名字追踪它
    using (Mutex mtx = new Mutex(false, "ItsGlobalName") )
    {
        mtx.WaitOne()
        try
        {
            // critical section
        }
        finally
        {
            mtx.ReleaseMutex();
        }
    }
```

### ReaderWriterLockSlim - 读写锁
专门用于多线程应用程序共同访问(**仅读**)共享资源。

与`Lock Monitor Mutex`不同， `ReaderWriterLockSlim` 读写锁不是 *Exclusive Lock*。当通过它获取 `Reader 权限`时，它允许其他线程获取读取shared resource的权限，同时不允许任何线程进行`写入`；当通过它获取 `Writer 权限`时，就编程常规的 *Exclusive Lock*效果，只能当前线程写入，其他线程不可读，也不可写。

```C#
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
```
### Semaphore And SemaphoreSlim - 信号量
这是一种通过限制 `资源/代码` 访问数量的线程同步技术。它做的是**通过记数的形式限制一定数量的线程来访问被控制的代码或者资源片段。**

**重要的是：**
1. 它 ***不保护 `Critical Section`*** ！！！所以要使用Semaphore相关技术要注意自己去使用其他 *保护 Critical Section* 的技术对共享资源进行锁定。

2. 它可以在其他线程或进程中释放。不受 Thread Affinity限制。


`Semaphore` 是可以跨进程的版本，与`Mutex`一样构造函数中包含一个 *可命名* 的版本，并用传入的参数名称来获得全局访问。

`SemaphoreSlim` 是进程内的跨线程版本，不可跨进程。


```C#

    // 或 using   SemaphoreSlim ssm = new SemaphoreSlim(initialCount:yourNum, maxCount:yourNum);
    // 取决于代码风格和上下文状态

    // 构造函数中声明初始的可用信号数量，和最大信号数量。如果二者一致就是固定数量的信号量
    SemaphoreSlim ssm = new SemaphoreSlim(initialCount:yourNum, maxCount:yourNum); 


    ssm.Wait(); // 获取到权限后可用信号量 减一， 当可用信号量为0时其他线程只能等待
    try
    {
        // 被控制的代码体
    }
    finally
    {
        ssm.Release()； // 释放信号源，每释放一个信号量恢复1个
    }
```

### AutoResetEvent - 1对1信号控制器
`AutoResetEvent` 也是一个信号控制性线程同步技术。
信号是一个2进制信号，只有`on - true` 和 `off - false`。 `当 `通行信号` 发出时 (由调用实例方法 `.Set()` 发出)，等待的权限的线程可获得 `通行权限`进入相应代码段：
1. ***一次只能由一个等待线程获取权限***;
2. ***通行信号发出后会立刻切换回 false 状态。无论是否有线程等待，通行信号的效能都不累计***
3. 与`Semaphore`一样，都不保护`Critical Section`,只起到`阻塞和协调权限`作用
4. 通常使用是，会有一个或若干个线程排队`.WaitOne()`,另一个线程`.Set()`

基础语法：
```c#
    AutoResetEvent are= new AutoResetEvent(false)； // false代表一开始就是阻拦状态, true是打开通行信号。大多数情况都会采用false

    are.WaitOne(); // 排队等待权限
    // code 获取权限开始执行代码

    are.Set(); // 确认开启通行信号
```
代码示例：
```C#
        internal class Program
    {
        static void Main(string[] args)
        {
            AutoResetEvent are = new AutoResetEvent(false); // 设置初始状态为false

            Console.WriteLine("Farmer is making food....");

            for (int i = 0; i < 5; i++) {

                Thread pig = new Thread(() => PigFeed(are));
                pig.Name = $"Pig {i}";
                pig.Start();
            }

            while (true)
            {
                string input = Console.ReadLine()??"";
                if (input =="a")
                {
                    Console.WriteLine("Food in the bowl");
                    are.Set();
                }
            }


        }
        
       static void PigFeed(AutoResetEvent are)
        {
            Console.WriteLine($"Pig {Thread.CurrentThread.Name} is waiting food...");

            are.WaitOne();

            Console.WriteLine($"{Thread.CurrentThread.Name} is Enjoying its food..., it's happy");

            Random random = new Random();

            Thread.Sleep( 1000 * random.Next(2,7));
            Console.WriteLine($"{Thread.CurrentThread.Name} finished its food,It's waiting again...");
        }

    }
```