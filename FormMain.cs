using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ProgramStatus
{

    public partial class FormMain : Form
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // generateProgramList();//turn on for generating the file
            fileReadAndCreateButton();
            dynamicFormHeightWidth();
        }

        private void DynamicButton_Click(object sender, EventArgs e)
        { //killing app

            Button DynBtn = (Button)sender;
            //Console.WriteLine(DynBtn.Name);
            //MessageBox.Show(DynBtn.Name+ " is closed");

            if (DynBtn.BackColor == Color.Green)
            {
                Process[] procs = System.Diagnostics.Process.GetProcessesByName(DynBtn.Name);
                if (procs.Length > 0)
                {
                    procs[0].Kill();
                    DynBtn.BackColor = Color.Red;
                }
            }
            else if (DynBtn.BackColor == Color.Red)
            {
                string address = toolTip1.GetToolTip(DynBtn);
                if (File.Exists(address) == true)
                {
                    Process myProcess = new Process();
                    try
                    {
                        string extension;
                        extension = Path.GetExtension(address);

                        myProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(address);
                        myProcess.StartInfo.FileName = Path.GetFileName(address);
                        if (extension.Equals(".bat"))
                        {
                            //   myProcess.StartInfo.UseShellExecute = true;
                            myProcess.StartInfo.CreateNoWindow = false;
                        }
                        else
                        {
                            myProcess.StartInfo.UseShellExecute = false;
                            myProcess.StartInfo.CreateNoWindow = true;
                        }
                        myProcess.Start();

                        if (extension.Equals(".bat"))
                            myProcess.WaitForExit();
                        myProcess.EnableRaisingEvents = true;
                        myProcess.Exited += new EventHandler(myProcess_Exited);
                        DynBtn.BackColor = Color.Green;
                        // This code assumes the process you are starting will terminate itself.
                        // Given that is is started without a window so you cannot terminate it
                        // on the desktop, it must terminate itself or you can do it programmatically
                        // from this application using the Kill method.
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("File path is invalid", "Warning Message", MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
                }

            }
            /*  }
              else {
                  Process[] processlist = Process.GetProcesses();

                  // Iterate over them
                  foreach (Process process in processlist)
                  {

                      // If the process appears on the Taskbar (if has a title)
                      // print the information of the process
                      if (!String.IsNullOrEmpty(process.MainWindowTitle))
                      {
                          //Console.WriteLine("Process:   {0}", process.ProcessName);
                          if (process.ProcessName == DynBtn.Name)
                          {
                              process.Kill();
                              DynBtn.BackColor = Color.Red;
                          }

                          //Console.WriteLine("    ID   : {0}", process.Id);
                          //Console.WriteLine("    Title: {0} \n", process.MainWindowTitle);
                      }
                  }
              }*/
        }

        private void minMaxButton_Click(object sender, EventArgs e)
        { //killing app

            Button DynBtn = (Button)sender;
            //Console.WriteLine(DynBtn.Name);
            //MessageBox.Show(DynBtn.Name+ " is closed");

            string btnName = DynBtn.Name.Substring(0, DynBtn.Name.Length - 6);
            Process[] procs = System.Diagnostics.Process.GetProcessesByName(btnName);

            //   IntPtr hWnd = FindWindow(btnName, "Untitled - Notepad");
            IntPtr hWnd = procs[0].MainWindowHandle;
            if (!hWnd.Equals(IntPtr.Zero))
            {
                // SW_SHOWMAXIMIZED to maximize the window
                // SW_SHOWMINIMIZED to minimize the window
                // SW_SHOWNORMAL to make the window be normal size
                // ShowWindowAsync(hWnd, SW_SHOWMAXIMIZED);
                SetForegroundWindow(procs[0].MainWindowHandle);
                ShowWindow(hWnd, SW_SHOWNORMAL);
            }

        }

        private void myProcess_Exited(object sender, EventArgs e)
        { //killing app

            Process myProcess = (Process)sender;
            string fileName = Path.GetFileNameWithoutExtension(myProcess.StartInfo.FileName);
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is Button)
                {
                    if (this.Controls[i].Name == fileName)
                    {
                        this.Controls[i].BackColor = Color.Red;
                    }
                }
            }
        }

        private void CreateDynamicButton(string buttonName, int index, string address)
        {
            //problem is existing running program cannot sync the program status for close

            //To determine the button to draw in red or green
            bool found = false;

            Process[] procs = Process.GetProcessesByName(buttonName);
            if (procs.Length > 0)
            {
                found = true;
            }
            //   }
            /*   else
               {
                   Process[] processlist = Process.GetProcesses();
                   foreach (Process process in processlist)
                   {
                       // If the process appears on the Taskbar (if has a title)
                       // print the information of the process
                       if (!String.IsNullOrEmpty(process.MainWindowTitle))
                       {
                           //Console.WriteLine("Process:   {0}", process.ProcessName);
                           if (process.ProcessName == buttonName)
                           {
                               found = true;
                           }
                           //Console.WriteLine("    ID   : {0}", process.Id);
                           //Console.WriteLine("    Title: {0} \n", process.MainWindowTitle);
                       }
                   }
               }*/

            // Create a Button object 
            Button dynamicButton = new Button();

            // Set Button properties
            dynamicButton.Height = 40;
            dynamicButton.Width = 300;

            Button minMaxButton = new Button();
            minMaxButton.Height = 40;
            minMaxButton.Width = 40;

            if (found)
            {
                dynamicButton.BackColor = Color.Green;
                dynamicButton.ForeColor = Color.Black;
            }
            else
            {
                dynamicButton.BackColor = Color.Red;
                dynamicButton.ForeColor = Color.Black;
            }

            dynamicButton.Location = new Point(20, -20 + 50 * index);
            dynamicButton.Text = buttonName;
            dynamicButton.Name = buttonName;
            dynamicButton.Font = new Font("Georgia", 16);
            toolTip1.SetToolTip(dynamicButton, address);
            // Add a Button Click Event handler
            dynamicButton.Click += new EventHandler(DynamicButton_Click);
            // Add Button to the Form. Placement of the Button
            // will be based on the Location and Size of button

            minMaxButton.Location = new Point(340, -20 + 50 * index);
            minMaxButton.Text = "-";
            minMaxButton.Name = buttonName + "MinMax";
            minMaxButton.Font = new Font("Georgia", 16);
            minMaxButton.Click += new EventHandler(minMaxButton_Click);

            Controls.Add(dynamicButton);
            Controls.Add(minMaxButton);

        }

        private void fileReadAndCreateButton()
        {
            // Default folder    
            //string rootFolder = @"C:\Temp\Data\";
            //Default file. MAKE SURE TO CHANGE THIS LOCATION AND FILE PATH TO YOUR FILE   
            string textFile = "ProgramList.txt"; //address @"C:\Temp\Data\Authors.txt"
            if (File.Exists(textFile))
            {
                // Read a text file line by line.  
                string[] lines = File.ReadAllLines(textFile);
                string[] lineSplit;
                int i = 0;

                foreach (string line in lines)
                {
                    lineSplit = line.Split(',');
                    //Console.WriteLine(line);
                    if (lineSplit.Length > 1)
                    {
                        CreateDynamicButton(lineSplit[0], ++i, lineSplit[1]);
                    }
                    else
                    {
                        CreateDynamicButton(lineSplit[0], ++i, " ");
                    }

                }
            }
        }

        private void generateProgramList()
        {
            List<string> lines = new List<string>();
            // 1. Create a list of the active processes of Windows
            Process[] processlist = Process.GetProcesses();
            // Iterate over them
            foreach (Process process in processlist)
            {
                // If the process appears on the Taskbar (if has a title)
                // print the information of the process
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    //Console.WriteLine("Process:   {0}", process.ProcessName);
                    //Console.WriteLine("    ID   : {0}", process.Id);
                    //Console.WriteLine("    Title: {0} \n", process.MainWindowTitle);
                    // and then closes the file.  You do NOT need to call Flush() or Close().
                    lines.Add(process.ProcessName);
                }
            }
            System.IO.File.WriteAllLines("ProgramList.txt", lines.ToArray());
        }

        private void dynamicFormHeightWidth()
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            destroyAlltheButton();
            fileReadAndCreateButton();
            dynamicFormHeightWidth();
        }

        public void destroyAlltheButton()
        {
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is Button) this.Controls[i].Dispose();
            }
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", AppDomain.CurrentDomain.BaseDirectory + @ "\ProgramList.txt");
        }
    }
}