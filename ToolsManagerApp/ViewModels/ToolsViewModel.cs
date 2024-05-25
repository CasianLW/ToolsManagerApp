using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolsManagerApp.Models;
using ToolsManagerApp.Repositories;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.Logging;

namespace ToolsManagerApp.ViewModels
{
    public class ToolsViewModel : ObservableObject
    {
        private readonly IToolRepository _toolRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<ToolsViewModel> _logger;

        // Parameterless constructor for XAML usage
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
        }

        public ObservableCollection<Tool> Tools { get; }
        public ObservableCollection<Category> Categories { get; }

        private Tool _selectedTool;
        public Tool SelectedTool
        {
            get => _selectedTool;
            set => SetProperty(ref _selectedTool, value);
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
                var newTool = new Tool
                {
                    Name = NewToolName,
                    Description = NewToolDescription,
                    CategoryId = NewToolCategory.Id
                };
                await _toolRepository.AddToolAsync(newTool);
                Tools.Add(newTool);
                NewToolName = string.Empty;
                NewToolDescription = string.Empty;
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
                    await _toolRepository.UpdateToolAsync(SelectedTool);
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
                    Tools.Remove(SelectedTool);
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
