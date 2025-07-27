namespace MutexBasicPractice
{
    internal class Program
    {
        static void Main(string[] args)
        {

            string filePath = "CounterFile.txt";


            int num = 0;
            using (Mutex mutext = new Mutex(false, "GlobalCounterFileMutex"))
            {

                mutext.WaitOne();
                try
                {
                    for (int i = 0; i < 1000; i++)
                    {


                        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                        {
                            using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8, leaveOpen: true))
                            {
                                string l = sr.ReadToEnd();
                                num = string.IsNullOrEmpty(l) ? 0 : int.Parse(l);
                            }

                            num++;

                            fs.SetLength(0);
                            fs.Seek(0, SeekOrigin.Begin);

                            using (StreamWriter wr = new StreamWriter(fs, System.Text.Encoding.UTF8, leaveOpen: true))
                            {
                                wr.Write(num.ToString());

                            }
                        }
                    }
               
                }
                finally
                {
                    mutext.ReleaseMutex();
                }
            }
            Console.WriteLine("Process is finished");
            Console.ReadKey();
        }

    }
}

