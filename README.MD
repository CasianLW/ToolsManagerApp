# Tools Manager

![gif](https://media.giphy.com/media/v1.Y2lkPTc5MGI3NjExcHg1cWRoZjRqbjFqb2RhdHZnMmF0NDVjZjhiaHprZ2d3aWZwZ2l5cCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/TKXrzWjZhgs1JtLVd5/giphy.gif)

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Project Structure](#project-structure)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Introduction

ToolsManagerApp is a comprehensive tool management application designed to streamline the process of managing tools, categories, and users. It offers functionality for adding, updating, deleting, and viewing tools and categories, as well as managing user accounts. This app is built using the Microsoft MAUI framework for cross-platform compatibility.

## Features

- User authentication and session management
- CRUD operations for users, tools, and categories
- Role-based navigation (Admin and Employee)
- Tool assignment to users
- QR code generation and management for tools
- Error handling and logging

## Technologies Used

- .NET MAUI
- MongoDB
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- DotNetEnv for environment variable management

## Project Structure

1. Model: Represents the data structure and business logic
2. View: Defines the user interface (UI) and is responsible for displaying data.
3. ViewModel: Acts as an intermediary between the Model and View, handling data binding and command logic.
4. Handles data operations and communication with the database. Provides an abstraction layer between the data access layer and the business logic.
   Example found in our project:
   ```
   ToolsManagerApp/
   │
   ├── Models/
   │   ├── Category.cs
   │   ├── Consumable.cs
   │   ├── MongoDBSettings.cs
   │   ├── RoleEnum.cs
   │   ├── StatusEnum.cs
   │   ├── StringToBoolConverter.cs
   │   ├── Tool.cs
   │   ├── User.cs
   │
   ├── Repositories/
   │   ├── CategoryRepository.cs
   │   ├── ICategoryRepository.cs
   │   ├── IToolRepository.cs
   │   ├── IUserRepository.cs
   │   ├── ToolRepository.cs
   │   ├── UserRepository.cs
   │
   ├── ViewModels/
   │   ├── CategoriesViewModel.cs
   │   ├── LoginViewModel.cs
   │   ├── ToolsViewModel.cs
   │   ├── UsersViewModel.cs
   │   ├── UserToolsViewModel.cs
   │   ├── UserViewModel.cs
   ```
   \*This version is structured for clarity and readability, making it easy for others to understand the project organization and MVVM pattern.

## Installation

1. Clone the repository:

   ```sh
   git clone https://github.com/CasianLW/ToolsManagerApp.git
   cd ToolsManagerApp
   ```

2. Set up the environment variables:

   - Create a `.env` file in the root directory with the following content (use .`.env.example`):
     ```env
     MONGODB_CONNECTION_STRING=your_mongodb_connection_string
     MONGO_DATABASE_NAME=DdName
     ```

3. Build the project:

   ```sh
   dotnet build
   ```

4. Run the project:
   ```sh
   dotnet run
   ```

## Usage

1. Launch the application.
2. Log in with the admin credentials (`admin@example.com` / `adminpassword`).
3. Manage users, tools, and categories via the respective pages.
4. Employees can log in to view tools assigned to them.

## Contributing

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/your-feature`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/your-feature`).
5. Open a pull request.

## License

This project is licensed under the MIT License.

## Other informations :

\*that i might delete later

dotnet installations:

dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.Maui.Resizetizer --version 8.0.40
dotnet add package MongoDB.Driver
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package DotNetEnv

after all installed, do:
dotnet clean
dotnet restore
dotnet build
