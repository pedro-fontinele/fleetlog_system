```markdown
# FleetLogSystem

## Description
The **FleetLogSystem** is a **FMS (Fleet Management System)** developed in **.NET 6**, using **Entity Framework (EF)** and **SQL Server**. It was created to manage integration and logistics flow for ERPs such as **Bling**, **Tiny**, and marketplaces like **Mercado Livre**.

This system handles the entire integration chain between marketplaces and ERPs, enabling full inventory control, invoice issuance, and logistics tracking.

## Features
- **ERP Integration**: Supports Bling, Tiny, and other systems.
- **Marketplace Synchronization**: Direct integration with Mercado Livre.
- **Inventory Management**: Real-time synchronization between ERP and marketplace.
- **Invoice Issuance**: Automatic generation and submission of NF-e.
- **Order Management**: Processing of orders from purchase to delivery.
- **Monitoring and Logs**: Records all operations and transactions performed within the system.

## Technologies Used
- **.NET 6**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Core MVC** with **Razor (CSHTML)**
- **JWT Authentication**

## Requirements
Before starting, ensure you have the following installed:
- **.NET 6 SDK**
- **SQL Server**
- **Entity Framework Core**

## Setup Instructions
1. Clone the repository:
   ```sh
   git clone https://github.com/pedro-fontinele/FleetLogSystem.git
   ```
2. Navigate to the project directory:
   ```sh
   cd FleetLogSystem
   ```
3. Configure the connection string in **appsettings.json**:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=FleetLogDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
   }
   ```
4. Apply database migrations:
   ```sh
   dotnet ef database update
   ```
5. Start the application:
   ```sh
   dotnet run
   ```

## Contribution
Feel free to contribute! To do so:
1. Fork the repository.
2. Create a new branch for your feature:
   ```sh
   git checkout -b my-new-feature
   ```
3. Make the necessary changes and commit:
   ```sh
   git commit -m "Adding a new feature"
   ```
4. Push your changes to your fork:
   ```sh
   git push origin my-new-feature
   ```
5. Open a Pull Request in the main repository.

## License
This project is licensed under the **MIT** license. For more details, see the [LICENSE](LICENSE) file.

## Contact
For more information or support, reach out:
- **Email**: contact@fleetlogsystem.com
- **GitHub**: [pedro-fontinele](https://github.com/pedro-fontinele)
- **LinkedIn**: [Pedro Fontinele](https://linkedin.com/in/pedro-fontinele)

---
**FleetLogSystem - Simplifying logistics for ERPs and Marketplaces!**
