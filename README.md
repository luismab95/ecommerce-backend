# E-commerce Backend API

Una API RESTful robusta para una plataforma de comercio electrónico construida con **ASP.NET Core 10.0**, siguiendo los principios de **Clean Architecture** y **Domain-Driven Design (DDD)**.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Arquitectura](#-arquitectura)
- [Tecnologías](#-tecnologías)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Uso](#-uso)
- [Endpoints de la API](#-endpoints-de-la-api)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Licencia](#-licencia)

## ✨ Características

### Autenticación y Autorización

- ✅ Sistema de autenticación basado en **JWT (JSON Web Tokens)**
- ✅ Registro e inicio de sesión de usuarios
- ✅ Gestión de sesiones con refresh tokens
- ✅ Recuperación y restablecimiento de contraseñas
- ✅ Control de acceso basado en roles (Cliente/Administrador)

### Gestión de Productos

- ✅ CRUD completo de productos
- ✅ Soporte para múltiples imágenes por producto
- ✅ Gestión de stock e inventario
- ✅ Productos destacados
- ✅ Paginación y filtrado

### Gestión de Pedidos (Orders)

- ✅ Creación y gestión de pedidos
- ✅ Conversión de carrito de compras a pedido
- ✅ Actualización de estado de pedidos (Admin)
- ✅ Cancelación de pedidos
- ✅ Historial de pedidos

### Carrito de Compras

- ✅ Persistencia de carrito con MongoDB
- ✅ Caché de carrito con Redis (Alto rendimiento)
- ✅ Sincronización de carrito

### Gestión de Categorías

- ✅ CRUD completo de categorías
- ✅ Relación con productos
- ✅ Imágenes de categorías

### Gestión de Usuarios

- ✅ Administración de usuarios
- ✅ Actualización de perfiles
- ✅ Cambio de roles
- ✅ Activación/desactivación de cuentas

### Gestión de Imágenes

- ✅ Carga de imágenes al servidor local
- ✅ Gestión de imágenes de productos
- ✅ Servicio de almacenamiento local

### Observabilidad y Monitoreo

- ✅ Health Checks
- ✅ Métricas con OpenTelemetry
- ✅ Logging estructurado con Serilog

### Características Técnicas

- ✅ Arquitectura limpia (Clean Architecture)
- ✅ Patrón Repository
- ✅ Casos de uso (Use Cases)
- ✅ Validación de modelos
- ✅ Manejo centralizado de errores
- ✅ Documentación Swagger/OpenAPI
- ✅ CORS configurado
- ✅ Entity Framework Core con SQL Server
- ✅ Base de datos NoSQL (MongoDB) para datos volátiles

## 🏗️ Arquitectura

El proyecto sigue los principios de **Clean Architecture**, dividido en 4 capas principales:

```
┌─────────────────────────────────────────┐
│         Ecommerce.Api (Presentation)    │
│  Controllers, Filters, Configurations   │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      Ecommerce.Application (Use Cases)  │
│   Business Logic, DTOs, Use Cases       │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        Ecommerce.Domain (Entities)      │
│   Entities, Interfaces, Domain Logic    │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│   Ecommerce.Infrastructure (Data)       │
│  Repositories, DbContext, Services      │
└─────────────────────────────────────────┘
```

### Capas del Proyecto

- **Ecommerce.Api**: Capa de presentación con controladores y configuración de la API
- **Ecommerce.Application**: Lógica de aplicación y casos de uso
- **Ecommerce.Domain**: Entidades de dominio e interfaces
- **Ecommerce.Infrastructure**: Implementación de repositorios y servicios externos

## 🛠️ Tecnologías

- **Framework**: .NET 10.0
- **ORM Relacional**: Entity Framework Core 10.0
- **Base de Datos Relacional**: SQL Server
- **Base de Datos NoSQL**: MongoDB (Carrito de compras)
- **Caché**: Redis
- **Autenticación**: JWT Bearer Tokens
- **Documentación**: Swagger/Swashbuckle
- **Email**: MailKit (SMTP)
- **Logging**: Serilog
- **Observabilidad**: OpenTelemetry
- **Almacenamiento**: Sistema de archivos local

### Paquetes NuGet Principales

```xml
- Microsoft.AspNetCore.Authentication.JwtBearer (10.0.0)
- Microsoft.EntityFrameworkCore (10.0.0)
- Microsoft.EntityFrameworkCore.SqlServer (10.0.0)
- Swashbuckle.AspNetCore (10.0.1)
- MongoDB.Driver (3.5.2)
- Microsoft.Extensions.Caching.StackExchangeRedis (10.0.1)
- Serilog.AspNetCore (10.0.0)
- MailKit (4.14.1)
```

## 📦 Requisitos Previos

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB, Express, o superior)
- [MongoDB](https://www.mongodb.com/try/download/community) (Local o Atlas)
- [Redis](https://redis.io/download) (Opcional, pero recomendado para caché)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

## 🚀 Instalación

1. **Clonar el repositorio**

```bash
git clone https://github.com/luismab95/ecommerce-backend.git
cd ecommerce-backend
```

2. **Restaurar dependencias**

```bash
dotnet restore
```

3. **Configurar la base de datos**

Edita el archivo `appsettings.json` en el proyecto `Ecommerce.Api` con tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EcommerceDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

4. **Aplicar migraciones (SQL Server)**

```bash
cd Ecommerce.Api
dotnet ef database update
```

5. **Ejecutar la aplicación**

```bash
dotnet run
```

La API estará disponible en: `https://localhost:7000` (o el puerto configurado)

## ⚙️ Configuración

### Configuración JWT

En `appsettings.json`, configura los parámetros JWT:

```json
{
  "JwtSettings": {
    "SecretKey": "tu-clave-secreta-super-segura-de-al-menos-32-caracteres",
    "Issuer": "EcommerceAPI",
    "Audience": "EcommerceClient",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

### Configuración NoSQL y Caché

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "EcommerceNoSql"
  },
  "RedisSettings": {
    "ConnectionString": "localhost:6379"
  }
}
```

### Configuración de Email

Para habilitar el envío de correos (recuperación de contraseña):

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "tu-email@gmail.com",
    "SenderPassword": "tu-contraseña-de-aplicación",
    "SenderName": "E-commerce"
  }
}
```

> **Nota**: Para Gmail, necesitas crear una [contraseña de aplicación](https://support.google.com/accounts/answer/185833).

### Configuración de CORS

El proyecto está configurado para permitir cualquier origen. Para producción, modifica en `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("NewPolicy", app =>
    {
        app.WithOrigins("https://tu-dominio.com")
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});
```

## 📖 Uso

### Acceder a Swagger

Una vez ejecutada la aplicación, accede a la documentación interactiva:

```
https://localhost:7000/swagger
```

### Autenticación

1. **Registrar un usuario**

   - Endpoint: `POST /api/auth/signup`
   - Por defecto, los usuarios se crean con rol "Cliente"

2. **Iniciar sesión**

   - Endpoint: `POST /api/auth/signin`
   - Recibirás un `accessToken` y un `refreshToken`

3. **Usar el token**
   - En Swagger, haz clic en el botón "Authorize"
   - Ingresa: `Bearer {tu-token}`
   - Ahora puedes acceder a endpoints protegidos

## 🔌 Endpoints de la API

### Autenticación (`/api/auth`)

| Método | Endpoint           | Descripción                          | Auth |
| ------ | ------------------ | ------------------------------------ | ---- |
| POST   | `/signup`          | Registrar nuevo usuario              | No   |
| POST   | `/signin`          | Iniciar sesión                       | No   |
| POST   | `/signout`         | Cerrar sesión                        | Sí   |
| POST   | `/refresh-token`   | Renovar token de acceso              | No   |
| POST   | `/forgot-password` | Solicitar recuperación de contraseña | No   |
| POST   | `/reset-password`  | Restablecer contraseña               | No   |

### Usuarios (`/api/user`)

| Método | Endpoint     | Descripción            | Auth | Rol   |
| ------ | ------------ | ---------------------- | ---- | ----- |
| GET    | `/`          | Listar usuarios        | Sí   | Admin |
| GET    | `/{id}`      | Obtener usuario por ID | Sí   | Admin |
| PUT    | `/{id}`      | Actualizar usuario     | Sí   | Admin |
| DELETE | `/{id}`      | Eliminar usuario       | Sí   | Admin |
| PUT    | `/{id}/role` | Cambiar rol de usuario | Sí   | Admin |

### Productos (`/api/product`)

| Método | Endpoint | Descripción                 | Auth | Rol   |
| ------ | -------- | --------------------------- | ---- | ----- |
| GET    | `/`      | Listar productos (paginado) | No   | -     |
| GET    | `/{id}`  | Obtener producto por ID     | No   | -     |
| POST   | `/`      | Crear producto              | Sí   | Admin |
| PUT    | `/{id}`  | Actualizar producto         | Sí   | Admin |
| DELETE | `/{id}`  | Eliminar producto           | Sí   | Admin |

### Categorías (`/api/category`)

| Método | Endpoint | Descripción                  | Auth | Rol   |
| ------ | -------- | ---------------------------- | ---- | ----- |
| GET    | `/`      | Listar categorías (paginado) | No   | -     |
| GET    | `/{id}`  | Obtener categoría por ID     | No   | -     |
| POST   | `/`      | Crear categoría              | Sí   | Admin |
| PUT    | `/{id}`  | Actualizar categoría         | Sí   | Admin |
| DELETE | `/{id}`  | Eliminar categoría           | Sí   | Admin |

### Pedidos (`/api/orders`)

| Método | Endpoint         | Descripción                 | Auth | Rol   |
| ------ | ---------------- | --------------------------- | ---- | ----- |
| GET    | `/`              | Listar pedidos (Filtros)    | Sí   | -     |
| GET    | `/{orderId}`     | Obtener detalle de pedido   | Sí   | -     |
| POST   | `/`              | Crear nuevo pedido          | Sí   | -     |
| POST   | `/shopping-cart` | Crear pedido desde carrito  | Sí   | -     |
| PUT    | `/{orderId}`     | Actualizar estado de pedido | Sí   | Admin |
| DELETE | `/{orderId}`     | Cancelar pedido             | Sí   | -     |

### Imágenes (`/api/image`)

| Método | Endpoint               | Descripción                  | Auth | Rol   |
| ------ | ---------------------- | ---------------------------- | ---- | ----- |
| GET    | `/product/{productId}` | Obtener imágenes de producto | No   | -     |
| POST   | `/upload`              | Subir imagen                 | Sí   | Admin |
| DELETE | `/{id}`                | Eliminar imagen              | Sí   | Admin |

### Salud y Monitoreo (`/api/health`)

| Método | Endpoint   | Descripción          | Auth |
| ------ | ---------- | -------------------- | ---- |
| GET    | `/`        | Estado del servicio  | Sí   |
| GET    | `/metrics` | Métricas del sistema | Sí   |

## 📁 Estructura del Proyecto

```
Ecommerce/
├── Ecommerce.Api/                    # Capa de presentación
│   ├── Controllers/                  # Controladores de la API
│   │   ├── AuthController.cs
│   │   ├── CategoryController.cs
│   │   ├── HealthCheckController.cs
│   │   ├── ImageController.cs
│   │   ├── OrderController.cs
│   │   ├── ProductController.cs
│   │   └── UserController.cs
│   ├── Filters/
│   ├── Configurations/
│   ├── Program.cs
│   └── appsettings.json
│
├── Ecommerce.Application/            # Capa de aplicación
│   └── UseCases/
│       ├── Auth/
│       ├── Categories/
│       ├── Images/
│       ├── Orders/
│       ├── Products/
│       └── Users/
│
├── Ecommerce.Domain/                 # Capa de dominio
│   ├── Entities/
│   │   ├── Category.cs
│   │   ├── Image.cs
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   ├── Session.cs
│   │   └── User.cs
│   ├── DTOs/
│   └── Interfaces/
│
└── Ecommerce.Infrastructure/         # Capa de infraestructura
    ├── Data/
    │   ├── ApplicationDbContext.cs
    ├── Mongo/                       # Documentos y Mappers Mongo
    │   ├── Documents/
    │   └── Mappers/
    ├── Repositories/
    │   ├── ShoppingCartRepository.cs
    │   └── ...
    └── Services/
```

## 🔐 Seguridad

- **Contraseñas**: Hasheadas usando algoritmos seguros
- **JWT**: Tokens firmados con clave secreta
- **HTTPS**: Recomendado para producción
- **Validación**: Validación de entrada en todos los endpoints
- **Autorización**: Control de acceso basado en roles

## 🧪 Testing

Para ejecutar las pruebas:

```bash
dotnet test
```

## 📝 Licencia

Este proyecto está licenciado bajo la Licencia MIT. Consulta el archivo [LICENSE.txt](LICENSE.txt) para más detalles.

---

## 👨‍💻 Desarrollo

### Agregar una nueva migración

```bash
cd Ecommerce.Api
dotnet ef migrations add NombreDeLaMigracion
dotnet ef database update
```

### Revertir una migración

```bash
dotnet ef database update MigracionAnterior
dotnet ef migrations remove
```

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Haz fork del proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📧 Contacto

Para preguntas o sugerencias, por favor abre un issue en el repositorio.

---

**Desarrollado con ❤️ usando ASP.NET Core 10.0**
