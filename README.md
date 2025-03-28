# FleetLogSystem 🚚📦

## Description

**FleetLogSystem** is a comprehensive **Fleet Management System (FMS)** developed in **.NET 6**, designed to streamline integration and logistics workflows for Enterprise Resource Planning (ERP) systems and marketplaces.

### Key Capabilities
The system provides end-to-end management of the integration chain between marketplaces and ERPs, offering:
- Full inventory control
- Automatic invoice issuance
- Real-time logistics tracking

## 🛠 Features

- **ERP Integration**: Seamless support for Bling, Tiny, and other systems
- **Marketplace Synchronization**: Direct integration with Mercado Livre
- **Inventory Management**: Real-time ERP and marketplace stock synchronization
- **Invoice Processing**: Automatic NF-e generation and submission
- **Order Lifecycle Management**: Complete tracking from purchase to delivery
- **Comprehensive Logging**: Detailed operation and transaction records

## 💻 Technologies

![.NET](https://img.shields.io/badge/.NET-6-purple)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-blue)
![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red)
![Authentication](https://img.shields.io/badge/Authentication-JWT-green)

- **.NET 6**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Core MVC** with **Razor**
- **JWT Authentication**

## 🚀 Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- SQL Server
- Entity Framework Core

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/pedro-fontinele/FleetLogSystem.git
   cd FleetLogSystem
   ```

2. Configure the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=FleetLogDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
   }
   ```

3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

4. Launch the application:
   ```bash
   dotnet run
   ```

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. Fork the repository
2. Create a feature branch
   ```bash
   git checkout -b feature/amazing-improvement
   ```
3. Commit your changes
   ```bash
   git commit -m "Add an amazing improvement"
   ```
4. Push to your branch
   ```bash
   git push origin feature/amazing-improvement
   ```
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 📞 Contact

- **Email**: contact@fleetlogsystem.com
- **GitHub**: [@pedro-fontinele](https://github.com/pedro-fontinele)
- **LinkedIn**: [Pedro Fontinele](https://www.linkedin.com/in/pedro-fontinele)

---

**FleetLogSystem** - Simplifying Logistics for ERPs and Marketplaces! 🚢🌐
