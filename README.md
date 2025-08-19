# Dealership API

## How to Run
1. Make sure you have **.NET 9 SDK** installed.
2. Run the project:
   ```bash
   dotnet run --project Dealership
   ```
3. Open [http://localhost:5000/swagger/index.html](http://localhost:5026/swagger/index.html) to explore and test the API.

---

## Available Endpoints

### Auth
- `POST /auth/register` -> Register a new user  with OTP  
- `POST /auth/login` -> Login with OTP  
- `POST /auth/logout` -> Logout the current user  
- `GET /auth/me` -> Retrieve logged-in user info  
When registering a new user, you must provide a **role** to define their permissions in the system.
1. Customer
2. Admin 

### AdminUser
- `GET /api/customers` => List all customers  
- `GET /api/vehicle/getAll` -> List all vehicles  
- `GET /api/vehicle/getBy/{id}` -> Get vehicle by ID  
- `POST /api/vehicle/create` -> Create a new vehicle   
- `PUT /api/vehicle/update` -> Update an existing vehicle with OTP
- `DELETE /api/vehicle/deleteBy/{id}` -> Delete a vehicle  

- `GET /api/allPurchases` -> Retrieve all purchases  
- `GET /api/purchaseById/{id}` -> Get a purchase by ID  
- `POST /api/purchase/{id}/approve` -> Approve a purchase request  
- `POST /api/purchase/{id}/reject` -> Reject a purchase request  

### CustomerUser
- `GET /api/CustomerUser/vehicle/getAll` -> Browse available vehicles  
- `GET /api/CustomerUser/vehicle/filtering` -> Filter vehicles by parameters  
- `GET /api/CustomerUser/vehicle/details/byNameOrMade/{nameOrMade}` -> Search vehicles by name or make  
- `POST /api/CustomerUser/purchaseRequest/{vehicleId}` -> Submit purchase request  
- `GET /api/CustomerUser/myPurchase` -> Retrieve customer's purchases  

### Otp
- The OTP (One-Time Password) system is used to secure sensitive operations.
- `POST /api/otp/request` -> Request OTP used for secure actions: "Login" | "Register" | "PurchaseRequest" | "UpdateVehicle"
Parameters:
- `userName` - The username of the requesting user.
- `purpose` - The action requiring OTP. Must be one of the following values:
"Login"
"Register"
"PurchaseRequest"
"UpdateVehicle"
---

## Assumptions & Design Decisions
- **Role-based separation**:
  - `AdminUser` manages vehicles and purchase approvals.
  - `CustomerUser` can browse vehicles and request purchases.
  - `Auth` handles authentication and session control.
- **OTP layer** for sensitive operations ("Login" | "Register" | "PurchaseRequest" | "UpdateVehicle").

- **Data store** is JSON-based (no external database required).
- **Swagger UI** is the main documentation and testing tool.

---
