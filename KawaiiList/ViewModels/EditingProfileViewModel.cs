using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GongSolutions.Wpf.DragDrop;
using KawaiiList.Models;
using KawaiiList.Services;
using KawaiiList.Stores;
using Microsoft.Win32;
using Supabase.Gotrue;
using Syroot.Windows.IO;
using static Supabase.Postgrest.Constants;
using Path = System.IO.Path;

namespace KawaiiList.ViewModels
{
    public partial class EditingProfileViewModel : BaseViewModel, IDropTarget, IDataErrorInfo
    {
        public ICommand LoadFilesCommand { get; }

        private readonly ICloseModalNavigationService _closeNavigationService;
        private readonly ISupaBaseService<Profiles> _profilesService;
        private readonly ISupaBaseService<UserImage> _userImageService;
        private readonly IStorageSupabaseService _storageService;
        private readonly UserStore _userStore;
        private readonly SupabaseClientStore _supabaseClientStore;

        private int _indexChanged = 0;
        private List<FileModel?> _images = [null, null]; // 0 - avatar, 1 - banner
        private bool _nicknameTouched;
        private bool _passwordTouched;

        [ObservableProperty]        
        private int _maxWidthImage = 1000;

        [ObservableProperty]
        private int _maxHeightImage = 1000;

        [ObservableProperty]
        private bool _isImageChange = false;

        [ObservableProperty]
        private string _errorSave = string.Empty;

        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _nickname = string.Empty;

        public ObservableCollection<FileModel> Files { get; set; } = [];

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Nickname):
                        if (!_nicknameTouched) return null;
                        if (string.IsNullOrEmpty(Nickname))
                            return string.Empty;
                        if (Regex.IsMatch(Nickname, @"[а-яА-Я]"))
                            return "Пароль не должен содержать русских символов";
                        if (Nickname.Length < 3)
                            return "Имя пользователя слишком короткое";
                        break;

                    case nameof(Password):
                        if (!_passwordTouched) return null;
                        if (string.IsNullOrWhiteSpace(Password))
                            return string.Empty;
                        if (Regex.IsMatch(Password, @"[а-яА-Я]"))
                            return "Пароль не должен содержать русских символов";
                        if (Password.Length < 6)
                            return "Пароль должен быть не короче 6 символов";
                        if (!Regex.IsMatch(Password, @"[A-Z]"))
                            return "Пароль должен содержать хотя бы одну заглавную букву";
                        if (!Regex.IsMatch(Password, @"[0-9]"))
                            return "Пароль должен содержать хотя бы одну цифру";
                        break;
                }

                return string.Empty;
            }
        }

        public bool IsFormValid =>
           string.IsNullOrEmpty(this[nameof(Nickname)]) &&
           string.IsNullOrEmpty(this[nameof(Password)]);

        public string Error => string.Empty;

        public EditingProfileViewModel(
            ICloseModalNavigationService closeNavigationService,
            ISupaBaseService<Profiles> supaBaseService,
            ISupaBaseService<UserImage> userImageService,
            IStorageSupabaseService storageService,
            UserStore userStore,
            SupabaseClientStore supabaseClientStore) 
        {
            _closeNavigationService = closeNavigationService;
            _profilesService = supaBaseService;
            _userImageService = userImageService;
            _storageService = storageService;
            _userStore = userStore;
            _supabaseClientStore = supabaseClientStore;

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
                return image.Width <= MaxWidthImage && image.Height <= MaxHeightImage;
            }
            catch
            {
                return false;
            }
        }

        partial void OnNicknameChanged(string value)
        {
            _nicknameTouched = true;
            OnPropertyChanged(nameof(IsFormValid));
            OnPropertyChanged("Item[]");
        }

        partial void OnPasswordChanged(string value)
        {
            _passwordTouched = true;
            OnPropertyChanged(nameof(IsFormValid));
            OnPropertyChanged("Item[]");
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

            MaxHeightImage = 1000;
            MaxWidthImage = 1000;
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

            MaxHeightImage = 1024;
            MaxWidthImage = 1920;
        }

        [RelayCommand]
        private void ClearFile()
        {
            IsImageChange = false;
            Files.Clear();
            _images = new List<FileModel?>() { null, null };
        }

        [RelayCommand]
        private async void SaveChanges()
        {
            if (!string.IsNullOrEmpty(Nickname))
            {
                FiltersQuery filtersQuery = new()
                {
                    ColumnName = "nickname",
                    OperatorFilter = Operator.Equals,
                    Value = Nickname
                };

                var respond = await _profilesService.GetFilter("*", filtersQuery);

                if (respond.Count() == 0)
                {
                    _userStore.CurrentUser.Nickname = Nickname;

                    Profiles profiles = new Profiles()
                    {
                        Id = _userStore.CurrentUser.Id,
                        Username = _userStore.CurrentUser.Username,
                        Nickname = _userStore.CurrentUser.Nickname,
                        Email = _userStore.CurrentUser.Email,
                    };

                    if (await _profilesService.Upsert(profiles, "id"))
                    {
                        ErrorSave = "Данные успешно сохранены!";
                    }
                    else
                    {
                        ErrorSave = "Данные не сохранены!";
                    }
                }
                else
                {
                    ErrorSave = "Пользователь с таким\nникнеймом уже существует";
                }
            }

            if (!string.IsNullOrEmpty(Password))
            {
                var respond = await _supabaseClientStore.Client.Auth.Update(new UserAttributes
                {
                    Password = Password
                });

                if (respond != null)
                {
                    ErrorSave = "Данные успешно сохранены!";
                }
                else
                {
                    ErrorSave = "Данные не сохранены!";
                }
            }

            if (_images[0]  != null)
            {
                string path = _images[0].File;

                byte[] fileBytes = await File.ReadAllBytesAsync(path);
                
                bool isLoad;
                string? filePath;

                (isLoad, filePath) = await _storageService.UploadImage(fileBytes, path, "avatar");

                if (!isLoad)
                {
                    ErrorSave = "Не удалось загрузить изображение аватара";
                    return;
                }

                UserImage userBannerImage = new UserImage
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = _userStore.CurrentUser.Id,
                    TypeImage = "avatar",
                    FileName = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                if (!await _userImageService.Upsert(userBannerImage))
                {
                    ErrorSave = "Не удалось загрузить изображение аватара";
                    return;
                }

                List<FiltersQuery> filters = new List<FiltersQuery>()
                {
                    new FiltersQuery()
                    {
                        ColumnName = "user_id",
                        OperatorFilter = Operator.Equals,
                        Value = _userStore.CurrentUser.Id
                    },
                    new FiltersQuery()
                    {
                        ColumnName = "type_image",
                        OperatorFilter = Operator.Equals,
                        Value = "avatar"
                    }
                };
                var userAvatarImages = await _userImageService.GetFilter("*", filters, "uploaded_at", Ordering.Descending);
                var userAvatarImage = userAvatarImages.FirstOrDefault();

                _userStore.CurrentUser.Images.AvatarUrl = $"images-{_userStore.CurrentUser.Id}/" + userAvatarImage.FileName;

                ErrorSave = "Данные успешно сохранены!";
            }

            if (_images[1] != null)
            {
                string path = _images[1].File;

                byte[] fileBytes = await File.ReadAllBytesAsync(path);

                bool isLoad;
                string? filePath;

                (isLoad, filePath) = await _storageService.UploadImage(fileBytes, path, "banner");

                if (!isLoad)
                {
                    ErrorSave = "Не удалось загрузить изображение баннера";
                    return;
                }

                UserImage userBannerImage = new UserImage
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = _userStore.CurrentUser.Id,
                    TypeImage = "banner",
                    FileName = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                if (!await _userImageService.Upsert(userBannerImage))
                {
                    ErrorSave = "Не удалось загрузить изображение баннера";
                    return;
                }

                List<FiltersQuery> filters = new List<FiltersQuery>()
                {
                    new FiltersQuery()
                    {
                        ColumnName = "user_id",
                        OperatorFilter = Operator.Equals,
                        Value = _userStore.CurrentUser.Id
                    },
                    new FiltersQuery()
                    {
                        ColumnName = "type_image",
                        OperatorFilter = Operator.Equals,
                        Value = "banner"
                    }
                };
                var userAvatarImages = await _userImageService.GetFilter("*", filters, "uploaded_at", Ordering.Descending);
                var userAvatarImage = userAvatarImages.FirstOrDefault();

                _userStore.CurrentUser.Images.BannerUrl = $"images-{_userStore.CurrentUser.Id}/" + userAvatarImage.FileName;

                ErrorSave = "Данные успешно сохранены!";
            }

            _userStore.CurrentUser = _userStore.CurrentUser;
        }
    }
}