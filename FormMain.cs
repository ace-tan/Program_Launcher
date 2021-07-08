using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace ProgramStatus
{
    public partial class FormMain : Form
    {
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

         //   if (DynBtn.Name.Equals("PCIO") || DynBtn.Name.Equals("SmcPci"))
         //   {
            if (DynBtn.BackColor == Color.Green)
            {
                Process[] procs = System.Diagnostics.Process.GetProcessesByName(DynBtn.Name);
                if (procs.Length > 0)
                {
                    procs[0].Kill();
                    DynBtn.BackColor = Color.Red;
                }
            }
            else if (DynBtn.BackColor == Color.Red) {
                string address = toolTip1.GetToolTip(DynBtn);
                if (File.Exists(address) == true)
                {
                    Process myProcess = new Process();
                    try
                    {
                        myProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(address);
                        myProcess.StartInfo.UseShellExecute = false;
                        // You can start any process, HelloWorld is a do-nothing example.
                        myProcess.StartInfo.FileName = address;
                        myProcess.StartInfo.CreateNoWindow = true;
                        myProcess.Start();
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
                else {
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

        private void CreateDynamicButton(string buttonName, int index,  string address)
        {
            //problem is existing running program cannot sync the program status for close

            //To determine the button to draw in red or green
            bool found = false;

         //   if (buttonName.Equals("PCIO") || buttonName.Equals("SmcPci")) 
        //    {
                Process[] procs = Process.GetProcessesByName(buttonName);
                if (procs.Length>0) {
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
            Controls.Add(dynamicButton);
        }

        private void fileReadAndCreateButton()
        {
            // Default folder    
            //string rootFolder = @"C:\Temp\Data\";
            //Default file. MAKE SURE TO CHANGE THIS LOCATION AND FILE PATH TO YOUR FILE   
            string textFile = "ProgramList.txt";//address @"C:\Temp\Data\Authors.txt"
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
    }
}
