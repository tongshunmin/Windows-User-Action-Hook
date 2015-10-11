﻿using System;
using System.Collections;
using System.Printing;
using EventHook.Hooks;
using EventHook.Hooks.Helpers;
using EventHook.Hooks.Library;

namespace EventHook
{

    public class PrintEvent
    {
        public DateTime EventDateTime { get; set; }
        public string PrinterName { get; set; }
        public string DocumentName { get; set; }
        public int? Pages { get; set; }
        public int? DocumentSize { get; set; }
    }

    public class PrintWatcher
    {
        /*Print history*/
        private static ArrayList _printers = null;
        private static PrintServer ps = null;
        public static bool PrintRun;
        public static void Start()
        {
            try
            {
                _printers = new ArrayList();
                ps = new PrintServer();
                foreach (var pq in ps.GetPrintQueues())
                {

                    var pqm = new PrintQueueHook(pq.Name);
                    pqm.OnJobStatusChange += pqm_OnJobStatusChange;
                    _printers.Add(pqm);
                }
                PrintRun = true;
            }
            catch { Stop(); }


        }
        public static void Stop()
        {
            if (_printers != null)
            {
                foreach (PrintQueueHook pqm in _printers)
                {
                    pqm.OnJobStatusChange -= pqm_OnJobStatusChange;
                    pqm.Stop();
                }
                _printers.Clear();
            }
            _printers = null;
            PrintRun = false;
        }
        static void pqm_OnJobStatusChange(object sender, PrintJobChangeEventArgs e)
        {

            if ((e.JobStatus & JOBSTATUS.JOB_STATUS_SPOOLING) == JOBSTATUS.JOB_STATUS_SPOOLING)
                {


                    var hWnd = WindowHelper.GetActiveWindowHandle();
                    var appTitle = WindowHelper.GetWindowText(hWnd);
                    var appName = WindowHelper.GetAppDescription(WindowHelper.GetAppPath(hWnd));

                    var record = new PrintEvent()
                    {
            
                        DocumentName = e.JobInfo.JobName,
                        DocumentSize = e.JobInfo.JobSize,
                        EventDateTime = DateTime.Now,
                        Pages = e.JobInfo.NumberOfPages,
                        PrinterName = ((PrintQueueHook)sender).SpoolerName

                    };
                  
                }

        }

    }
}