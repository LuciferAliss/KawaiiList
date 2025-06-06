using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using KawaiiList.Models;
using KawaiiList.Services;
using Microsoft.Win32;
using Syroot.Windows.IO;

namespace KawaiiList.ViewModels
{
    public partial class EditingProfileViewModel : BaseViewModel, IDropTarget
    {
        public ICommand LoadFilesCommand { get; }

        private readonly ICloseModalNavigationService _closeNavigationService;

        private int _indexChanged = 0;
        private List<FileModel?> _images = new List<FileModel?> { null, null }; // 0 - avatar, 1 - banner
        
        [ObservableProperty]        
        private int _maxWidthImage = 500;

        [ObservableProperty]
        private int _maxHeightImage = 500;

        [ObservableProperty]
        private bool _isImageChange = false;

        public ObservableCollection<FileModel> Files { get; set; } = [];

        public EditingProfileViewModel(ICloseModalNavigationService closeNavigationService) 
        {
            _closeNavigationService = closeNavigationService;

            LoadFilesCommand = new RelayCommand(LoadFiles);
        }

        private void LoadFiles()
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "All files (*.*)|*.*",
                InitialDirectory = KnownFolders.Downloads.Path
            };

            if (dlg.ShowDialog(Application.Current.MainWindow) == true)
            {
                var files = dlg.FileNames;
                foreach (var file in files)
                {
                    this.Files.Add(new FileModel(file));
                }
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo?.VisualSource is null
                && dropInfo.Data is DataObject dataObject
                && dataObject.GetDataPresent(DataFormats.FileDrop)
                && dataObject.ContainsFileDropList())
            {

                if (dataObject.GetData(DataFormats.FileDrop) is string[] files
                    && files.Length == 1 
                    && File.Exists(files[0])
                    && IsImageFile(files[0])) 
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                }
                else
                {
                    dropInfo.Effects = DragDropEffects.None;
                }
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.DragInfo?.VisualSource is null
                && dropInfo.Data is DataObject dataObject
                && dataObject.GetDataPresent(DataFormats.FileDrop)
                && dataObject.ContainsFileDropList())
            {
                var files = dataObject.GetFileDropList();
                if (files.Count == 1 && File.Exists(files[0]) && IsImageFile(files[0]))
                {
                    Files.Clear(); 
                    Files.Add(new FileModel(files[0]));
                    _images[_indexChanged] = new FileModel(files[0]);
                }
            }
            else
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }
        }

        private bool IsImageFile(string path)
        {
            var imageExtensions = new[] { ".png", ".jpg", ".jpeg" };
            var ext = Path.GetExtension(path)?.ToLowerInvariant();

            if (!imageExtensions.Contains(ext))
                return false;
            
            try
            {
                using var image = System.Drawing.Image.FromFile(path);
                return image.Width <= 500 && image.Height <= 500;
            }
            catch
            {
                return false;
            }
        }

        [RelayCommand]
        private void CloseModalWindow()
        {
            _closeNavigationService.Navigate();
        }

        [RelayCommand]
        private void ChangeImageAvatar()
        {
            IsImageChange = true;
            _indexChanged = 0;

            Files.Clear();
            if (_images[_indexChanged] != null)
            {
                Files.Add(_images[_indexChanged] ?? new());
            }

            MaxHeightImage = 500;
            MaxWidthImage = 500;
        }

        [RelayCommand]
        private void ChangeImageBanner()
        {
            IsImageChange = true;
            _indexChanged = 1;
            
            Files.Clear();
            if (_images[_indexChanged] != null)
            {
                Files.Add(_images[_indexChanged] ?? new());
            }

            MaxHeightImage = 1000;
            MaxWidthImage = 1920;
        }

        [RelayCommand]
        private void ClearFile()
        {
            IsImageChange = false;
            Files.Clear();
            _images = new List<FileModel?>() { null, null };
        }
    }
}