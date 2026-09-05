# Prueba Técnica: Microservicios .NET

Este proyecto implementa una solución de microservicios en .NET 8 con Clean Architecture. Son dos servicios independientes que se comunican de forma asincrona a traves de RabbitMQ.

## Los dos microservicios

| Microservicio | Puerto | Que hace |
|---|---|---|
| cliente-api | 5001 | Administra Personas y Clientes (CRUD) y publica eventos |
| cuentamovimiento-api | 5002 | Administra Cuentas y Movimientos, aplica la regla de negocio y genera reportes |

## Tecnologia utilizada

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core con PostgreSQL (persistencia relacional)
- RabbitMQ (comunicacion asincrona entre servicios)
- xUnit, FluentAssertions y Moq (pruebas unitarias)
- WebApplicationFactory y EF Core InMemory (pruebas de integracion)
- Docker y Docker Compose (despliegue)

## Estructura del proyecto

La carpeta src contiene cada capa de la arquitectura y los tests.

- src/scripts/BaseDatos.sql: estructura de la base de datos de referencia (los datos se cargan solos al iniciar)
- src/ClienteService: el primer microservicio
  - Domain: entidades, excepciones e interfaces
  - Application: DTOs, servicios y casos de uso
  - Infrastructure: EF Core, repositorios y publicacion de eventos en RabbitMQ
  - API: controladores y Program.cs
  - Dockerfile
- src/CuentaMovimientoService: el segundo microservicio
  - Domain: entidades, excepciones e interfaces
  - Application: DTOs, servicios y casos de uso
  - Infrastructure: EF Core, consumo de eventos de RabbitMQ y reportes
  - API: controladores y Program.cs
  - Dockerfile
- src/tests: las pruebas
  - ClienteService.Tests: pruebas unitarias de la entidad Cliente
  - CuentaMovimientoService.Tests: pruebas unitarias del servicio de movimientos
  - IntegrationTests: pruebas de integracion del API

## Como se comunican los servicios

Los dos microservicios se comunican de forma asincrona a traves de RabbitMQ usando un exchange de tipo Topic, con el siguiente flujo:

1. Cuando se crea, actualiza o elimina un cliente, el cliente-api publica el evento en el exchange cliente.exchange (durable, tipo Topic) con una routing key segun el tipo: cliente.created, cliente.updated o cliente.deleted.
2. El exchange rutea el mensaje a la cola cliente.sync.queue usando el patron de enrutamiento cliente.*, que coincide con las tres routing keys.
3. El cuentamovimiento-api consume de esa cola y guarda el cliente en una tabla local de lectura (upsert por ClienteId). De esta forma el segundo servicio puede validar las cuentas y generar reportes por cliente sin depender de una llamada sincrona.

```
                cliente.created / cliente.updated / cliente.deleted
 ClienteService  -------------------------------------------------> [cliente.exchange]  (topic, durable)
                                                                          |
                                                                          | cliente.* (patron de enrutamiento)
                                                                          v
                                                                    [cliente.sync.queue]  (durable)
                                                                          |
                                                                          | consume
                                                                          v
                                                              CuentaMovimientoService
                                                                   (upsert en tabla Clientes)
```

Ejemplo con POST /api/clientes: el alta de un cliente genera el evento cliente.created con el cuerpo { "ClienteId": 1, "Nombre": "Jose Lema", "EventDate": "2026-09-04T..." }; el cuentamovimiento-api lo recibe y deja el cliente disponible en su tabla Clientes para que las cuentas puedan referenciarlo.

## Funcionalidades implementadas

### F1: CRUD de endpoints
- /api/clientes: crear (POST), leer (GET), actualizar (PUT/PATCH), eliminar (DELETE)
- /api/cuentas: crear (POST), leer (GET), actualizar (PUT/PATCH)
- /api/movimientos: crear (POST), leer (GET), actualizar (PUT/PATCH)

### F2: Registro de movimientos y saldo disponible
- Acepta valores positivos (depositos) y negativos (retiros)
- Al registrar un movimiento se recalcula el saldo disponible
- Se guarda el historial de cada transaccion

### F3: Regla de negocio y manejo de excepciones
- Si un movimiento deja el saldo en negativo, se lanza la excepcion con el mensaje "Saldo no disponible" y responde HTTP 400. Esta regla se gestiona con un manejo global de excepciones (ExceptionMiddleware).

### F4: Reporte de estado de cuenta
- GET /api/reportes?fechaInicio={dd/mm/yyyy}&fechaFin={dd/mm/yyyy}&cliente={cliente_id}
- Devuelve en JSON las cuentas asociadas, los saldos actualizados y los movimientos dentro del rango solicitado.

### F5: Pruebas unitarias
- ClienteService.Tests valida la entidad de dominio Cliente (ClienteTests.cs)
- CuentaMovimientoService.Tests valida la regla de negocio del servicio de movimientos (MovimientoServiceTests.cs)

### F6: Pruebas de integracion
- El proyecto IntegrationTests prueba el flujo completo API, controlador, servicio, repositorio y EF Core InMemory mediante WebApplicationFactory.

### F7: Despliegue en contenedores
- Dockerfiles multi-stage y docker-compose.yaml para levantar todo el sistema.

## Datos de prueba

Al levantar cada servicio, el codigo inserta los datos de prueba automaticamente la primera vez que inicia (y solo entonces, para no duplicarlos). Estos son los datos que se cargan solos:

### Clientes

| Nombre | Direccion | Telefono | Contrasena | Estado |
|---|---|---|---|---|
| Jose Lema | Otavalo sn y principal | 098254785 | 1234 | True |
| Marianela Montalvo | Amazonas y NNUU | 097548965 | 5678 | True |
| Juan Osorio | 13 Junio y Equinoccial | 098874587 | 1245 | True |

### Cuentas

| Numero | Tipo | Saldo Inicial | Estado | Cliente |
|---|---|---|---|---|
| 478758 | Ahorros | 2000 | True | Jose Lema |
| 225487 | Corriente | 100 | True | Marianela Montalvo |
| 495878 | Ahorros | 0 | True | Juan Osorio |
| 496825 | Ahorros | 540 | True | Marianela Montalvo |
| 585545 | Corriente | 1000 | True | Jose Lema |

### Movimientos a realizar

- Cuenta 478758 (Jose Lema): retiro de 575, saldo 1425
- Cuenta 225487 (Marianela Montalvo): deposito de 600, saldo 700
- Cuenta 495878 (Juan Osorio): deposito de 150, saldo 150
- Cuenta 496825 (Marianela Montalvo): retiro de 540, saldo 0

## Requisitos previos

Para levantar todo con Docker solo necesitas Docker Desktop. Si ademas quieres compilar y ejecutar los tests fuera de Docker, instala el .NET SDK 8.

## Levantar el sistema con Docker

Desde la raiz del proyecto ejecuta:

```
docker-compose up --build
```

Esto levanta dos bases PostgreSQL, RabbitMQ y los dos microservicios.

Los accesos (contrasenas y usuario de RabbitMQ) se definen con variables de entorno en un archivo .env de la raiz del proyecto, que Docker Compose lee automaticamente. Ya existe un .env con valores de desarrollo; si necesitas crear los tuyos, copia el ejemplo y ajusta los valores:

```
cp .env.example .env
```

Las claves de acceso no estan en el codigo .NET (no hay usuarios, contrasenas o cadenas de conexion en el codigo ni en appsettings.json). Cada servicio expone un singleton (ClientesDbConnection/CuentasDbConnection y RabbitMqConnection) que lee la configuracion una sola vez desde variables de entorno por componente (Host, Port, Database, Username, Password; y RabbitMQ__HostName/Port/UserName/Password), la conserva en memoria y abre la conexion sobre esa configuracion. Si falta algun valor, el servicio falla en el arranque con un error claro en lugar de usar un valor por defecto.

Direcciones disponibles:

- Clientes API: http://localhost:5001/swagger
- Cuentas y movimientos API: http://localhost:5002/swagger
- RabbitMQ Management: http://localhost:15672 (usuario guest, contrasena guest)

Las bases de datos se crean solas al iniciar los servicios usando EF Core, y los datos de prueba se insertan automaticamente la primera vez que arrancan. El script src/scripts/BaseDatos.sql queda solo como referencia de estructura y datos.

## Como correr los tests usando Docker

No hace falta tener el .NET SDK instalado en la maquina. Se puede ejecutar toda la bateria de pruebas dentro de un contenedor con la imagen del SDK.

El comando monta la carpeta actual dentro del contenedor, restaura los paquetes de NuGet y ejecuta dotnet test sobre el archivo de solucion (src/PruebaTecnica.sln). Se hace un restore explicito primero para que funcione igual aunque ya hayas compilado el proyecto antes en tu maquina. Es mejor usar una ruta con variables de entorno para que funcione igual en Windows y en Linux.

En Linux o macOS (bash):

```
docker run --rm -v "$PWD":/workspace -w /workspace mcr.microsoft.com/dotnet/sdk:8.0 sh -c "dotnet restore src/PruebaTecnica.sln && dotnet test src/PruebaTecnica.sln --no-restore"
```

En Windows (PowerShell):

```
docker run --rm -v ${PWD}:/workspace -w /workspace mcr.microsoft.com/dotnet/sdk:8.0 sh -c "dotnet restore src/PruebaTecnica.sln && dotnet test src/PruebaTecnica.sln --no-restore"
```

Si quieres ver el resultado de las pruebas de forma mas limpia, agrega al final del comando las opciones de xUnit, por ejemplo:

```
docker run --rm -v "$PWD":/workspace -w /workspace mcr.microsoft.com/dotnet/sdk:8.0 sh -c "dotnet restore src/PruebaTecnica.sln && dotnet test src/PruebaTecnica.sln --no-restore --logger console;verbosity=detailed"
```

Al terminar, el contenedor se borra solo (flag --rm) y no queda ningun archivo o proceso instalado en tu maquina.

## Compilar y ejecutar sin Docker

Si quieres correr los servicios directamente:

1. Levanta PostgreSQL y RabbitMQ con: docker-compose up db_clientes db_cuentas rabbitmq
2. La configuracion de conexion se define con variables de entorno por componentes: para la base ClientesDb y CuentasDb (Host, Port, Database, Username, Password) se usa el prefijo correspondiente (por ejemplo ClientesDb__Host, ClientesDb__Password) y para RabbitMQ las claves RabbitMQ__HostName, RabbitMQ__Port, RabbitMQ__UserName, RabbitMQ__Password. No hay valores en el codigo ni en appsettings.json
3. Ejecuta cada API:

```
dotnet run --project src/ClienteService/API
dotnet run --project src/CuentaMovimientoService/API
```

Para correr los tests sin Docker, desde la carpeta src:

```
dotnet test PruebaTecnica.sln
```

Los proyectos de test usan xUnit v3 con Microsoft.Testing.Platform. Para que dotnet test funcione, los proyectos de test declaran OutputType Exe y TestingPlatformDotnetTestSupport, y el global.json de la raiz activa el corredor MTP en los SDK 10 en adelante. No hace falta cambiar nada para correr los tests.

## Endpoints principales

| Metodo | URL | Microservicio | Descripcion |
|---|---|---|---|
| POST | /api/clientes | cliente-api | Crear cliente |
| GET | /api/clientes | cliente-api | Listar clientes |
| GET | /api/clientes/{id} | cliente-api | Obtener cliente |
| PUT/PATCH | /api/clientes/{id} | cliente-api | Actualizar cliente |
| DELETE | /api/clientes/{id} | cliente-api | Eliminar cliente |
| POST | /api/cuentas | cuentamovimiento-api | Crear cuenta |
| GET | /api/cuentas | cuentamovimiento-api | Listar cuentas |
| GET | /api/cuentas/{id} | cuentamovimiento-api | Obtener cuenta |
| PUT/PATCH | /api/cuentas/{numeroCuenta} | cuentamovimiento-api | Actualizar cuenta |
| POST | /api/movimientos | cuentamovimiento-api | Registrar movimiento |
| GET | /api/movimientos | cuentamovimiento-api | Listar movimientos |
| GET | /api/movimientos/cuenta/{numeroCuenta} | cuentamovimiento-api | Listar movimientos de una cuenta |
| GET | /api/reportes | cuentamovimiento-api | Reporte de estado de cuenta |

## Ejemplos de respuestas

### Crear un movimiento (deposito)

POST /api/movimientos

```
{ "numeroCuenta": "225487", "valor": 600, "tipoMovimiento": "Deposito" }
```

Responde con saldo 700.

### Saldo no disponible

POST /api/movimientos

```
{ "numeroCuenta": "478758", "valor": -5000, "tipoMovimiento": "Retiro" }
```

Responde 400 Bad Request.

```
{ "mensaje": "Saldo no disponible" }
```

### Cuenta duplicada

POST /api/cuentas con un numeroCuenta que ya existe.

```
{ "clienteId": 1, "numeroCuenta": "478758", "tipoCuenta": "Ahorros", "saldoInicial": 100, "estado": true }
```

Responde 409 Conflict.

```
{ "mensaje": "Ya existe una cuenta con el número 478758." }
```

### Reporte

GET /api/reportes?fechaInicio=01/01/2022&fechaFin=31/12/2022&cliente=2

```
[
  {
    "fecha": "10/2/2022",
    "cliente": "Marianela Montalvo",
    "numeroCuenta": "225487",
    "tipo": "Corriente",
    "saldoInicial": 100,
    "estado": true,
    "movimiento": 600,
    "saldoDisponible": 700
  }
]
```

El rango es inclusivo y abarca todo el día de fechaFin: un movimiento del 15/02/2022 se incluye aunque la consulta use fechaFin=15/02/2022. Si se invierte el rango (fechaFin anterior a fechaInicio) o faltan las fechas, responde 400 Bad Request con un mensaje descriptivo.

## Patrones y buenas practicas

- Clean Architecture: capas Domain, Application, Infrastructure y API con dependencias hacia adentro.
- Repository Pattern: el acceso a datos queda aislado en la capa de infraestructura.
- DTOs: separan el API de las entidades de dominio.
- Manejo global de excepciones: un middleware centralizado convierte las excepciones de dominio en respuestas HTTP (404, 400 y 500).
- Inyeccion de dependencias y publicacion/suscripcion de eventos.

## Entregables

- src/scripts/BaseDatos.sql: script de referencia con la estructura de la base de datos.
- Coleccion Postman: PruebaTecnica.postman_collection.json en la raiz.
- docker-compose.yaml y Dockerfiles: despliegue en contenedores.
- src/PruebaTecnica.sln: solucion .NET completa.
