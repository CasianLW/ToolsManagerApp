using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using ToolsManagerApp.Models;
using ToolsManagerApp.Repositories;

namespace ToolsManagerApp.ViewModels
{
    public class ToolsViewModel : ObservableObject
    {
        private readonly IToolRepository _toolRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<ToolsViewModel> _logger;

        private ObservableCollection<object> _items;
        private object _selectedItem;
        private string _selectedType;

        public ObservableCollection<string> Types { get; } = new ObservableCollection<string> { "Tool", "Consumable" };

        public ObservableCollection<object> Items
        {
            get => _items;
            private set => SetProperty(ref _items, value);
        }

        public string SelectedType
        {
            get => _selectedType;
            set
            {
                SetProperty(ref _selectedType, value);
                OnPropertyChanged(nameof(IsConsumableSelected));
                _ = LoadItemsAsync(); // Trigger UI updates and refresh list
            }
        }

        public bool IsConsumableSelected => SelectedType == "Consumable";

        public ToolsViewModel() { }

        public ToolsViewModel(IToolRepository toolRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, ILogger<ToolsViewModel> logger)
        {
            _toolRepository = toolRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _logger = logger;

            Items = new ObservableCollection<object>();
            SelectedType = "Tool"; // Default to showing tools

            LoadItemsAsync().ConfigureAwait(false);
        }

        private async Task LoadItemsAsync()
        {
            try
            {
                if (IsConsumableSelected)
                {
                    var consumables = await _toolRepository.GetAllConsumablesAsync();
                    Items = new ObservableCollection<object>(consumables);
                }
                else
                {
                    var tools = await _toolRepository.GetAllToolsAsync();
                    Items = new ObservableCollection<object>(tools);
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

                if (Categories.Any())
                {
                    NewCategory = Categories.First();
                }

                NewStatus = StatusEnum.Working;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load items");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load items", "OK");
            }
        }

        public IAsyncRelayCommand LoadItemsCommand => new AsyncRelayCommand(LoadItemsAsync);
        public IAsyncRelayCommand AddItemCommand => new AsyncRelayCommand(AddItemAsync);
        public IAsyncRelayCommand UpdateItemCommand => new AsyncRelayCommand(UpdateItemAsync);
        public IAsyncRelayCommand DeleteItemCommand => new AsyncRelayCommand(DeleteItemAsync);
        public IRelayCommand UnselectItemCommand => new RelayCommand(UnselectItem);

        private async Task AddItemAsync()
        {
            try
            {
                if (NewCategory == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Please select a category", "OK");
                    return;
                }

                if (IsConsumableSelected)
                {
                    var newConsumable = new Consumable
                    {
                        Name = NewName,
                        Description = NewDescription,
                        CategoryId = NewCategory.Id,
                        QRCode = NewQRCode,
                        UserAssignedId = NewAssignedUser?.Id,
                        Status = NewStatus,
                        InitialValue = NewInitialValue,
                        MaxValue = NewMaxValue,
                        CurrentValue = NewCurrentValue
                    };

                    await _toolRepository.AddConsumableAsync(newConsumable);
                    Items.Add(newConsumable);
                }
                else
                {
                    var newTool = new Tool
                    {
                        Name = NewName,
                        Description = NewDescription,
                        CategoryId = NewCategory.Id,
                        QRCode = NewQRCode,
                        UserAssignedId = NewAssignedUser?.Id,
                        Status = NewStatus
                    };

                    await _toolRepository.AddToolAsync(newTool);
                    Items.Add(newTool);
                }

                if (NewAssignedUser != null)
                {
                    NewAssignedUser.AssignedToolIds.Add(((Tool)SelectedItem).Id);
                    await _userRepository.UpdateUserAsync(NewAssignedUser);
                }

                ClearForm();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add item");
                // await Application.Current.MainPage.DisplayAlert("Error", "Failed to add item", "OK");
            }
        }

        private async Task UpdateItemAsync()
        {
            try
            {
                if (SelectedItem != null)
                {
                    string selectedItemId = null;

                    if (SelectedItem is Consumable consumable)
                    {
                        consumable.Name = NewName;
                        consumable.Description = NewDescription;
                        consumable.QRCode = NewQRCode;
                        consumable.CategoryId = NewCategory.Id;
                        consumable.UserAssignedId = NewAssignedUser?.Id;
                        consumable.Status = NewStatus;
                        consumable.InitialValue = NewInitialValue;
                        consumable.MaxValue = NewMaxValue;
                        consumable.CurrentValue = NewCurrentValue;

                        await _toolRepository.UpdateConsumableAsync(consumable);
                        selectedItemId = consumable.Id;
                    }
                    else if (SelectedItem is Tool tool)
                    {
                        tool.Name = NewName;
                        tool.Description = NewDescription;
                        tool.QRCode = NewQRCode;
                        tool.CategoryId = NewCategory.Id;
                        tool.UserAssignedId = NewAssignedUser?.Id;
                        tool.Status = NewStatus;

                        await _toolRepository.UpdateToolAsync(tool);
                        selectedItemId = tool.Id;
                    }

                    if (selectedItemId != null)
                    {
                        var previousUser = Users.FirstOrDefault(u => u.AssignedToolIds.Contains(selectedItemId));
                        if (previousUser != null && previousUser.Id != NewAssignedUser?.Id)
                        {
                            previousUser.AssignedToolIds.Remove(selectedItemId);
                            await _userRepository.UpdateUserAsync(previousUser);
                        }

                        if (NewAssignedUser != null && !NewAssignedUser.AssignedToolIds.Contains(selectedItemId))
                        {
                            NewAssignedUser.AssignedToolIds.Add(selectedItemId);
                            await _userRepository.UpdateUserAsync(NewAssignedUser);
                        }
                    }

                    await LoadItemsAsync();
                    UnselectItem();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update item");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update item", "OK");
            }
        }

        private async Task DeleteItemAsync()
        {
            try
            {
                if (SelectedItem != null)
                {
                    string selectedItemId = null;

                    if (SelectedItem is Consumable consumable)
                    {
                        selectedItemId = consumable.Id;
                        await _toolRepository.DeleteConsumableAsync(consumable.Id);
                    }
                    else if (SelectedItem is Tool tool)
                    {
                        selectedItemId = tool.Id;
                        await _toolRepository.DeleteToolAsync(tool.Id);
                    }

                    if (selectedItemId != null)
                    {
                        var user = Users.FirstOrDefault(u => u.AssignedToolIds.Contains(selectedItemId));
                        if (user != null)
                        {
                            user.AssignedToolIds.Remove(selectedItemId);
                            await _userRepository.UpdateUserAsync(user);
                        }
                    }

                    Items.Remove(SelectedItem);
                    ClearForm();
                    UnselectItem();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete item");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete item", "OK");
            }
        }

        private void UnselectItem()
        {
            SelectedItem = null;
            ClearForm();
        }

        private void ClearForm()
        {
            NewName = string.Empty;
            NewDescription = string.Empty;
            NewQRCode = string.Empty;
            NewCategory = Categories.FirstOrDefault();
            NewAssignedUser = null;
            NewStatus = StatusEnum.Working;
            NewInitialValue = 0;
            NewMaxValue = 0;
            NewCurrentValue = 0;
        }

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                if (value != null)
                {
                    if (value is Consumable consumable)
                    {
                        NewName = consumable.Name;
                        NewDescription = consumable.Description;
                        NewQRCode = consumable.QRCode;
                        NewCategory = Categories.FirstOrDefault(c => c.Id == consumable.CategoryId);
                        NewAssignedUser = Users.FirstOrDefault(u => u.Id == consumable.UserAssignedId);
                        NewStatus = consumable.Status;
                        NewInitialValue = consumable.InitialValue;
                        NewMaxValue = consumable.MaxValue;
                        NewCurrentValue = consumable.CurrentValue;
                    }
                    else if (value is Tool tool)
                    {
                        NewName = tool.Name;
                        NewDescription = tool.Description;
                        NewQRCode = tool.QRCode;
                        NewCategory = Categories.FirstOrDefault(c => c.Id == tool.CategoryId);
                        NewAssignedUser = Users.FirstOrDefault(u => u.Id == tool.UserAssignedId);
                        NewStatus = tool.Status;
                    }
                }
            }
        }

        private string _newName;
        public string NewName
        {
            get => _newName;
            set => SetProperty(ref _newName, value);
        }

        private string _newDescription;
        public string NewDescription
        {
            get => _newDescription;
            set => SetProperty(ref _newDescription, value);
        }

        private string _newQRCode;
        public string NewQRCode
        {
            get => _newQRCode;
            set => SetProperty(ref _newQRCode, value);
        }

        private Category _newCategory;
        public Category NewCategory
        {
            get => _newCategory;
            set => SetProperty(ref _newCategory, value);
        }

        private User _newAssignedUser;
        public User NewAssignedUser
        {
            get => _newAssignedUser;
            set => SetProperty(ref _newAssignedUser, value);
        }

        private StatusEnum _newStatus;
        public StatusEnum NewStatus
        {
            get => _newStatus;
            set => SetProperty(ref _newStatus, value);
        }

        private int _newInitialValue;
        public int NewInitialValue
        {
            get => _newInitialValue;
            set => SetProperty(ref _newInitialValue, value);
        }

        private int _newMaxValue;
        public int NewMaxValue
        {
            get => _newMaxValue;
            set => SetProperty(ref _newMaxValue, value);
        }

        private int _newCurrentValue;
        public int NewCurrentValue
        {
            get => _newCurrentValue;
            set => SetProperty(ref _newCurrentValue, value);
        }

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();
        public ObservableCollection<User> Users { get; } = new ObservableCollection<User>();
        public ObservableCollection<StatusEnum> Statuses { get; } = new ObservableCollection<StatusEnum>(Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>());
    }
}
