-- ==========================================
-- Base de Datos: Arquitectura de Microservicios
-- Microservicio 1: Clientes / Personas
-- Microservicio 2: Cuentas / Movimientos
-- Base de datos relacional: PostgreSQL
-- ==========================================

-- ==========================================
-- MICROSERVICIO 1: CLIENTES / PERSONAS
-- ==========================================

CREATE TABLE IF NOT EXISTS Personas (
    Id SERIAL PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Genero VARCHAR(20) NOT NULL,
    Edad INT NOT NULL,
    Identificacion VARCHAR(20) UNIQUE NOT NULL,
    Direccion VARCHAR(200) NOT NULL,
    Telefono VARCHAR(20) NOT NULL
);

CREATE TABLE IF NOT EXISTS Clientes (
    Id INT PRIMARY KEY,
    Contrasena VARCHAR(100) NOT NULL,
    Estado BOOLEAN NOT NULL DEFAULT TRUE,
    FOREIGN KEY (Id) REFERENCES Personas(Id) ON DELETE CASCADE
);

-- Datos de prueba: Creación de Clientes
INSERT INTO Personas (Nombre, Genero, Edad, Identificacion, Direccion, Telefono) VALUES
('Jose Lema', 'Masculino', 35, '1700000001', 'Otavalo sn y principal', '098254785'),
('Marianela Montalvo', 'Femenino', 32, '1700000002', 'Amazonas y NNUU', '097548965'),
('Juan Osorio', 'Masculino', 40, '1700000003', '13 Junio y Equinoccial', '098874587');

INSERT INTO Clientes (Id, Contrasena, Estado) VALUES
(1, '1234', TRUE),
(2, '5678', TRUE),
(3, '1245', TRUE);

-- ==========================================
-- MICROSERVICIO 2: CUENTAS / MOVIMIENTOS
-- ==========================================

-- Modelo de lectura de clientes (sincronizado vía eventos RabbitMQ)
CREATE TABLE IF NOT EXISTS ClientesSync (
    ClienteId INT PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Estado BOOLEAN NOT NULL DEFAULT TRUE
);

-- Datos iniciales de sincronización
INSERT INTO ClientesSync (ClienteId, Nombre, Estado) VALUES
(1, 'Jose Lema', TRUE),
(2, 'Marianela Montalvo', TRUE),
(3, 'Juan Osorio', TRUE)
ON CONFLICT (ClienteId) DO NOTHING;

CREATE TABLE IF NOT EXISTS Cuentas (
    Id SERIAL PRIMARY KEY,
    NumeroCuenta VARCHAR(20) UNIQUE NOT NULL,
    TipoCuenta VARCHAR(20) NOT NULL,
    SaldoInicial NUMERIC(12,2) NOT NULL DEFAULT 0.00,
    Estado BOOLEAN NOT NULL DEFAULT TRUE,
    ClienteId INT NOT NULL
);

CREATE TABLE IF NOT EXISTS Movimientos (
    MovimientoId SERIAL PRIMARY KEY,
    Fecha TIMESTAMPTZ NOT NULL,
    TipoMovimiento VARCHAR(50) NOT NULL,
    Valor NUMERIC(12,2) NOT NULL,
    Saldo NUMERIC(12,2) NOT NULL,
    CuentaId INT NOT NULL REFERENCES Cuentas(Id) ON DELETE CASCADE
);

-- Datos de prueba: Creación de Cuentas
INSERT INTO Cuentas (NumeroCuenta, TipoCuenta, SaldoInicial, Estado, ClienteId) VALUES
('478758', 'Ahorros',   2000, TRUE, 1),
('225487', 'Corriente',  100, TRUE, 2),
('495878', 'Ahorros',      0, TRUE, 3),
('496825', 'Ahorros',    540, TRUE, 2),
('585545', 'Corriente', 1000, TRUE, 1);

-- Movimientos a realizar (vía API) con saldo calculado:
-- Cuenta 478758 (Jose Lema):          Retiro de 575   -> Saldo 1425
-- Cuenta 225487 (Marianela Montalvo): Depósito de 600 -> Saldo 700
-- Cuenta 495878 (Juan Osorio):        Depósito de 150 -> Saldo 150
-- Cuenta 496825 (Marianela Montalvo): Retiro de 540   -> Saldo 0
