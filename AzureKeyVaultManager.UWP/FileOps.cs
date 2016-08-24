using System;
using System.IO;
using System.Threading.Tasks;

namespace AzureKeyVaultManager.UWP
{
    public static class FileOps
    {
        public static async Task<string> ReadStringFromSelectedFile()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".dat");
            picker.FileTypeFilter.Add(".");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    var storedFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(file.Path);
                    using (var readStream = await storedFile.OpenReadAsync())
                    using (var sr = new System.IO.StreamReader(readStream.AsStream()))
                    {
                        return sr.ReadToEndAsync();
                    }
                });
            }
            return null;
        }
    }
}
