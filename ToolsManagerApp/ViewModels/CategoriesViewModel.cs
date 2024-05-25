using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using ToolsManagerApp.Models;
using ToolsManagerApp.Repositories;

namespace ToolsManagerApp.ViewModels
{
    public class CategoriesViewModel : ObservableObject
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoriesViewModel> _logger;

        public CategoriesViewModel() { }

        public CategoriesViewModel(ICategoryRepository categoryRepository, ILogger<CategoriesViewModel> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;

            LoadCategoriesCommand = new AsyncRelayCommand(LoadCategoriesAsync);
            AddCategoryCommand = new AsyncRelayCommand(AddCategoryAsync);
            UpdateCategoryCommand = new AsyncRelayCommand(UpdateCategoryAsync);
            DeleteCategoryCommand = new AsyncRelayCommand(DeleteCategoryAsync);
            UnselectCategoryCommand = new RelayCommand(UnselectCategory);

            Categories = new ObservableCollection<Category>();

            LoadCategoriesCommand.Execute(null);
        }

        public ObservableCollection<Category> Categories { get; }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                if (value != null)
                {
                    NewCategoryName = value.Name;
                    NewCategoryDescription = value.Description;
                }
            }
        }

        private string _newCategoryName;
        public string NewCategoryName
        {
            get => _newCategoryName;
            set => SetProperty(ref _newCategoryName, value);
        }

        private string _newCategoryDescription;
        public string NewCategoryDescription
        {
            get => _newCategoryDescription;
            set => SetProperty(ref _newCategoryDescription, value);
        }

        public IAsyncRelayCommand LoadCategoriesCommand { get; }
        public IAsyncRelayCommand AddCategoryCommand { get; }
        public IAsyncRelayCommand UpdateCategoryCommand { get; }
        public IAsyncRelayCommand DeleteCategoryCommand { get; }
        public IRelayCommand UnselectCategoryCommand { get; }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                Categories.Clear();
                var categories = await _categoryRepository.GetAllCategoriesAsync();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load categories");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load categories", "OK");
            }
        }

        private async Task AddCategoryAsync()
        {
            try
            {
                var newCategory = new Category
                {
                    Name = NewCategoryName,
                    Description = NewCategoryDescription
                };

                await _categoryRepository.AddCategoryAsync(newCategory);
                Categories.Add(newCategory);

                ClearForm();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add category");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to add category", "OK");
            }
        }

        private async Task UpdateCategoryAsync()
        {
            try
            {
                if (SelectedCategory != null)
                {
                    SelectedCategory.Name = NewCategoryName;
                    SelectedCategory.Description = NewCategoryDescription;

                    await _categoryRepository.UpdateCategoryAsync(SelectedCategory);
                    await LoadCategoriesAsync(); // Reload categories after update
                    UnselectCategory();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to update category", "OK");
            }
        }

        private async Task DeleteCategoryAsync()
        {
            try
            {
                if (SelectedCategory != null)
                {
                    await _categoryRepository.DeleteCategoryAsync(SelectedCategory.Id);
                    Categories.Remove(SelectedCategory);
                    await LoadCategoriesAsync(); // Reload categories after delete
                    UnselectCategory();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to delete category", "OK");
            }
        }

        private void UnselectCategory()
        {
            SelectedCategory = null;
            ClearForm();
        }

        private void ClearForm()
        {
            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;
        }
    }
}
