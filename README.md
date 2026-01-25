# 🚀 Company.Project - .NET 8 Clean Architecture

A professional-grade backend foundation built on **.NET 8**. This project implements **Clean Architecture** principles, utilizing a **Generic Repository Pattern** and **Unit of Work** to ensure a strictly decoupled, maintainable, and testable codebase.

---

## 🏗️ Architecture Design & Layering

The solution is structured into four distinct layers. This separation ensures that business logic remains isolated from external concerns like databases or UI frameworks.



### 1. 🟢 Domain Layer (`Company.Project.Domain`)
The heart of the application, containing enterprise-wide logic and fundamental abstractions.
* **📂 Folders:** * `Models`: Contains the `BaseEntity` (Shared properties like `Id`, `CreatedAt`, `IsDeleted`).
    * `Interfaces`: Defines the contracts for `IBaseRepository` and `IUnitOfWork`.
    * `Enums`: Centralized domain enumerations.
* **📦 Packages:** * `Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.18)`
* **🔗 References:** None (Independent).

### 2. 🔵 Infrastructure Layer (`Company.Project.Infrastructure`)
Responsible for data persistence and technical implementations of Domain interfaces.
* **📂 Folders:** * `Repositories`: Concrete implementation of the `BaseRepository`.
    * `UnitOfWork`: Implementation of the `UnitOfWork` pattern.
    * `Filters`: Features the `DynamicFilterHelper` for advanced data querying.
    * `Migrations`: Database schema history.
    * `Context.cs`: The EF Core database context.
* **📦 Packages:** * `Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.18)`
    * `Microsoft.EntityFrameworkCore.SqlServer (8.0.18)`
    * `Microsoft.EntityFrameworkCore.Tools (8.0.18)`
* **🔗 References:** `Company.Project.Domain`

### 3. 🟡 Application Layer (`Company.Project.Application`)
Orchestrates business flow and defines the "how-to" of the application without worrying about the "where-to-save."
* **📂 Folders:** * `Services`: Business logic implementations.
    * `Contracts`: Service interfaces.
    * `Dto`: Data Transfer Objects.
    * `Helper`: Utilities like `JWT.cs`.
    * `Validation`: FluentValidation logic.
* **📦 Packages:** * `MediatR (13.0.0)`
    * `FluentValidation (12.0.0)`
    * `Stripe.net (48.4.0)`
    * `MailKit (4.13.0)` / `MimeKit`
    * `Newtonsoft.Json (13.0.4-beta1)`
    * `Moq (4.20.72)`
* **🔗 References:** `Company.Project.Infrastructure`

### 4. 🔴 Presentation Layer (`PL` / Web API)
The entry point of the application, handling HTTP requests and Dependency Injection.
* **📂 Folders:** * `Controllers`: API Endpoints.
    * `Program.cs`: System configuration and DI container.
* **📦 Packages:** * `Swashbuckle.AspNetCore (6.6.2)` (Swagger)
    * `Microsoft.AspNetCore.Authentication.JwtBearer (8.0.18)`
    * `FluentValidation.AspNetCore (11.3.1)`
    * `Microsoft.EntityFrameworkCore.Design (8.0.18)`
* **🔗 References:** `Company.Project.Application` | `Company.Project.Infrastructure`

---

## 🛠️ Design Patterns

* **Generic Repository Pattern:** Centralizes data access logic to reduce duplication.
* **Unit of Work:** Ensures all repository operations in a single request are treated as one atomic transaction.
* **Dynamic Filtering:** Uses a custom `DynamicFilterHelper` to apply complex runtime filters to `IQueryable` sources.

---

## 🔐 Feature: JWT Authentication & Identity Implementation

This project implements a secure, token-based authentication system using **ASP.NET Core Identity** and **JWT (JSON Web Tokens)**. It features user registration, role-based access control, and a robust **Refresh Token** mechanism.

### 🛠️ Step-by-Step Implementation Guide

#### 1. Domain Layer: Models & Core Identity
The foundation starts here by defining our data structures and core identity requirements.
* **Step A:** Install `Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.18)`.
* **Step B:** Create `ApplicationUser.cs` inheriting from `IdentityUser`. 
    * *Custom Logic:* Added read-only properties to calculate `Age` and `Gender` automatically by parsing the National ID (`NID`) string.
* **Step C:** Create `RefreshToken.cs` as a model to track user sessions, expiry dates, and revocation status.

#### 2. Infrastructure Layer: Persistence & Context
Integrating the identity models into the database engine.
* **Step A:** Install Identity and EF Core SQL Server packages.
* **Step B:** Update `Context.cs` to inherit from `IdentityDbContext<ApplicationUser>`.
* **Step C:** Configure entity relationships in `OnModelCreating`, specifically linking `ApplicationUser` to `RefreshTokens` with a **Cascade Delete** behavior.

#### 3. Application Layer: Business Logic & Contracts
This is where the authentication "engine" is built.
* **Step A:** Install `System.IdentityModel.Tokens.Jwt` and `FluentValidation`.
* **Step B:** Create **DTOs** in the `Dto/Account` folder:
    * `RegisterModel.cs`: Captures user details and NID.
    * `TokenRequestModel.cs`: For login attempts.
    * `AuthModel.cs`: Returns the JWT, Refresh Token, and user metadata.
* **Step C:** Create **`IAuthService.cs`** inside the **`Contracts`** folder to define the interface for registration, token generation, and token refreshing.
* **Step D:** Create **`JWT.cs`** in the **`Helper`** folder to serve as a configuration mapping class for `appsettings.json`.
* **Step E:** Implement **`AuthService.cs`** in the **`Services`** folder. This handles:
    * User creation and password hashing via `UserManager`.
    * JWT creation with claims (Email, Name, Roles).
    * Cryptographically secure `RefreshToken` generation using `RNGCryptoServiceProvider`.



#### 4. Presentation Layer (PL): API Security
The final step exposes the service to the outside world.
* **Step A:** Install `Microsoft.AspNetCore.Authentication.JwtBearer`.
* **Step B:** Configure `Program.cs`:
    * Register `IAuthService` with its implementation.
    * Add `AddAuthentication` and configure `JwtBearerOptions` (validating Issuer, Audience, and the Security Key).
    * Implement a **Role Seeding** method (`SeedRolesAsync`) to ensure "Admin" and "User" roles are created in the database upon startup.
* **Step C:** Create `AccountController.cs` to handle HTTP POST requests for `/Register`, `/token`, and `/refresh-token`.

---

### 🛡️ Key Capabilities
* **Dynamic User Metadata:** Age and Gender are never stored in the DB; they are computed on-the-fly from the NID.
* **Token Rotation:** When a user refreshes their session, the old refresh token is revoked and replaced by a new one.
* **Role Security:** Access to specific endpoints can be restricted using `[Authorize(Roles = "Admin")]`.


## 🛡️ Feature: OTP (One-Time Password) System

This feature implements **One-Time Password (OTP)** verification for secure email validation during registration or login. The system generates a 6-digit code, sends it via email, and validates it on the backend.

### How OTP Verification Was Built

1. **Domain Layer**  
   Go to the Domain layer and add the `OTP` model with fields: Id, Code, Email, ExpirationTime, IsUsed, UserId (foreign key to ApplicationUser).  
   Add navigation property `User` in the `ApplicationUser` model if needed for relationships.

2. **Infrastructure Layer**  
   Go to the Infrastructure layer and add the `DbSet<OTP>` to the Context class.  
   Configure the relationship between OTP and ApplicationUser in `OnModelCreating` if needed.  
   Create the `IOTPRepository` interface with methods: AddAsync, GetByEmailAsync, UpdateAsync.  
   Implement `OTPRepository` using EF Core (AddAsync with SaveChanges, GetByEmailAsync with OrderByDescending on ExpirationTime, UpdateAsync with SaveChanges).  
   Add the OTP repository to `IUnitOfWork` and initialize it in `UnitOfWork` constructor.  
   Run a new migration and update the database.

3. **Application Layer**  
   Go to the Application layer and add the DTOs: `EmailDTO` (To, Subject, Body) and `OTPDTO` (Email, Code).  
   Create the `IEmailService` interface with SendEmailAsync method.  
   Implement `EmailService` using SmtpClient (configure with Gmail SMTP, credentials from appsettings, send MailMessage).  
   Create the `IOTPService` interface with GenerateAndSendOTPAsync and ValidateOTPAsync methods.  
   Implement `OTPService` (generate random 6-digit code, create OTP entity with expiration, save via repository, send email via email service, validate code against DB entry and mark as used).  
   Register `IEmailService` and `IOTPService` in DI (add Scoped services in Program.cs, configure HttpClient if needed for email).

4. **Presentation Layer**  
   Go to the Presentation layer and add the `OTPController` with endpoints: POST `/send` (takes EmailDTO, calls service to generate and send) and POST `/validate` (takes OTPDTO, calls service to validate and return success/fail).  
   Add email settings to `appsettings.json` (Email, Password, Host, DisplayName, Port).  
   Configure SMTP in Program.cs if using external email service.

### Result
Users can request an OTP via email and validate it. OTPs expire automatically and can only be used once. The system integrates with email service for delivery and repository for storage.
---

### 🚀 Technical Highlights of this Pattern
* **Decoupling:** You can swap the database (e.g., SQL Server to PostgreSQL) by only changing the Infrastructure layer.
* **Dry Principle:** You don't write "Create/Update/Delete" code for every new entity; the `BaseRepository` handles it all.
* **Transaction Safety:** The `UnitOfWork` ensures that if you are saving an OTP and updating a User, either both succeed or both fail.

# AI Chatbot with Conversation History 🚀

This feature brings an **intelligent AI chatbot** to the app using **OpenRouter** (powered by Llama model) — users can chat naturally and the bot remembers the entire conversation history! 🧠💬

### How the AI Chatbot Was Built – Step by Step 🔥

1. **Domain Layer** 🏗️  
   Go to the **Domain** layer and create the `ChatBotMessages` model (inherits from BaseEntity).  
   Add fields: `Message`, `Sender` (User or Bot), `UserId`.  
   Add foreign key and navigation property to `ApplicationUser`.

2. **Infrastructure Layer** ⚙️  
   Go to the **Infrastructure** layer and add the `DbSet<ChatBotMessages>` to the `Context`.  
   Configure the relationship between ChatBotMessages and ApplicationUser in `OnModelCreating`.  
   Create the `IChatBotMessageRepository` interface (extends IBaseRepository<ChatBotMessages>) and add `GetUserMessagesAsync` method.  
   Implement `ChatBotMessageRepository` using EF Core (filter by UserId + order by CreatedAt).  
   Add the chatbot repository to `IUnitOfWork` and initialize it in `UnitOfWork`.  
   Run a new migration and update the database.

3. **Application Layer** 🧠  
   Go to the **Application** layer and create the `IChatBotMessageService` interface with methods:  
   - `SendMessageAsync` (returns user message + bot message)  
   - `GetMessagesByUserAsync`  
   - `GetAllMessagesAsync`  
   Implement `ChatBotMessageService` (inject IUnitOfWork + HttpClient).  
   In `SendMessageAsync`: save user message → fetch history → build prompt with history → call OpenRouter API → save bot reply → return both messages.  
   In `GetBotReplyAsync`: retrieve user messages → construct messages array (system prompt + history + current message) → POST to `/chat/completions` → parse JSON response.  
   Register the service in DI (add HttpClient with OpenRouter base URL and API key header).

4. **Presentation Layer** 🎯  
   Go to the **Presentation** layer and create the `ChatbotController` with endpoints:  
   - POST `/send` (accepts message, calls service, returns user & bot messages)  
   - GET `/history` (returns user's conversation history)  
   Add the OpenAI section to `appsettings.json` with `BaseURL` and `ApiKey`.

### Result ✨
Users can chat with an AI that remembers previous messages.  
Conversation history is stored per user in the database.  
Responses are powered by OpenRouter's Llama model — fast, natural, and contextual.  

Perfect foundation for customer support, personal assistant, or fun chat features!

## Image Upload & Download with Compression 📁

This feature enables **secure image upload, download, and metadata editing** with automatic **GZip compression** to save storage space and improve performance. Users can upload images, retrieve them, delete, or edit file names/types.

### How the Image Upload Feature Was Built – Step by Step 📸

1. **Domain Layer** 🏗️  
   Go to the **Domain** layer and create the `ImageFile` model (inherits BaseEntity).  
   Add fields: `FileName`, `FileType`, `Data` (byte array for compressed image).  
   Add static methods `Compress` and `Decompress` using GZipStream for compression/decompression.

2. **Infrastructure Layer** ⚙️  
   Go to the **Infrastructure** layer and add the `DbSet<ImageFile>` to the `Context`.  
   Create the `IImageFileRepository` interface (extends IBaseRepository<ImageFile>).  
   Implement `ImageFileRepositories` using the base repository (no custom methods needed yet).  
   Add the ImageFile repository to `IUnitOfWork` and initialize it in `UnitOfWork`.  
   Run a new migration and update the database.

3. **Application Layer** 🧠  
   Go to the **Application** layer and add the DTOs: `ImageDownloadDto` (Id, FileName, FileType, Data) and `EditImageMetaData` (Id, NewFileName, NewFileType).  
   Create the `IImageFileService` interface with methods: UploadImageAsync, DownloadFileAsync, DeleteImageAsync, ListAllImagesAsync, EditImageMetadataAsync.  
   Implement `ImageFileService` (inject IUnitOfWork).  
   In `UploadImageAsync`: compress data using static method, create ImageFile entity, save via repository.  
   In `DownloadFileAsync`: get entity, decompress data, return DTO.  
   In `EditImageMetadataAsync`: get entity, update FileName/FileType/UpdatedAt, save via repository.  
   Register the service in DI (add Scoped in Program.cs).

4. **Presentation Layer** 🎯  
   Go to the **Presentation** layer and create the `ImageFileController` with endpoints:  
   - POST `/upload` (accepts IFormFile, calls service to upload and compress, returns Id and FileName).  
   - GET `/{id}` (calls service to download, returns File with content type).  
   - DELETE `/{id}` (calls service to delete).  
   - GET `/list` (calls service to list all images).  
   - PUT `/{id}/edit` (takes EditImageMetaData, calls service to update metadata, returns updated info).
  
# Stripe Payment Integration 💳

This feature implements a complete **Stripe payment system** with support for creating payments, saving cards for future use, processing saved payments, refunds (full/partial), and viewing transaction history. All operations are secure and follow PCI compliance best practices.

### How the Stripe Payment System Was Built – Step by Step 💸

1. **Domain Layer** 🏗️  
   Go to the **Domain** layer and create the `PaymentTransaction` model (inherits BaseEntity) with fields: UserId, Gateway, ExternalTransactionId, Amount, Currency, Status, Description, CompletedAt, FailureReason.  
   Add navigation property `User` (ApplicationUser).  
   Create the `PaymentMethod` model (inherits BaseEntity) with fields: UserId, Gateway, ExternalId, TokenId, LastFourDigits, CardBrand, ExpiryMonth, ExpiryYear, IsDefault.  
   Add navigation property `User` (ApplicationUser).  
   Add the collections `PaymentTransactions` and `PaymentMethods` to the `ApplicationUser` model.  
   Create the `TransactionStatus` enum (Pending, Success, Failed, Refunded).

2. **Infrastructure Layer** ⚙️  
   Go to the **Infrastructure** layer and add the `DbSet<PaymentTransaction>` and `DbSet<PaymentMethod>` to the `Context`.  
   Configure the relationships between PaymentTransaction/PaymentMethod and ApplicationUser in `OnModelCreating`.  
   Create the `IStripeRepository` interface if needed (extends IBaseRepository for both models, but generic base repo handles it).  
   Add the PaymentTransaction and PaymentMethod repositories to `IUnitOfWork` and initialize them in `UnitOfWork` constructor.  
   Run a new migration and update the database.

3. **Application Layer** 🧠  
   Go to the **Application** layer and add the DTOs: `CreatePaymentRequest` (Amount, Currency, Gateway, Description, SavePaymentMethod, SavePaymentMethodId, CardToken), `PaymentResultDto` (IsSuccess, TransactionId, Status, ErrorMessage, ClientSecret, Transaction), `PaymentTransactionDto` (Id, Amount, Currency, Status, Description, CompletedAt, Gateway, ExternalTransactionId, CreatedAt), `SavePaymentMethodRequest` (Gateway, CardToken, SetAsDefault), `SavePaymentMethodResultDto` (IsSuccess, ErrorMessage, PaymentMethod), `PaymentMethodDto` (CreatedAt, UpdatedAt, IsDeleted, UserId, Gateway, ExternalId, TokenId, Last4, CardBrand, ExpMonth, ExpYear, IsDefault).  
   Create the `IStripeService` interface with methods: ProcessPaymentAsync, ProcessSavedPaymentAsync, SavePaymentMethodAsync, RefundPaymentAsync, GetPaymentMethods, GetTransactions.  
   Implement `StripeService` (inject IUnitOfWork and IConfiguration).  
   In `ProcessPaymentAsync`: get user, create/get Stripe customer, create PaymentIntent (with Confirm=false for frontend confirmation), save transaction as Pending, return result with ClientSecret.  
   In `SavePaymentMethodAsync`: attach payment method to customer, create PaymentMethod entity with card details, save to DB, set as default if requested.  
   In `RefundPaymentAsync`: get transaction, create refund via Stripe, update status to Refunded.  
   In `GetPaymentMethods` and `GetTransactions`: query via unit of work, map entities to DTOs.  
   Add Stripe keys to appsettings.json (SecretKey, PublishableKey).  
   Register the service in DI (add Scoped in Program.cs).

4. **Presentation Layer** 🎯  
   Go to the **Presentation** layer and create the `StripeController` with endpoints:  
   - POST `/process-payment` (takes CreatePaymentRequest, gets userId from JWT, calls service, returns PaymentResultDto with ClientSecret).  
   - POST `/process-saved-payment` (takes savedPaymentMethodId, amount, currency, calls service).  
   - POST `/save-payment-method` (takes SavePaymentMethodRequest, calls service, returns SavePaymentMethodResultDto).  
   - POST `/refund/{externalTransactionId}` (optional amount in body, calls service, returns success/fail).  
   - GET `/payment-methods` (gets userId from JWT, calls service, returns list of PaymentMethodDto).  
   - GET `/transactions` (gets userId from JWT, calls service, returns list of PaymentTransactionDto).
  
# ExampleClass – Demo CRUD Endpoint Setup

This feature demonstrates how to create a **simple CRUD endpoint** using the established Clean Architecture pattern. It shows the full flow from model to controller for basic operations (Create, Read, Update, Delete).

### How the ExampleClass Endpoint Was Built – Step by Step 📋

1. **Domain Layer** 🏗️  
   Go to the **Domain** layer and create the `ExampleClass` model (inherits BaseEntity).  
   Add fields: `Name` (string).  

2. **Infrastructure Layer** ⚙️  
   Go to the **Infrastructure** layer and add the `DbSet<ExampleClass>` to the `Context`.  
   Create the `IExampleClassRepository` interface (extends IBaseRepository<ExampleClass>) and add `GetAllAsync` method with pagination (skip, take).  
   Implement `ExampleClassRepository` using the base repository (no custom implementation needed for basic CRUD).  
   Add the ExampleClass repository to `IUnitOfWork` and initialize it in `UnitOfWork` constructor.  
   Run a new migration and update the database.

3. **Application Layer** 🧠  
   Go to the **Application** layer and add the DTOs: `CreateExampleClassDto` (Name) and `UpdateExampleClassDto` (Id, Name).  
   Create the `IExampleClassService` interface with methods: GetAllAsync (with skip/take), GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync.  
   Implement `ExampleClassService` (inject IUnitOfWork).  
   In `GetAllAsync`: call repository GetAllAsync with pagination.  
   In `CreateAsync`: create entity from DTO, add via unit of work, complete transaction.  
   In `UpdateAsync`: get entity by ID, update fields from DTO, update via unit of work, complete.  
   In `DeleteAsync`: get entity by ID, delete via unit of work, complete.  
   Add validators using FluentValidation: `CreateExampleClassValidator` (Name required, max length) and `UpdateExampleClassValidator` (Id > 0, Name required).  
   Register the service in DI (add Scoped in Program.cs).

4. **Presentation Layer** 🎯  
   Go to the **Presentation** layer and create the `ExampleClassController` with endpoints:  
   - GET `/` (calls service GetAllAsync with skip=0, take=10, returns list).  
   - GET `/{id}` (calls service GetByIdAsync, returns entity or NotFound).  
   - POST `/` (takes CreateExampleClassDto, validates, calls service CreateAsync, returns created entity).  
   - PUT `/{id}` (takes UpdateExampleClassDto, validates, calls service UpdateAsync, returns updated or NotFound).  
   - DELETE `/{id}` (calls service DeleteAsync, returns NoContent or NotFound).
