# RealEstate API

API para la gestión de propiedades inmobiliarias en EE. UU., desarrollada como prueba técnica.  
Se implementa utilizando .NET 6, Arquitectura Hexagonal, Entity Framework Core, JWT Authentication, y buenas prácticas de desarrollo (SOLID, inyección de dependencias, DTOs, Unit of Work, Repository Pattern).

---

## Tecnologías 
- .NET 6 Web API  
- Entity Framework Core (SQL Server)  
- JWT Authentication  
- Arquitectura Hexagonal  
- Unit of Work & Repository Pattern  
- Swagger (OpenAPI)  
- Pruebas unitarias  

---

## Backup de la BD 
- Se debe restaurar el backup de la BD, el archivo se encuentra en la raíz del repo con nombre **RealEstateBak.bak**  
- La base de datos restaurada queda con nombre **RealEstate**  
- Modificar la cadena de conexión en el `appsettings.json` para que apunte al servidor de SQL donde se restaure la BD:

  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RealEstate;Trusted_Connection=True;TrustServerCertificate=True"
  }

--- 

## Controllers

  ## AuthController
  
  #### Register 
  **POST** `/api/auth/register`  
    Permite registrar un nuevo usuario para autenticarse en la API
  
  ---
  
  #### Login 
  **POST** `/api/auth/login`  
    Permite generar el token de autenticacion con el email y contraseña del usuario registrado
    Para autorizar los endpoints en swagger o postman se debe agregar el token precedido de la palabra Bearer  

 ---
 
 ## PropertiesController
  
  #### Crear propiedad
  **POST** `/api/properties`  
    Crea una nueva propiedad.  
  
  ---    
  
  #### Actualizar propiedad
  **PUT** `/api/properties/{id}`  
    Actualiza una propiedad existente por su **ID**.  
  
  ---
  
  #### Cambiar precio de propiedad
  **PATCH** `/api/properties/{id}/price`  
    Cambia el precio de una propiedad por su **ID**.  
  
  ---
  
  #### Agregar imagen a propiedad
  **POST** `/api/properties/{id}/images`  
    Agrega una imagen a la propiedad especificada por su **ID**.  
 
  
  ---
  
  #### Buscar propiedades
  **GET** `/api/properties`  
    Busca propiedades aplicando filtros opcionales.  
 
      
