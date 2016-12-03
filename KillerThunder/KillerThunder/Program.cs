using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KillerThunder
{
    class Program
    {
        static List<string> msg = new List<string>();

        static void Main(string[] args)
        {
            int sec = 60 * 10;
            string mainExe = "Thunder".ToLower();
            string childExe = "ThunderPlatform".ToLower();
            string mainExePath = @"C:\Program Files (x86)\Thunder Network\Thunder\Program\Thunder.exe";
            Directory.CreateDirectory("log/");

            writeLog("開啟程式 !!");

            while (true)
            {
                writeLog($"尋找 {mainExe} 是否存在 ...");

                if (Process
                        .GetProcesses()
                        .Any(x => x.ProcessName.ToLower() == mainExe) == true)
                {
                    writeLog($"已找到 {mainExe} ...");
                    writeLog($"尋找 {childExe} 是否存在並且關閉 ...");

                    Process
                    .GetProcesses()
                    .Where(x => x.ProcessName.ToLower() == childExe)
                    .ToList()
                    .ForEach(x => x.Kill());

                    writeLog($"執行完畢 ...");
                }
                else
                {
                    writeLog($"找不到 {mainExe} ...");

                    try
                    {
                        writeLog($"嘗試開啟 {mainExePath} ...");
                        Process.Start(mainExePath);
                        writeLog($"開啟成功 !!");
                    }
                    catch
                    {
                        writeLog($"開啟失敗 !!");
                    }
                }

                writeLog($"待 {sec} 秒後再次執行 !!");
                Thread.Sleep(sec * 1000);
            }
        }

        static void writeLog(string str)
        {
            DateTime date = DateTime.UtcNow.AddHours(8);
            string path = $"log/{date.ToString("yyyyMMdd")}.txt";

            str = date.ToString("yyyy/MM/dd HH:mm:ss") + "：" + str;
            msg.Add(str);
            Console.WriteLine(str);

            if (msg.Count > 10)
            {
                if (File.Exists(path) == false)
                {
                    using (FileStream fs = File.Create(path))
                    {
                        msg.Insert(0, date.ToString("yyyy/MM/dd HH:mm:ss") + "：建立此TXT");
                    }
                }


                if (IsFileLocked(path) == false)
                {
                    StreamWriter writer = new StreamWriter(path, true);

                    foreach (var item in msg)
                    {
                        writer.WriteLine(item);
                    }

                    msg.Clear();
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
            }

        }

        static bool IsFileLocked(string file)
        {
            try
            {
                using (File.Open(file, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException exception)
            {
                var errorCode = Marshal.GetHRForException(exception) & 65535;
                return errorCode == 32 || errorCode == 33;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
