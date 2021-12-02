using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Win32;
using System.IO;

namespace generative_raster_filters
{
    public partial class MainWindow : Window
    {
        private string loadedImageName;
        private string outputDirectory;
        private Bitmap loadedImage;
        private string filterName;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Image files (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            if (ofd.ShowDialog() == true)
            {
                loadedImage = new Bitmap(ofd.FileName);
                FileName.Content = ofd.SafeFileName;
                loadedImageName = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
                FileInfo.Content = "Resolution: " + loadedImage.Width + "x" + loadedImage.Height + ", Format: " + loadedImage.PixelFormat.ToString().Substring(6);
                if (outputDirectory == null)
                {
                    var d = System.IO.Path.GetDirectoryName(ofd.FileName);
                    outputDirectory = d;
                    DirectoryName.Content = Helpers.ShortFile(d, 20);
                }
            }
        }
        private void ProcessImage_Click(object sender, RoutedEventArgs e)
        {
            if (loadedImage != null && outputDirectory != null)
            {
                Bitmap bmpE = null;
                try
                {
                    if (filterName == "Triangles")
                    {
                        bmpE = Filters.TriangleFilter(loadedImage, Convert.ToInt16(Factor1.Text), Convert.ToSingle(Factor2.Text));
                    }
                    else
                    {
                        throw new Exception("Invalid filter");
                    }
                }
                catch
                {
                    throw new Exception("Error in processing, possible invalid factor inputs");
                }
                Raster.SaveFile(bmpE, outputDirectory, loadedImageName + "grfedit.png");
            }
        }
        private void DirectorySelect_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker = true;
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                outputDirectory = ofd.FileName;
                DirectoryName.Content = Helpers.ShortFile(ofd.FileName, 20);
            }
        }

        private void FilterSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var si = (ComboBoxItem)FilterSelect.SelectedItem;
            filterName = si.Content.ToString();
            var sf = Filters.GetFilter(filterName);
            Factor1.Text = sf.factor1Default.ToString();
            Factor2.Text = sf.factor2Default.ToString();
            string line1 = sf.description;
            string line2 = "Factor 1: " + sf.factor1Description;
            string line3 = "Factor 2: " + sf.factor2Description;
            FilterDescription.Content = line1 + "\n" + line2 + "\n" + line3;
        }
    }
}
