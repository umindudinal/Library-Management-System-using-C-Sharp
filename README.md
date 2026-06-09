# 📚 Sarasavi Library System

> A Windows desktop application for managing a library — built with C# Windows Forms and SQL Server — covering book registration, user management, loans, returns, reservations, and book inquiry.

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET Framework](https://img.shields.io/badge/.NET_Framework_4.7.2-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)

---

## 📖 About

**Sarasavi Library System** is a desktop-based Library Management System built with C# Windows Forms and SQL Server Express. It provides a full workflow for library staff — from registering books and users, issuing loans, processing returns, handling reservations, to searching the catalogue. The app starts with a secure login form and opens a main dashboard to access all modules.

---

## ✨ Features

- 🔐 **Login** — Secure username/password authentication from SQL `Login` table
- 🏠 **Main Dashboard** — Central navigation to all 6 modules + Logout
- 📗 **Book Registration** — Register books with auto-generated Book Number by classification (e.g. `CS0001`), Title, Author, Publisher, ISBN; creates a `Borrowable` or `Reference` copy simultaneously
- 👤 **User Registration** — Register Members or Visitors with auto-generated User Number (e.g. `U0001`), Name, NIC, Sex, Address, UserType
- 📤 **Loan** — Issue books to Members (Visitors cannot borrow); validates max 5 active loans per user; checks if copy is Reference-only or already on loan; sets 14-day return period automatically
- 📥 **Return** — Process book returns by Copy Number; updates loan status to `Returned`; checks for pending reservations and alerts staff if the returned book is reserved
- 🔖 **Reservation** — Reserve books for Members (Visitors cannot reserve); duplicate reservation check per user per book; stores with `Pending` status
- 🔍 **Inquiry** — Search book catalogue by Book Number, Title (LIKE), or Author (LIKE); displays results in DataGridView with availability status (Available / On Loan / Reserved)

---

## 🛠️ Tech Stack

| Technology | Usage |
|------------|-------|
| C# (.NET Framework 4.7.2) | Application logic and UI |
| Windows Forms (WinForms) | Desktop GUI |
| SQL Server Express | Database (via `SqlConnection`) |
| ADO.NET | Database access (`SqlCommand`, `SqlDataReader`, `SqlDataAdapter`) |
| Visual Studio 2022 | IDE (`.slnx` solution format) |

---

## 🗄️ Database Schema

**Database name:** `SarasaviLibraryDB`

| Table | Key Columns |
|-------|-------------|
| `Login` | `Username`, `Password` |
| `Books` | `BookNumber` (PK), `Title`, `Author`, `Publisher`, `Classification`, `ISBN` |
| `Copies` | `CopyNumber` (PK), `BookNumber` (FK), `Status` (`Borrowable` / `Reference`) |
| `Users` | `UserNumber` (PK), `Name`, `Sex`, `NIC`, `Address`, `UserType` (`Member` / `Visitor`) |
| `Loans` | `LoanID` (PK), `CopyNumber`, `UserNumber`, `LoanDate`, `ExpectedReturnDate`, `ActualReturnDate`, `Status` (`Active` / `Returned`) |
| `Reservations` | `ReservationID` (PK), `BookNumber`, `UserNumber`, `ReservationDate`, `Status` (`Pending` / `Completed`) |

---

## 📁 Project Structure

```
SarasaviLibrarySystem/
│
├── SarasaviLibrarySystem.slnx      # Visual Studio solution file
├── SarasaviLibrarySystem.csproj    # Project file (.NET Framework 4.7.2)
├── App.config                      # Application configuration
├── Program.cs                      # Entry point — launches frmLogin
├── DatabaseHelper.cs               # SQL Server connection helper
│
├── frmLogin.cs / Designer.cs       # Login form
├── frmMain.cs / Designer.cs        # Main dashboard (navigation hub)
├── frmBookReg.cs / Designer.cs     # Book registration form
├── frmUserReg.cs / Designer.cs     # User registration form
├── frmLoan.cs / Designer.cs        # Book loan / issue form
├── frmReturn.cs / Designer.cs      # Book return form
├── frmReservation.cs / Designer.cs # Book reservation form
├── frmInquiry.cs / Designer.cs     # Book search / inquiry form
│
├── Properties/
│   ├── AssemblyInfo.cs
│   ├── Resources.Designer.cs
│   └── Settings.Designer.cs
│
└── bin/Debug/
    └── SarasaviLibrarySystem.exe   # Compiled executable
```

---

## 🚀 Getting Started

### Prerequisites

- Windows OS
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with .NET desktop development workload)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- SQL Server Management Studio (SSMS) — optional but recommended

### Setup Steps

1. **Clone or download the repository**
   ```bash
   git clone https://github.com/umindudinal/Library-Management-System-using-C-.git
   ```

2. **Create the database in SQL Server**

   Open SSMS and run:
   ```sql
   CREATE DATABASE SarasaviLibraryDB;
   ```
   Then create the required tables (`Login`, `Books`, `Copies`, `Users`, `Loans`, `Reservations`) with the schema above.

3. **Update the connection string** *(if needed)*

   Open `DatabaseHelper.cs` and update the server name to match your machine:
   ```csharp
   private static string connectionString =
       "Server=YOUR-PC-NAME\\SQLEXPRESS;Database=SarasaviLibraryDB;Integrated Security=True;";
   ```

4. **Open and run the project**
   - Open `SarasaviLibrarySystem.slnx` in Visual Studio 2022
   - Press **F5** or click **Start** to build and run
   - The Login form will launch

5. **Insert a login record to access the system**
   ```sql
   INSERT INTO Login (Username, Password) VALUES ('admin', 'admin123');
   ```

---

## 🧭 Application Flow

```
frmLogin
    └── frmMain (Dashboard)
            ├── frmBookReg    — Register books + copies
            ├── frmUserReg    — Register members / visitors
            ├── frmLoan       — Issue books (14-day loan period)
            ├── frmReturn     — Return books + reservation alert
            ├── frmReservation — Reserve books for members
            └── frmInquiry    — Search catalogue by title / author / book no.
```

---

## 📏 Business Rules

- **Loan limit** — A member can have a maximum of **5 active loans** at a time
- **Reference copies** — Copies marked as `Reference` cannot be borrowed or loaned
- **Visitor restrictions** — Visitors cannot borrow books or make reservations
- **Reservation alert** — When a book is returned, if it has a pending reservation, staff are alerted with the reserving member's name
- **Auto Book Number** — Generated as `{Classification}{0001}` e.g. `CS0001`, `EE0002`
- **Auto User Number** — Generated as `U{count+1}` e.g. `U0001`, `U0002`
- **Return date** — Automatically set to **14 days** from loan date

---

## 👨‍💻 Author

**Umindu Dinal**
- GitHub: [@umindudinal](https://github.com/umindudinal)
- Email: umindudinal@gmail.com

---

## 📄 License

Copyright © 2025 Umindu Dinal. All Rights Reserved.

---

<p align="center">Built with ❤️ using C# and SQL Server</p>
