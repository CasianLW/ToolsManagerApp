using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ToolsManagerApp.Models;
using ToolsManagerApp.Repositories;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;
using ToolsManagerApp.Services;

namespace ToolsManagerApp.ViewModels
{
    public class LoginViewModel : ObservableObject
    {
        private readonly IUserRepository _userRepository;

        public LoginViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            LoginCommand = new AsyncRelayCommand(OnLoginAsync);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        private async Task OnLoginAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _userRepository.GetUserByEmailAsync(Email);
                if (user != null && user.Password == Password)
                {
                    UserSession.Instance.Email = Email;
                    UserSession.Instance.Role = user.Role;

                    if (user.Role == RoleEnum.Admin)
                    {
                        await Shell.Current.GoToAsync("UserPage");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync("UserToolsPage");
                    }
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                    await Application.Current.MainPage.DisplayAlert("Login Failed", ErrorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                await Application.Current.MainPage.DisplayAlert("Error", ErrorMessage, "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
