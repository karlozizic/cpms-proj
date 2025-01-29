CREATE TABLE ChargePoint (
    ChargePointId VARCHAR(100) PRIMARY KEY,
    Name VARCHAR(100),
    Comment VARCHAR(200),
    Username VARCHAR(50),
    Password VARCHAR(50),
    ClientCertThumb VARCHAR(100)
);

CREATE TABLE ChargeTags (
    TagId VARCHAR(50) PRIMARY KEY,
    TagName VARCHAR(200),
    ParentTagId VARCHAR(50),
    ExpiryDate TIMESTAMP,
    Blocked BOOLEAN
);

CREATE TABLE MessageLog (
    LogId SERIAL PRIMARY KEY,
    LogTime TIMESTAMP NOT NULL,
    ChargePointId VARCHAR(100) NOT NULL,
    ConnectorId INTEGER,
    Message VARCHAR(100) NOT NULL,
    Result TEXT,
    ErrorCode VARCHAR(100)
);

CREATE TABLE Transactions (
    TransactionId SERIAL PRIMARY KEY,
    Uid VARCHAR(50),
    ChargePointId VARCHAR(100) NOT NULL,
    ConnectorId INTEGER NOT NULL,
    StartTagId VARCHAR(50),
    StartTime TIMESTAMP NOT NULL,
    MeterStart DOUBLE PRECISION NOT NULL,
    StartResult VARCHAR(100),
    StopTagId VARCHAR(50),
    StopTime TIMESTAMP,
    MeterStop DOUBLE PRECISION,
    StopReason VARCHAR(100)
);

CREATE TABLE ConnectorStatus (
    ChargePointId VARCHAR(100) NOT NULL,
    ConnectorId INTEGER NOT NULL,
    ConnectorName VARCHAR(100),
    LastStatus VARCHAR(100),
    LastStatusTime TIMESTAMP,
    LastMeter DOUBLE PRECISION,
    LastMeterTime TIMESTAMP,
    PRIMARY KEY (ChargePointId, ConnectorId)
);

CREATE VIEW ConnectorStatusView AS
SELECT cs.ChargePointId, cs.ConnectorId, cs.ConnectorName, cs.LastStatus, cs.LastStatusTime, 
       cs.LastMeter, cs.LastMeterTime, t.TransactionId, t.StartTagId, t.StartTime, 
       t.MeterStart, t.StartResult, t.StopTagId, t.StopTime, t.MeterStop, t.StopReason
FROM ConnectorStatus cs
LEFT JOIN Transactions t 
    ON t.ChargePointId = cs.ChargePointId AND t.ConnectorId = cs.ConnectorId
WHERE t.TransactionId IS NULL OR 
      t.TransactionId IN (
          SELECT MAX(TransactionId) 
          FROM Transactions
          GROUP BY ChargePointId, ConnectorId
      );

CREATE UNIQUE INDEX ChargePoint_Identifier ON ChargePoint(ChargePointId);

CREATE INDEX IX_MessageLog_ChargePointId ON MessageLog(LogTime);

ALTER TABLE Transactions ADD CONSTRAINT FK_Transactions_ChargePoint 
    FOREIGN KEY (ChargePointId) REFERENCES ChargePoint(ChargePointId);

ALTER TABLE Transactions ADD CONSTRAINT FK_Transactions_Transactions 
    FOREIGN KEY (TransactionId) REFERENCES Transactions(TransactionId);

CREATE INDEX IX_Transactions_ChargePointId_ConnectorId 
    ON Transactions(ChargePointId, ConnectorId);