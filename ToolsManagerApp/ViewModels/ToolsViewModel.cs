using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
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
        private readonly ILogger<ToolsViewModel> _logger;

        public ToolsViewModel() { }

        public ToolsViewModel(IToolRepository toolRepository, ICategoryRepository categoryRepository, ILogger<ToolsViewModel> logger)
        {
            _toolRepository = toolRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;

            LoadToolsCommand = new AsyncRelayCommand(LoadToolsAsync);
            AddToolCommand = new AsyncRelayCommand(AddToolAsync);
            UpdateToolCommand = new AsyncRelayCommand(UpdateToolAsync);
            DeleteToolCommand = new AsyncRelayCommand(DeleteToolAsync);

            Tools = new ObservableCollection<Tool>();
            Categories = new ObservableCollection<Category>();

            LoadToolsCommand.Execute(null);
        }

        public ObservableCollection<Tool> Tools { get; }
        public ObservableCollection<Category> Categories { get; }

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
                    NewToolCategory = Categories.FirstOrDefault(c => c.Id == value.CategoryId);
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

        private Category _newToolCategory;
        public Category NewToolCategory
        {
            get => _newToolCategory;
            set => SetProperty(ref _newToolCategory, value);
        }

        public IAsyncRelayCommand LoadToolsCommand { get; }
        public IAsyncRelayCommand AddToolCommand { get; }
        public IAsyncRelayCommand UpdateToolCommand { get; }
        public IAsyncRelayCommand DeleteToolCommand { get; }

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

                // Set default category if available
                if (Categories.Any())
                {
                    NewToolCategory = Categories.First();
                }
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
                    Status = StatusEnum.Available
                };

                await _toolRepository.AddToolAsync(newTool);
                Tools.Add(newTool);

                NewToolName = string.Empty;
                NewToolDescription = string.Empty;
                NewToolCategory = Categories.FirstOrDefault();
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
                    SelectedTool.CategoryId = NewToolCategory.Id;

                    await _toolRepository.UpdateToolAsync(SelectedTool);
                    await LoadToolsAsync(); // Reload tools after update
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
                    await _toolRepository.DeleteToolAsync(SelectedTool.Id.ToString());
                    Tools.Remove(SelectedTool);
                    await LoadToolsAsync(); // Reload tools after delete
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete tool");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete tool", "OK");
            }
        }
    }
}
