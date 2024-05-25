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
    public class UserToolsViewModel : ObservableObject
    {
        private readonly IToolRepository _toolRepository;
        private readonly ILogger<UserToolsViewModel> _logger;
        private readonly IUserRepository _userRepository;

        public UserToolsViewModel(IToolRepository toolRepository, ILogger<UserToolsViewModel> logger, IUserRepository userRepository)
        {
            _toolRepository = toolRepository;
            _logger = logger;
            _userRepository = userRepository;

            LoadUserToolsCommand = new AsyncRelayCommand(LoadUserToolsAsync);
            Tools = new ObservableCollection<Tool>();

            // Load user tools when the view model is initialized
            LoadUserToolsCommand.Execute(null);
        }

        public ObservableCollection<Tool> Tools { get; }

        private Tool _selectedTool;
        public Tool SelectedTool
        {
            get => _selectedTool;
            set => SetProperty(ref _selectedTool, value);
        }

        public IAsyncRelayCommand LoadUserToolsCommand { get; }

        private async Task LoadUserToolsAsync()
        {
            try
            {
                Tools.Clear();

                var currentUser = await _userRepository.GetUserByEmailAsync(UserSession.Instance.Email);

                if (currentUser != null)
                {
                    var tools = await _toolRepository.GetToolsByIdsAsync(currentUser.AssignedToolIds);
                    foreach (var tool in tools)
                    {
                        Tools.Add(tool);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user tools");
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load user tools", "OK");
            }
        }
    }
}
