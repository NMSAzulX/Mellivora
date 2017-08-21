using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace System
{
    public class PerfomanceHelper
    {
        private int maxLength = 0;

        private int _count;

        private string _format;
        public IDictionary<string, Action> ActionDict
        {
            get; set;
        }

        public IDictionary<string, TimeSpan> ActionTime { get; set; }
        public IDictionary<string,long> ActionLong { get; set; }
        public IDictionary<string, string> ActionFomart { get; set; }
        public PerfomanceHelper(int count,string format=null)
        {
            ActionDict = new Dictionary<string, Action>();
            ActionLong = new Dictionary<string, long>();
            ActionTime = new Dictionary<string, TimeSpan>();
            ActionFomart = new Dictionary<string, string>();
            _count = count;
            if (format==null)
            {
                format = _count + "次{0}";
            }
            _format = format;
        }


        public void Add(string temp, Action func)
        {
            temp = string.Format(_format, temp);
            if (maxLength< DealFormat(temp))
            {
                maxLength = DealFormat(temp);
            }
            Stopwatch time = new Stopwatch();
            time.Start();
            func();
            time.Stop();
            ActionDict[temp] = func;
            if (ActionTime.ContainsKey(temp))
            {
                if (ActionTime[temp]< time.Elapsed)
                {
                    return;
                }
            }
            ActionTime[temp] = time.Elapsed;
            ActionLong[temp] = time.ElapsedMilliseconds;
        }

        private int DealFormat(string temp)
        {
            int i = 0;
            foreach (char item in temp)
            {
                if (item >= 0 && item <=127)
                {
                    i+=1;
                }
            }
            return temp.Length * 2 - i;
        }
        public void ShowReuslt(bool isShowMilliseconds=false)
        {
            foreach (KeyValuePair<string, long> item in ActionLong)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
               
                Console.Write(String.Format("{0,"+ (maxLength-DealFormat(item.Key)) + "}", ""));
                Console.Write(item.Key);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("→执行耗时 :");
                if (item.Value>1000)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (item.Value>600)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (item.Value>200)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                if (isShowMilliseconds)
                {
                    Console.WriteLine(ActionTime[item.Key]);
                }
                else
                {
                    Console.WriteLine(ActionTime[item.Key]);
                }
                
            }
        }

    }
}
