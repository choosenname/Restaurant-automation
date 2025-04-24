# 🍽️ Restaurant Automation App

**Restaurant Automation App** is a desktop application built using WPF (.NET) to streamline and automate daily operations of a restaurant, covering everything from order processing and employee scheduling to dish management and system administration.

## 📦 Project Structure

```grapgql
WpfApp1/
├── App.xaml, App.xaml.cs # App startup
├── MainWindow.xaml, .cs # Entry point window
├── ManagerWindow.xaml, .cs # Manager interface
├── RestoranAdminWindow.xaml, .cs # Admin interface for the restaurant
├── SystemAdminWindow.xaml, .cs # System administration panel
├── WaiterWindow.xaml, .cs # Waiter interface
├── EmployeeSchedulePage.xaml, .cs # Schedule management
├── Models/
│ ├── Database/
│ │ ├── Order, Dish, Employee... # Data entities
│ └── DatabaseContext.cs # EF-style data context
├── Services/
│ └── MenuService.cs # Business logic for menu operations
├── SystemAdmin/
│ ├── Add/Delete Dish/Employee/Category
│ ├── AllDishes, AllEmployees
│ ├── DeleteItem.cs, DeleteViewModel.cs
├── Waiter/
│ ├── AddOrder, EndOrder
│ └── SplitCheckWindow.xaml, .cs
├── Images/
│ └── logout.png
├── WpfApp1.csproj # Project file
└── WpfApp1.sln # Visual Studio solution file
```

## ✨ Features

- **Role-based UI**:
  - **System Admin**: Manage employees, dishes, categories
  - **Restaurant Admin**: Oversee operations and generate reports
  - **Manager**: Schedule staff and manage workflow
  - **Waiter**: Create, manage, and close orders

- **Entity Management**:
  - Full CRUD for dishes, employees, categories
  - Schedule planning and viewing

- **Order Handling**:
  - Add and complete orders
  - Split checks between customers

## 🖥️ Technologies Used

- **WPF (Windows Presentation Foundation)**
- **C# (.NET Framework)**
- **MVVM pattern (partial)**
- **XAML for UI**
- **Entity-style models (not full EF)**

## 🚀 Getting Started

1. Open the solution file `WpfApp1.sln` in [Visual Studio](https://visualstudio.microsoft.com/).
2. Build the solution.
3. Run the application (set `WpfApp1` as the startup project).

## 📄 License

This project is for educational and demonstration purposes. For commercial use, contact the original authors.
