using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UnZip
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void btn_Unzip_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder appFolder = ApplicationData.Current.LocalFolder;
                var zipFile = await Package.Current.InstalledLocation.GetFileAsync("Assets\\TestZip.zip");
                using (var zipFileStream = await zipFile.OpenStreamForReadAsync())
                {
                    using (MemoryStream memoryStream = new MemoryStream((int)zipFileStream.Length))
                    {
                        await zipFileStream.CopyToAsync(memoryStream);

                        using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                        {
                            foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                            {
                                string[] arrayFolder = zipArchiveEntry.FullName.Split(new string[] { "/" }, StringSplitOptions.None);
                                string folderName = string.Empty;

                                for (int i = 0; i < arrayFolder.Length - 1; i++)
                                {
                                    folderName += "\\" + arrayFolder[i];
                                }

                                if (zipArchiveEntry.Name != "")
                                {
                                    using (Stream fileData = zipArchiveEntry.Open())
                                    {
                                        StorageFile outputFile = await appFolder.CreateFileAsync(folderName + "\\" + zipArchiveEntry.Name, CreationCollisionOption.ReplaceExisting);
                                        using (Stream outputFileStream = await outputFile.OpenStreamForWriteAsync())
                                        {
                                            await fileData.CopyToAsync(outputFileStream);
                                            await outputFileStream.FlushAsync();
                                        }
                                    }
                                }
                                else
                                {
                                    await appFolder.CreateFolderAsync(folderName, CreationCollisionOption.ReplaceExisting);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
