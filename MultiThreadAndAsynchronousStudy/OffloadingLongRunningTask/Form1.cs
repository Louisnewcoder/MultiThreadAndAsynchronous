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
            // TextLabel.Text = message; ֱ������д�ǲ����Ե�,��Ϊ����������ڷ��߳��ϵ��õ�,��UI����������߳��ϴ�����

            TextLabel.Invoke(() => TextLabel.Text = message); // ʹ�����̵߳���Ч����Invoke�����´�����Ȼ����߳� 
        }
    }
}
