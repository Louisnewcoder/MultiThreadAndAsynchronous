namespace OffloadingLongRunningTask
{
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
}
