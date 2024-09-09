﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VcCoop.src.utils
{
    internal class Automation
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)] private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", SetLastError = true)] private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)] private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll")][return: MarshalAs(UnmanagedType.Bool)] private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")] private static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)] private static extern int GetWindowTextLength(IntPtr hWnd);



        private const int LB_GETCOUNT = 0x018B;
        private const int LB_GETTEXTLEN = 0x018A;
        private const int LB_GETTEXT = 0x0189;

        private const uint PROCESS_VM_OPERATION = 0x0008;
        private const uint PROCESS_VM_READ = 0x0010;
        private const uint PROCESS_VM_WRITE = 0x0020;
        private const uint PROCESS_QUERY_INFORMATION = 0x0400;

        private const int WM_SETTEXT = 0x000C;
        private const int LB_DELETESTRING = 0x0182;

        private const int WM_GETTEXT = 0x000D;





        /// <summary>
        /// Main GUI window handle.
        /// </summary>
        private static IntPtr windowHandle;

        /// <summary>
        /// Process ID
        /// </summary>
        private readonly int processId;

        /// <summary>
        /// Process instance
        /// </summary>
        private readonly Process processInstance;

        /// <summary>
        /// Queue of messages
        /// </summary>
        private static readonly Queue<string> messages = new Queue<string>();

        public Automation(int pID)
        {
            Process proc = Process.GetProcesses()
                .Where(p => p.Id == pID)
                .FirstOrDefault();

            if (proc.Id > 0 && !proc.HasExited)
            {
                processInstance = proc;
                windowHandle = proc.MainWindowHandle;
                processId = proc.Id;

                new Thread(RunQueueWatcher).Start();
            }
        }

        /// <summary>
        /// Dequeues the queue and send output into process
        /// </summary>
        private void RunQueueWatcher()
        {
            while (true)
            {
                if (messages.Count > 0)
                {
                    SendInput(messages.Dequeue(), "Edit");
                }

                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// Inserts a message into queue.
        /// </summary>
        /// <param name="message">Message to queue</param>
        public void InsertToQueue(string message)
        {
            messages.Enqueue(message);
        }

        /// <summary>
        /// Sends input to a window's specified GUI element.
        /// </summary>
        /// <param name="content">Text to send</param>
        /// <param name="element">Element name (like textbox, listview, etc..)</param>
        private void SendInput(string content, string element)
        {
            IntPtr textBoxHandle = FindWindowEx(windowHandle, IntPtr.Zero, element, null);

            GetWindowThreadProcessId(textBoxHandle, out _);

            SetFocus(textBoxHandle);

            SendMessage(textBoxHandle, 0x000C, 0, new StringBuilder(content));
            SendMessage(textBoxHandle, 513, 0, null);
            SendMessage(textBoxHandle, 514, 0, null);
            PostMessage(textBoxHandle, 256, (IntPtr)13, IntPtr.Zero);
            PostMessage(textBoxHandle, 257, (IntPtr)13, IntPtr.Zero);
        }

        /// <summary>
        /// Extracts all text from elements.
        /// </summary>
        /// <returns></returns>
        private static string ExtractTextFromControls()
        {
            try
            {
                string data = "";

                IntPtr childHandle = IntPtr.Zero;

                while ((childHandle = FindWindowEx(windowHandle, childHandle, null, null)) != IntPtr.Zero)
                {
                    string text = GetTextFromWindow(childHandle);

                    if (!string.IsNullOrEmpty(text))
                    {
                        data += text.Trim() + "\n";
                    }
                }

                return data;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets complete text presented on dialog window
        /// </summary>
        /// <param name="windowHandle"></param>
        /// <returns></returns>
        private static string GetTextFromWindow(IntPtr childHandle)
        {
            try
            {
                int length = GetWindowTextLength(childHandle);

                if (length > 0)
                {
                    StringBuilder sb = new StringBuilder(length + 1);
                    SendMessage(childHandle, WM_GETTEXT, length + 1, sb);
                    return sb.ToString();
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all data items on dialog window on specified element.
        /// </summary>
        /// <returns></returns>
        public string[] GetElementData(string element)
        {
            IntPtr listBoxHandle = FindWindowEx(windowHandle, IntPtr.Zero, element, null);

            GetWindowThreadProcessId(listBoxHandle, out uint targetProcessId);

            IntPtr processHandle = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_QUERY_INFORMATION, false, targetProcessId);

            int count = SendMessage(listBoxHandle, LB_GETCOUNT, 0, null);

            List<string> items = new List<string>();

            for (int i = 0; i < count; i++)
            {
                int textLength = SendMessage(listBoxHandle, LB_GETTEXTLEN, i, null);

                if (textLength > 0)
                {
                    StringBuilder buffer = new StringBuilder(textLength + 1);

                    SendMessage(listBoxHandle, LB_GETTEXT, i, buffer);

                    items.Add(buffer.ToString());
                }
            }

            if (processHandle != IntPtr.Zero)
            {
                CloseHandle(processHandle);
            }

            return items.ToArray();
        }
    }
}