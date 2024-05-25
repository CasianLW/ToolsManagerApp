using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using ToolsManagerApp.Models;
using ToolsManagerApp.Repositories;
using ToolsManagerApp.Services;

namespace ToolsManagerApp.ViewModels
{
    public class ToolsViewModel : ObservableObject
    {
        private readonly IToolRepository _toolRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ToolsViewModel> _logger;

        public ToolsViewModel() { }

        public ToolsViewModel(IToolRepository toolRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, ILogger<ToolsViewModel> logger)
        {
            _toolRepository = toolRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _logger = logger;

            LoadToolsCommand = new AsyncRelayCommand(LoadToolsAsync);
            AddToolCommand = new AsyncRelayCommand(AddToolAsync);
            UpdateToolCommand = new AsyncRelayCommand(UpdateToolAsync);
            DeleteToolCommand = new AsyncRelayCommand(DeleteToolAsync);
            UnselectToolCommand = new RelayCommand(UnselectTool);

            Tools = new ObservableCollection<Tool>();
            Categories = new ObservableCollection<Category>();
            Users = new ObservableCollection<User>();
            Statuses = new ObservableCollection<StatusEnum>(Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>());

            LoadToolsCommand.Execute(null);
        }

        public ObservableCollection<Tool> Tools { get; }
        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<User> Users { get; }
        public ObservableCollection<StatusEnum> Statuses { get; }

        private Tool _selectedTool;
        public Tool SelectedTool
        {
            get => _selectedTool;
            set
            {
                SetProperty(ref _selectedTool, value);
                if (value != null)
                {
                    NewToolName = value.Name;
                    NewToolDescription = value.Description;
                    NewToolQRCode = value.QRCode;
                    NewToolCategory = Categories.FirstOrDefault(c => c.Id == value.CategoryId);
                    NewToolAssignedUser = Users.FirstOrDefault(u => u.Id == value.UserAssignedId);
                    NewToolStatus = value.Status;
                }
            }
        }

        private string _newToolName;
        public string NewToolName
        {
            get => _newToolName;
            set => SetProperty(ref _newToolName, value);
        }

        private string _newToolDescription;
        public string NewToolDescription
        {
            get => _newToolDescription;
            set => SetProperty(ref _newToolDescription, value);
        }

        private string _newToolQRCode;
        public string NewToolQRCode
        {
            get => _newToolQRCode;
            set => SetProperty(ref _newToolQRCode, value);
        }

        private Category _newToolCategory;
        public Category NewToolCategory
        {
            get => _newToolCategory;
            set => SetProperty(ref _newToolCategory, value);
        }

        private User _newToolAssignedUser;
        public User NewToolAssignedUser
        {
            get => _newToolAssignedUser;
            set => SetProperty(ref _newToolAssignedUser, value);
        }

        private StatusEnum _newToolStatus;
        public StatusEnum NewToolStatus
        {
            get => _newToolStatus;
            set => SetProperty(ref _newToolStatus, value);
        }

        public IAsyncRelayCommand LoadToolsCommand { get; }
        public IAsyncRelayCommand AddToolCommand { get; }
        public IAsyncRelayCommand UpdateToolCommand { get; }
        public IAsyncRelayCommand DeleteToolCommand { get; }
        public IRelayCommand UnselectToolCommand { get; }

        private async Task LoadToolsAsync()
        {
            try
            {
                Tools.Clear();
                var tools = await _toolRepository.GetAllToolsAsync();
                foreach (var tool in tools)
                {
                    Tools.Add(tool);
                }

                Categories.Clear();
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                Users.Clear();
                var users = await _userRepository.GetAllUsersAsync();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                // Set default category if available
                if (Categories.Any())
                {
                    NewToolCategory = Categories.First();
                }

                // Set default status
                NewToolStatus = StatusEnum.Working;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load tools");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load tools", "OK");
            }
        }

        private async Task AddToolAsync()
        {
            try
            {
                if (NewToolCategory == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please select a category", "OK");
                    return;
                }

                var newTool = new Tool
                {
                    Name = NewToolName,
                    Description = NewToolDescription,
                    CategoryId = NewToolCategory.Id,
                    QRCode = NewToolQRCode,
                    UserAssignedId = NewToolAssignedUser?.Id,
                    Status = NewToolStatus
                };

                await _toolRepository.AddToolAsync(newTool);
                Tools.Add(newTool);

                if (NewToolAssignedUser != null)
                {
                    NewToolAssignedUser.AssignedToolIds.Add(newTool.Id);
                    await _userRepository.UpdateUserAsync(NewToolAssignedUser);
                }

                ClearForm();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tool");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add tool", "OK");
            }
        }

        private async Task UpdateToolAsync()
        {
            try
            {
                if (SelectedTool != null)
                {
                    SelectedTool.Name = NewToolName;
                    SelectedTool.Description = NewToolDescription;
                    SelectedTool.QRCode = NewToolQRCode;
                    SelectedTool.CategoryId = NewToolCategory.Id;
                    SelectedTool.UserAssignedId = NewToolAssignedUser?.Id;
                    SelectedTool.Status = NewToolStatus;

                    await _toolRepository.UpdateToolAsync(SelectedTool);

                    var previousUser = Users.FirstOrDefault(u => u.AssignedToolIds.Contains(SelectedTool.Id));
                    if (previousUser != null && previousUser.Id != NewToolAssignedUser?.Id)
                    {
                        previousUser.AssignedToolIds.Remove(SelectedTool.Id);
                        await _userRepository.UpdateUserAsync(previousUser);
                    }

                    if (NewToolAssignedUser != null && !NewToolAssignedUser.AssignedToolIds.Contains(SelectedTool.Id))
                    {
                        NewToolAssignedUser.AssignedToolIds.Add(SelectedTool.Id);
                        await _userRepository.UpdateUserAsync(NewToolAssignedUser);
                    }

                    await LoadToolsAsync();
                    UnselectTool();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update tool");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update tool", "OK");
            }
        }

        private async Task DeleteToolAsync()
        {
            try
            {
                if (SelectedTool != null)
                {
                    await _toolRepository.DeleteToolAsync(SelectedTool.Id);

                    var user = Users.FirstOrDefault(u => u.AssignedToolIds.Contains(SelectedTool.Id));
                    if (user != null)
                    {
                        user.AssignedToolIds.Remove(SelectedTool.Id);
                        await _userRepository.UpdateUserAsync(user);
                    }

                    Tools.Remove(SelectedTool);
                    ClearForm();
                    UnselectTool();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete tool");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete tool", "OK");
            }
        }

        private void UnselectTool()
        {
            SelectedTool = null;
            ClearForm();
        }

        private void ClearForm()
        {
            NewToolName = string.Empty;
            NewToolDescription = string.Empty;
            NewToolQRCode = string.Empty;
            NewToolCategory = Categories.FirstOrDefault();
            NewToolAssignedUser = null;
            NewToolStatus = StatusEnum.Working;
        }
    }
}
