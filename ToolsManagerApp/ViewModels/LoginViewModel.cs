using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using ToolsManagerApp.Repositories;
using ToolsManagerApp.Models;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System;

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

        private bool _isLoginSuccessful;
        public bool IsLoginSuccessful
        {
            get => _isLoginSuccessful;
            set => SetProperty(ref _isLoginSuccessful, value);
        }

        public ICommand LoginCommand { get; }

        private async Task OnLoginAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            IsLoginSuccessful = false;

            try
            {
                var user = await _userRepository.GetUserByEmailAndPasswordAsync(Email, Password);
                if (user != null)
                {
                    IsLoginSuccessful = true;
                    // Handle successful login (navigate to next page)
                    await Shell.Current.GoToAsync("//UserPage");
                }
                else
                {
                    // Handle login failure (show error message)
                    ErrorMessage = "Invalid email or password.";
                    await Application.Current.MainPage.DisplayAlert("Login Failed", ErrorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during login
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
