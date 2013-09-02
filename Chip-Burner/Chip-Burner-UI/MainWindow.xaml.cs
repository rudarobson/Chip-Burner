using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChipBurner;

namespace Chip_Burner_UI
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //StreamReader reader = new StreamReader("./last.txt");
            //HexFileNameTextBox.Text = reader.ReadLine().TrimEnd();//@"C:\Users\rudarobson\Documents\GitHub\Chip-Burner\Firmware\Chip-Burner\dist\default\production\Chip-Burner.production.hex"
            //reader.Close();
        }


        private void HexFileClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            //dlg.InitialDirectory = HexFileNameTextBox.Text;// @"C:\Users\rudarobson\MPLABXProjects";
            dlg.DefaultExt = "Todos os arquivos (*.*)";
            dlg.Filter = "";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                HexFileNameTextBox.Text = filename;
            }
        }

        private void WriteButtonClick(object sender, RoutedEventArgs e)
        {
            Programmer.Write(HexFileNameTextBox.Text);
            MessageBox.Show("Written!");
        }

        private void VerifyButtonClick(object sender, RoutedEventArgs e)
        {
            if(!Programmer.Verify(HexFileNameTextBox.Text))
                MessageBox.Show("Verified!");
            else
                MessageBox.Show("Error Ocurred!");
        }

    }
}
