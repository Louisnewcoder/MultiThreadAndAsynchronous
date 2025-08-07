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

### Thread states
线程是有状态的。
可以通过调用实例属性查看 `.ThreadState`。

一般都是从 `Unstarted`开始,直到 `Stopped`结束。
具体定义可以参看 https://learn.microsoft.com/en-us/dotnet/api/system.threading.threadstate?view=net-8.0
每个`.Net`版本可能会有不同，注意选择对应版本。

***根据官方建议，ThreadState只适合用来Debug，不要用它来做涉及线程同步的操作***

### Thread Safty
是指一个`Function`、`Class`或者`Data structure`可以在被多个线程同时使用或访问或修改的前提下，不会造成任何 **Race Condition、不符合期待的行为、或者数据污染**。一般来讲，具有这样性质的代码段的内部都已经使用了类似线程锁的机制。

### ThreadPool
线程池。 和之前学的对象池一个概念，就是将线程缓存起来的集合。
但是C#中的线程池是由 *.Net*的*CLR*为应用程序创建的。开发者可以直接用：
最小线程数：ThreadPool.GetMinThreads() (默认通常为处理器核心数)
最大线程数：ThreadPool.GetMaxThreads() (默认约 32767)

但是线程池是由.Net管理的，不能销毁也不能改名。如果需要对线程生命周期控制比较高的任务还是自己new Thread

`Task`的底层就是线程池。 线程池适合处理IO密集型任务、短平快的任务或CPU密集型的任务。
**长时间阻塞操作不要用线程池！**
如何使用线程池
```C#
        ThreadPool.QueueUserWorkItem(AA);
        void AA(object? i) 
        {
            // code 
        };
```


### Thread Affinity - 线程关联性
是指在多线程编程中，某些操作或者资源必须由创建它们的原始线程访问或者修改的特性。

典型的例子，以Windows app开发举例，就是UI更新只能在主线程中更新，当代码直接希望通过分线程更新UI时就会报错。
避免这样异常发生就要确保`操作或资源`**是在创建它们的原始线程中访问或者修改的。**，或者利用一些机制比如 .Net提供的一些组件具有线程调度的`.Invoke( Action callback)` 方法。

```C#
        public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(()=>ShowText("Ho~~ Miao Mi",5000));
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() => ShowText("Wang wang wangwagn", 2000));
            thread.Start();
        }

        void ShowText(string message ,int delay)
        {
            Thread.Sleep(delay);
            // TextLabel.Text = message; 直接这样写是不可以的,因为这个方法是在分线程上调用的,而UI组件是在主线程上创建的

            TextLabel.Invoke(() => TextLabel.Text = message); // 使用有线程调度效果的Invoke将更新代码调度回主线程 
        }
    }
```

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

### ManualResetEvent - 1对多信号控制器
`ManualResetEvent` 是一个 *1对多* 类型的信号控制器。当它 *信号亮起* 的时候，所有等待线程都可以进入被阻塞的代码区域。并且开发者必须手动调用示例方法 `.Reset()`才能关闭信号，从而实现阻塞效果。
另外，它是跨进程的版本。 它比较重，性能开销大，时间精准度一般。如果不是跨进程需求，推荐使用`ManualResetEventSlim`。

***无论使用哪个版本都要记得释放资源***

`ManualResetEvent`语法代码示例：

```C#
    internal class Program
    {
        static int CarCount = 0;
        static void Main(string[] args)
        {
            ManualResetEvent mre = new ManualResetEvent(false); // 代表 traffic light控制器,初始化是红灯
       
            Console.WriteLine("Traffic is operating......");
            Console.WriteLine("Now it is Red Light");
            Console.WriteLine("All need to wait");

      
            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(() => DoWork(mre));
                thread.Name = $"Car {i}";
                thread.Start();
            }

            bool isGreenLight=false;
            while (true)
            {
                if (!isGreenLight)
                {
                    Console.ReadKey();  // 通过用户输入来开启绿灯
                    mre.Set();
                    isGreenLight = true;
                }
                    
                if(CarCount>=5) // 如果大于5就跳出循环, 让后面的代码模拟变成红灯有新的车在等
                    break;
            }

            for (int i = 0; i < 10; i++) // 模拟新来了10辆车
            {
                Thread thread = new Thread(() => DoWork(mre));
                thread.Name = $"Car {10+i}";
                thread.Start();
            }
        }

        static void DoWork(ManualResetEvent mre)
        {
            Console.WriteLine($"Working thread {Thread.CurrentThread.Name} is waiting ...");
            mre.WaitOne();
            Random r = new Random();
            Thread.Sleep(1000 * r.Next(2, 7));
            Console.WriteLine($"Working thread {Thread.CurrentThread.Name} is Passing by ...");
        
            
            CarCount++;  // 记数

            if(CarCount>=5) // 模拟车过去时间到了 要变红灯
            {
                mre.Reset();    
            }
        }
    }
```

#### ManualResetEventSlim - 轻量级进程内跨线程版本
与`ManualResetEvent`语法基本一致。只是等待信号的方法是 `.Wait()`。
```C#
    internal class Program
    {
        static int CarCount = 0;
        static void Main(string[] args)
        {
            ManualResetEventSlim mre = new ManualResetEventSlim(false); // 代表 traffic light控制器,初始化是红灯

           
            Console.WriteLine("Traffic is operating......");
            Console.WriteLine("Now it is Red Light");
            Console.WriteLine("All need to wait");

          
            for (int i = 0; i < 10; i++)
            {
                Thread thread = new Thread(() => DoWork(mre));
                thread.Name = $"Car {i}";
                thread.Start();
            }

            bool isGreenLight=false;
            while (true)
            {
                if (!isGreenLight)
                {
                    Console.ReadKey();  // 通过用户输入来开启绿灯
                    mre.Set();
                    isGreenLight = true;
                }
                    
                if(CarCount>=5) // 如果大于5就跳出循环, 让后面的代码模拟变成红灯有新的车在等
                    break;
            }



            for (int i = 0; i < 10; i++) // 模拟新来了10辆车
            {
                Thread thread = new Thread(() => DoWork(mre));
                thread.Name = $"Car {10+i}";
                thread.Start();
            }
        }

        static void DoWork(ManualResetEventSlim mre)
        {
            Console.WriteLine($"Working thread {Thread.CurrentThread.Name} is waiting ...");
            mre.Wait(); // 如果是这个方法，如果用完后不是放资源相对安全，但是还是建议释放
            Random r = new Random();
            Thread.Sleep(1000 * r.Next(2, 7));
            Console.WriteLine($"Working thread {Thread.CurrentThread.Name} is Passing by ...");
        
            
            CarCount++;  // 记数

            if(CarCount>=5) // 模拟车过去时间到了 要变红灯
            {
                mre.Reset();    
            }
        }
    }
```

### ThreadPool
```C#
        ThreadPool.QueueUserWorkItem(AA);
        void AA(object? i) 
        {
            // code 
        };
```

## MultiThreads Programming debugging
VS开发环境下，使用debug(F5) 模式运行程序后可以依赖`Threads`窗口和`Parallel Stacks`窗口进行Debug分析。
这两个窗口可以在`顶部菜单->Debug->Windows`菜单中找到。

1. 两个窗口的核心信息基本上是一致的。`Parallel Stacks`窗口 提供了更多图形化的提示。
2. 可以在`Threads`窗口 `暂停` 具体某个线程，然后让其他线程继续执行。只需要根据需要点击 `Pause`暂停或 `Thaw`继续运转
3. 在Debug暂停时，可以使用`immediate window` 输入命令查看相关信息。

## 控制线程等待
有3中方法：
```C#
    Thread.Sleep( milliseconds ); 
    /// Thread.Sleep的效果是将 `线程挂起`，也改变了`线程的状态`。它把线程从CPU移除一段时间(传入的参数)。 这个方法最节省资源，但是精度略低，在10-15毫秒左右。

    Thread.SpinWait( number of iterations ); 
    /// Thread.SpinWiat的效果是线程继续霸占CPU核心。让CPU空转n次(传入的参数)，线程状态还是在运行。适合极端时间等待，用于竞争锁的情况下使用。精度极高，纳秒级。 
    /// SpinWait(1000) 的耗时远远低于 Thread.Sleep(1)，甚至是微秒级。

    SpinWait.SpinUntil( condition CallBack，timeout );
    /// 更高级一点的线程等待处理方式。与SpinWait一样，先让CPU空转，如果条件表达式成立了，返回 true ，结束等待进入后面的代码。有3个重载版本，不带timeout的就只会依赖条件表达式的结果。带timeout的，如果条件表达式不成立，时间到了也会结束等待，但是返回false 然后执行后面的代码。

```

## 从线程中返回一个结果
***首先，Thread本身并没有内置的返回一个结果的方法！！***
如果必须要从工作线程中返回一个结果只能通过操作`共享资源`的方式，在分线程中更新这个资源的值之后，在主线程中使用工作线程的`.join()`方法将线程阻塞，等待线程完成后再让后续代码使用更新后的结果。

但是`Task` 类有内置的返回一个结果的方法。

## 如何处理工作线程中的异常Exception
***在工作线程中出现的异常不会报告在主线线程中***。如果工作线程中出现了异常，从线程角度来讲不会影响主线程，会直接结束工作线程。虽然不会影响主线程，但是会影响工作线程中的任务和它的结果。

```c#
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread thread = new Thread(DoWork); // 工作线程的任务
            thread.Start();
            thread.Join();

            Console.WriteLine("Program Finished"); 
        }

        static void DoWork()
        {
            // 工作线程中的异常不会报告到主线程中
            // 处理 exception 的代码要在worker thread中完成
            try
            {
                throw new InvalidOperationException("uh Oh! I am from worker thread");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
```
## Asynchronous Programming 异步编程
### multi threads programming VS asynchronous programming
**多线程编程**强调同时调用多个线程处理多个任务。可以是将一个任务切片，交由多个线程同时处理从而提高效率；也可能是同时处理不同的任务提高全局效率和cpu利用率。 这取决于程序是如何设计的、

它是基于多个线程的，偏重于处理CPU密集型任务。对于参与任务的线程而言，它是阻塞性的

**异步编程**强调不阻塞性。一个任务没处理完时，程序先去干别的，等这个任务处理完了，程序在回来处理结果。它可以是单线程，也可以是多线程。对于参与任务的线程而言， 它强调不阻塞。

它适合处理IO型任务。

### Task

`Task`的特点：
1. 默认使用线程池；
2. 可以返回值；
3. 可以控制后续流程 - `ContinueWith`
4. 可以将`Asynchronous/Await`编程写的像Synchronous编程

#### TaskBasic Syntax
`Task`的使用语法与`Thread`很像：

```C#
    internal class Program
    {
        static void Main(string[] args)
        {
            Task task = new Task(DoWorkByTask); // 语法1 只接受同步委托
            task.Start();

            Console.ReadKey();

            Console.WriteLine();
            
            Task.Run(()=> Console.WriteLine("I am written By Task Syntax2")); // 语法2 接受同步委托也接受异步委托
                                                                              // .Run方法返回一个 Task实例, 如果由后续操作可以操作这个实例

            Console.ReadKey();

        }

        static void DoWorkByTask()
        {
            Console.WriteLine("I am used by TASK");
            Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread); // 证明是线程池的线程
        }
    }
```

#### 利用Task获取返回值
要多利用`Task`的各种构造函数，支持泛型返回值。
```C#
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] numbers = { 1, 2, 3 ,4,5,6,7,8,9,10};

            int totalParts = 4;

            int interval = numbers.Length/totalParts;

            var t1 = Task.Run(()=>DoSum(numbers,0,interval));
            var t2 = Task.Run(()=>DoSum(numbers, interval, interval*2));
            var t3 = Task.Run(()=>DoSum(numbers, interval * 2, interval*3));
            var t4 = Task.Run(()=>DoSum(numbers, interval * 3, numbers.Length));


             t1.Wait();
            t2.Wait();
            t3.Wait();
            t4.Wait();

            List<Task<int>> tasks = new List<Task<int>>(); // 展示Task的功能
            tasks.Add(t1);
            tasks.Add(t2);
            tasks.Add(t3);
            tasks.Add(t4);

            Console.WriteLine(t1.Result+t2.Result+t3.Result+t4.Result); // 结果与下面一行代码一致
            Console.WriteLine(tasks.Sum(t=>t.Result));// 为了展示Task的更多功能, 在一个集合里也可以使用聚合方法 

            Console.ReadKey();
        }

        static int DoSum(int[] nums, int Start, int End) {
            int tempSum = 0;

            for (int i = Start; i < End; i++) { 

                tempSum += nums[i];
            }
        
            return tempSum;
        }
    }
```

### Task API

#### Task.Delay() 
静态方法，类似 `Thread.Sleep`, 让当前Task延迟若干毫秒。

#### Task.Wait()
实例方法，等待当前实例的任务完成。阻塞当前线程，直到任务完成。类似`Thread.Join()`。

#### Task.WaitAll()
静态方法，可以将若干个task实例作为参数传入，然后等待这些参数tasks全部完成后，在执行后续代码。也是阻塞线程。

#### .result 属性
访问当前task实例的`结果`。
***需要注意的是： 访问 `result` 属性实际上也是阻塞当前线程，直到`result`被Task处理完并返回***

#### Task.ContinueWith()
实例方法。传入`自己(当前实例委托)`去执行另一个`委托任务`并返回一个新的`Task`。主要目的是为了**基于当前委托的结果**去执行另一个任务，并且避免等待当前`.Result`属性导致的*阻塞*。

```C#
        static void Main(string[] args)
        {
            // 使用Task.Run 来传入一个异步任务
            Task<int> task1 = Task.Run(async () =>
            {
                int sum = 0;
                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(1000); // 模拟耗时任务
                    sum += i;
                }
                return sum;
            });

            task1.ContinueWith(task1 =>
            {
                int result = task1.Result;
                Console.WriteLine($"Task1 result is {result}");
            });

            while (true)
            {
                string input = Console.ReadLine();
                if (input == "exit")
                { break; }
                else
                {
                    Console.WriteLine(input);
                }
            }

            Console.WriteLine("Program is finished");
            Console.ReadKey();
        }
```
#### .WhenAll()或.WhenAny()来处理任务集合的后续
它们都接收一个`Task`集合。
`.WhenAll` 代表 - 当集合中的所有任务**都**完成之后..
`.WhenAny` 代表 - 当集合中的**任意一个**任务完成之后..

```C#
        static void Main(string[] args)
        {
            List<Task<int>> taskForWhenAll = new List<Task<int>>(); // 准备个Task列表最后用来测试


            for (int i = 0; i < 10; i++)
            {
                int a = i;                  // 防止闭包
                var task = Task.Run(async () =>
                {                   
                    await Task.Delay(500);
                    return a;
                });
                taskForWhenAll.Add(task);   // 把任务加到列表中
            }

            Task.WhenAll(taskForWhenAll)        // 使用WhenAll - 所有任务都完成时在继续做...
                .ContinueWith(
                t=>         // 这个t 是WhenAll方法在所有任务都完成时创建的一个返回值为int[] 的Task<int[]> 
                Console.WriteLine($"The result for when all is {t.Result.Sum()}") // 将返回值的int[]进行聚合运算
                );

            List<Task<int>> taskForWhenAny = new List<Task<int>>();
            for (int i = 0; i < 10; i++)
            {
                var task = Task.Run(async () =>
                {
                   int a = i;
                    await Task.Delay(500);
                    
                    return a;
                });
                taskForWhenAll.Add(task);

            }

            Task.WhenAny(taskForWhenAll)    // 使用WhenAny - 任务集合中任何一个任务完成就做...
                .ContinueWith(
               t =>         // 这个t 是一个返回值为 Task<int> 的Task,这个返回值代表着最先完成的那个任务
               Console.WriteLine($"One of the Result is {t.Result.Result}") // 使用返回值任务的返回值！！
               );


            Console.WriteLine("This is the End of The program!"); // 异步证明, 这最后一行代码先打出来了

            Console.ReadKey();
        }
```

#### .Unwrap()方法来剥离ConitnueWith的Task<Task<T>>型返回值的外壳
使用`.Unwrap()`方法可以在使用`ContinueWith`或`Task.Factory.StartNew`等返回`Task`类型值的方法时让代码更优雅。它可以剥去返回值的"外壳"Task，直接让开发者调用作为泛型参数的“结果” Task。
```C#
            Task<int> taskOrigin = Task.Run(() => 10);

           var taskWithoutUnWrap = taskOrigin
                .ContinueWith(task =>               // ContinueWith 返回的是个Task<T>
            {
                return Task.Run(() => task.Result * 5); // .Run返回的是个Task
            }  ); // 不用Unwrap, 所以ContinueWith返回带是Task<Task<int>>

            Console.WriteLine(taskWithoutUnWrap.Result.Result); // 所以这里用的是Task<>的结果泛型参数的Task<int>的结果

            var taskByUnWrap = taskOrigin.ContinueWith(task =>
            {
                return Task.Run(() => task.Result * 7);
            }).Unwrap(); // 使用Unwrap剥离返回值的外层Task仅保留内层泛型 Task<int>

            Console.WriteLine(taskByUnWrap.Result); // 使用Unwrap代码更优雅
```
#### .IsFaulted属性
如果是`true`则该实例task未处理异常任务失败了。是Task的`status`之一。

### 关于Task的异常
1. Task的异常不会在主线程中暴露出来，也不会阻塞主线程导致崩溃。它会隐藏于产生异常的Task中。
2. 使用`try...catch`语句将Task相关代码包裹起来不会捕获异常，catch中的代码不会执行。
3. 异常会存储在发生异常的Task中。
4. 因为异常存储与Task中，所以如果由多个是可以遍历的。
5. 使用`.Wait()`或者`.Result`**可以抛出异常**。
#### 如何遍历Task实例中存储的异常
```C#
            ///利用status - IsFaulted检查是否失败了,同时查看是否存储了Exception
        if(taskOrigin.IsFaulted && taskOrigin.Exception != null)
        {
                // 如果确认异常存在则遍历异常
            foreach (Exception ex in taskOrigin.Exception.InnerExceptions)
            {
                Console.WriteLine(ex.Message);
            }
        }
```

#### 使用ContinueWith的时候利用带有 TaskContinuationOptions参数的签名版本
`.ContinueWith(Action callbackTask, TaskContinuationOptions)` 参数可以选择`TaskContinuationOptions.NotOnFaulted`枚举项，这样可以在调用这个方法的*基任务*上出现异常时不执行作为参数传入的回调任务。

#### 被await修饰的任务出现异常时也会直接抛出异常
但是如果有多个异常，只会抛出第一个。

### 关于Task Synchronization
与Thread Synchronization相关技术一致，只需要把Thread实例换成Task实例。

### 关于 Task Cancellation
推荐使用`CancellationTokenSource`。
**另外如果是自己主动取消的建议使用抛出异常**
可以直接用token的实例
`token.ThrowIfCancellationRequested()`

```C#
using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        using (CancellationTokenSource cts = new CancellationTokenSource())
        {
            Console.WriteLine("开始上传文件... 输入 'cancel' 终止上传。");

            Task uploadTask = Task.Run(() => UploadFile(cts.Token), cts.Token);

            // 主线程监听用户输入
            while (!uploadTask.IsCompleted)
            {
                string? input = Console.ReadLine();
                if (input == "cancel")
                {
                    cts.Cancel(); // 通知任务取消
                    Console.WriteLine("取消请求已发送！");
                    break;
                }
            }

            try
            {
                uploadTask.Wait(); // 等待任务结束
            }
            catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
            {
                Console.WriteLine("上传任务被取消了！");
            }

            Console.WriteLine("主程序结束，资源已清理。");
        } // 👈 using 块结束，cts 被 Dispose
    }

    static void UploadFile(CancellationToken token)
    {
        try
        {
            for (int i = 1; i <= 10; i++)
            {
                token.ThrowIfCancellationRequested();

                Console.WriteLine($"上传中...{i * 10}%");
                Thread.Sleep(500); // 模拟上传延迟
            }

            Console.WriteLine("文件上传完成！");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("上传过程被用户取消！");
        }
    }
}
```

**另外，如果是希望通过超时来控制取消，还可以使用`CancellationTokenSource`实例方法的`.CancelAfter( int milliseconds)`方法**

### await 关键字
1. 在一个方法或一段代码中，当程序运行到 `await`关键字的时候，`calling thread` 会被立即释放。然后所有在`await`关键字后面的内容会被看成是一个 `continuation`,可能在分线程运行，也可能在后台运行。
2. `await`关键字会返回任务的值，不在需要`task.Result`。
3. `await`关键字可以抛出异常，返回任务异常集合中的第一个。
4. `await`关键字会自动管理上下文同步。

### async 和 await 关键字的基础语法
`async`和`await`是一组固定搭配语法糖。
**`async`关键字告诉编译器生成状态机、`await`实现把任务挂起，然后等待完成后恢复流程。**
语法规则：
1. `await`只能存在于用`async`修饰的方法或lambda表达式中。`await`修饰的一定是**可等待的类型**，如`Task`。**PS:`GetAwaiter()`方法是`Task`类的扩展方法提供的，返回一个`TaskAWater`类型**。
2. `async`修饰的方法返回值需要是`Task`、`Task<T>`或者`ValueTask`的一种。虽然可以写void返回值类型，但是不推荐，除非是事件处理器，比如：
```C#
private async void button1_Click(object sender, EventArgs e)
{
    await DoSomethingAsync();
}
```
**PS:`ValueTask` 是轻量级的Task，可以避免堆分配，适合高性能场景**

#### 在不允许定义async的环境下如何使用async/await组合？ 如何void main 和 Unity声明函数中
用一个以 `async` 修饰的 返回值为 `void`的方法包装一下。
```C#
    static void Main(string[] args)
    {
        Console.WriteLine("Start program.....");
        // await DoSomeThingAsync(); // 无法编译

        DoAsWraper(); // 使用一个包装器

        Console.WriteLine("Program finished");
        Console.ReadKey();
    }

    static async void DoAsWraper() // 包装器 实际执行 DoSomeThingAsync
    {
        Console.WriteLine("I am wrapper");
        await DoSomeThingAsync();
        Console.WriteLine("Wrapper finished");
    }

    static async Task DoSomeThingAsync() //异步方法
    {
        Console.WriteLine("Enters in DoSomeThingAsync");
        await Task.Delay(1000);
        Console.WriteLine("After DoSomeThingAsync");
    }
```

#### 基于async和await的 continuation
```C#
public async Task<int> SomeWorkAsync(CancellationToken cancellationToken)
{
    var data = await FetchDataAsync(cancellationToken);
    var processed = await ProcessDataAsync(data, cancellationToken);
    return await SaveDataAsync(processed, cancellationToken);
}
```


## Parallel Loops
用于处理 `divide and conquer` 类型的编程问题。

### Parallel Loops的特点
**Data Parition**
默认情况下，编译器会自己决定用多少个线程，然后将目标数据集合分成对应数量的份数。

**主要用线程池的线程，也会创建新的线程**

**编译器会自己认为最合适的决定**
使用多少线程、用线程池还是创建新的、将数据分成多少份。

**Parallel loops系列方法是阻塞方法**

### 处理Parallel Loops的异常
1. try-catch要包裹在 `Parallel loops`的外面。
2. 任何一个线程抛出一场之后，所有线程都不会进入下一个循环。
3. 新的迭代不会启动（但已运行的迭代会继续执行完）。已启动的迭代可能继续抛出异常。
4. 所有的线程上的异常都会进入到一个`AggregateException`中。
5. 在 `Parallel loops`的重载版本中，有可以使用`state`参数的版本。
```C#

 // 处理Parallel Loops的异常
 try
 {
     Parallel.For(0, numbers.Count, (i, state) =>   // state 代表任意一个线程的状态
     {
         if (state.IsExceptional)        // 检查是否有任意一个线程抛出了异常 
             return;

         lock (_lock)
         {
             //  sum += numbers[i];
             double x = Math.Sqrt(i) * Math.Log(i + 1);
         }
     });
 }
 catch (AggregateException ex) { 
 
     foreach (Exception inner_ex in ex.InnerExceptions) { Console.WriteLine(inner_ex.Message); }
 }
```

### 使用线程状态的Break()和Stop()结束Parallel Loops的迭代
`分线程state.Break()`和`分线程state.Stop()`两个方法可以用来结束Parallel循环中的迭代。
`Break()`更温和在尽快结束循环的前提下让已经开始的迭代完成。**适合 需要已完成部分结果 的场景**
`Stop()`更强制一些，已开始的循环可能完不成。**适合 不需要考虑结果 的场景**
```C#
            int[] nums = Enumerable.Range(0, 100).ToArray();

            Parallel.For(0, nums.Length, (i, state) =>   // state 代表任意一个线程的状态
            {


                if (i == 50)        
                {
                    state.Stop(); // 尽快结束循环,正在处理中的迭代可能也会结束不会获得结果
                    Console.WriteLine("I stopped at " + nums[i]); // 可能47,48 也不会处理完,不会迭代51至100

                    state.Break(); // 尽快结束循环, 正在处理中的迭代会完成,可以得到结果
                    Console.WriteLine("I Broke at " + i);// 如果其他线程正在处理47 48，会处理完,不会迭代51至100

                }
                Console.WriteLine("I print {0}", nums[i]);
            });
```
### ParallelLoopResult
`ParallelLoopResult`是Parallel系列循环的返回值。它可以提示开发者循环是否完成以及`LowestBreakIteration`是多少。但是只有在调用`Break()`且没有抛出异常的情况下才会记录`LowestBreakIteration`信息。调用`Stop()`或者抛出异常的话都是`null`。


### 在ParallelLoop中使用局部变量处理DivideAndConquer问题
利用带有 `local Variable`的方法签名版本的循环:

```C#
// Parallel.For(int fromInclusive, int toExclusive, Func<T localVariable> localVariable, Func<in T,in ParallelLoopState,in TlocalVariable,out TlocalVariable> taskBody, Action<T localVariable> finallyCallBack)

            Parallel.For(0, numbers.Count, () => 0, // 局部变量初始化
                (i, state, localSum) => localSum + numbers[i],
                localSum =>
                    {
                        lock (_lock)
                        {
                            sum += localSum; // 最后合并
                        }
                    });
```

## PLINQ
### .AsParallel() 方法执行多线程LINQ查询

当要使用`LINQ`查询大规模集合时，使用传统`顺序型集合LINQ查询方式`会产生较长耗时。可以在集合实例上对集合调用`.AsParallel()`方法将集合转换为`可多线程型查询型集合`，利用多线程的效果处理结果。

*ps: 经过自己测试和其他多线程编程效果一样，如果是简单运算，可能还是不如单线程上的顺序循环快！*
因为PLINQ是将数据自动差分成组之后交给不同线程同时处理，并且每个线程的顺序和效率可能不同，所以返回的结果集合也几乎是乱序的。如果想将结果顺序使用，可以在`AsParallel()`后链接`.AsOrdered()`可以将结果排序。

```C#
        static void Main(string[] args)
        {
            int count = 100;
            var items = Enumerable.Range(0, count).ToArray();

            // 传统阻塞型查询与遍历
            // var resultByTraditional  = items.Where( i => i %2==0 );
            //  foreach (var item in resultByTraditional) { Console.WriteLine(item); }
  

            // 使用PLINQ遍历
            //  var resultByPLinq = items.AsParallel().Where(i => i % 2 == 0);

            // 在AsParallel()后链接.AsOrdered()可以将结果排序
            var resultByPLinq = items.AsParallel().AsOrdered().Where(i => i % 2 == 0); 
              foreach (var item in resultByPLinq){Console.WriteLine(item);}
        }
```
### 在.AsParallel()查询结果上调用.ForAll()可以执行多线程遍历
```C#
            // 基于上面的示例代码
            resultByPLinq.ForAll(i=>
            {   
                Console.WriteLine(i + $" Current Id is {Thread.CurrentThread.ManagedThreadId}"); 
            });  
```

### 调整PLINQ查询结果与遍历的Merge/Buffer设置
使用`.WithMergeOptions()`可以传入参数来控制生产多少内容之后交给**在主线程上**的*消费者，比如迭代器*去使用。
***如果要手动设置这个参数，就需要使用普通的`foreach`遍历，因为是Merge是为了服务主线程上的“消费者”。而非在多线程结果上调用.ForAll()方法，因为多线程遍历producer和consumer会在同一个线程上***
本质上就是所谓的`producer-consumer`模式中的`什么时候消费与消费多少的问题`
有3个常规设置:
1. Deffault/AutoBuffered:
行为：每个线程计算一批结果（默认块大小）后，批量传递给消费者。
优点：平衡了实时性和吞吐量。
缺点：轻微的内存和延迟开销
**适合场景： 常规查询**
```C#
var result = data.AsParallel()
    .WithMergeOptions(ParallelMergeOptions.AutoBuffered) // 可省略（默认）
    .Where(x => x % 2 == 0);
```
2. NotBuffered:
行为：每个线程计算完一个结果后，立即传递给消费者（如 foreach）。
优点：延迟极低，内存占用最小。
缺点：频繁的线程同步可能降低吞吐量。
**适合场景： 流式处理,如实时日志**
```C#
var result = data.AsParallel()
    .WithMergeOptions(ParallelMergeOptions.NotBuffered)
    .Where(x => x % 2 == 0);

foreach (var item in result) // 结果逐条输出
{
    Console.WriteLine(item);
}
```

3. FullyBuffered - 全部生产完了之后，再全部消费掉
行为：所有线程完成计算后，一次性输出全部结果。
优点：吞吐量最高（适合聚合操作如 Count()、Sum()）。
缺点：内存占用高，延迟最大。
**适合场景： 聚合操作如 ToList()、需要严格顺序AsOrdered()**

```C#
var result = data.AsParallel()
    .WithMergeOptions(ParallelMergeOptions.FullyBuffered)
    .Where(x => x % 2 == 0);

// 所有结果计算完成后才会进入 foreach
foreach (var item in result)
{
    Console.WriteLine(item);
}
```

### PLINQ如何处理异常
在`Query`部分不会抛出异常，和`LINQ`一样，只是`Query`部分只是创建了存储方法。只有在使用`Query结果`的时候才会执行`Query`代码。所以try-catch语句应该去包裹**使用Query结果**的代码部分。

### 如何取消PLINQ查询结果的使用
Step 1 创建`CancellationToken`,然后在PLINQ创建查询的代码部分链接`.WithCancellation()`并将Token作为参数传入。
Step 2 在使用查询结果的代码部分，调用CancellationToken实例的`.Cancel()`方法。

***要注意： ForAll的特点是“尽力的完全执行”, 而且在使用Parallel系列的方法中因为是多线程，很多线程已经领到了任务。所以即使使用了.Cancel() 可能也还是会执行出很多内容，如果迭代的目标少，可能会都执行出来***

```C#
        CancellationTokenSource cts = new CancellationTokenSource();

        var resultByPLinq = items.AsParallel().AsOrdered().WithCancellation(cts.Token).Where(i => i % 2 == 0);
        foreach (var item in resultByPLinq) { Console.WriteLine(item); }

        try
        {
                resultByPLinq.ForAll(i =>
                {
                    Console.WriteLine(i + $" Current Id is {Thread.CurrentThread.ManagedThreadId}");

                    if (i == 10)
                        cts.Cancel();

                });
        }
        catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
```