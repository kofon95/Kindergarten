-- Script Date: 20.11.2016 18:16  - ErikEJ.SqlCeScripting version 3.5.2.64
-- Database information:
-- Locale Identifier: 1049
-- Encryption Mode: 
-- Case Sensitive: False
-- Database: D:\Projects\DotNet\Release\Kindergarten\WpfApp\bin\Debug\KindergartenDb.sdf
-- ServerVersion: 4.0.8876.1
-- DatabaseSize: 384 KB
-- SpaceAvailable: 3,999 GB
-- Created: 19.11.2016 20:34

-- User Table information:
-- Number of tables: 12
-- __MigrationHistory: 15 row(s)
-- Children: 9 row(s)
-- EnterChildHistory: 15 row(s)
-- Expenses: 5 row(s)
-- Groups: 3 row(s)
-- Incomes: 1 row(s)
-- MonthlyPayments: 16 row(s)
-- ParentChild: 5 row(s)
-- Parents: 6 row(s)
-- People: 15 row(s)
-- RangePayments: 4 row(s)
-- Tarifs: 4 row(s)

CREATE TABLE [Tarifs] (
  [Id] int IDENTITY (56,1) NOT NULL
, [MonthlyPayment] float NOT NULL
, [AnnualPayment] float NOT NULL
, [Note] nvarchar(255) NOT NULL
);
GO
CREATE TABLE [People] (
  [Id] int IDENTITY (60,1) NOT NULL
, [FirstName] nvarchar(64) NOT NULL
, [LastName] nvarchar(64) NOT NULL
, [Patronymic] nvarchar(64) NULL
, [PhotoPath] nvarchar(255) NULL
);
GO
CREATE TABLE [Parents] (
  [Id] int NOT NULL
, [LocationAddress] nvarchar(128) NOT NULL
, [ResidenceAddress] nvarchar(128) NULL
, [WorkAddress] nvarchar(128) NULL
, [PassportSeries] nvarchar(10) NOT NULL
, [PassportIssuedBy] nvarchar(256) NOT NULL
, [PassportIssueDate] datetime NOT NULL
, [PhoneNumber] nvarchar(20) NULL
);
GO
CREATE TABLE [Incomes] (
  [Id] int IDENTITY (12,1) NOT NULL
, [PersonName] nvarchar(64) NULL
, [Money] float NOT NULL
, [Note] nvarchar(255) NULL
, [IncomeDate] datetime NOT NULL
);
GO
CREATE TABLE [Groups] (
  [Id] int IDENTITY (24,1) NOT NULL
, [Name] nvarchar(64) NOT NULL
, [GroupType] tinyint NOT NULL
, [CreatedDate] datetime NOT NULL
);
GO
CREATE TABLE [Expenses] (
  [Id] int IDENTITY (11,1) NOT NULL
, [ExpenseType] int NOT NULL
, [Money] float NOT NULL
, [ExpenseDate] datetime NOT NULL
, [Description] nvarchar(255) NULL
);
GO
CREATE TABLE [Children] (
  [Id] int NOT NULL
, [GroupId] int NOT NULL
, [LocationAddress] nvarchar(128) NOT NULL
, [BirthDate] datetime NOT NULL
, [Sex] tinyint NOT NULL
, [TarifId] int DEFAULT 0 NOT NULL
, [IsNobody] bit DEFAULT 0 NOT NULL
);
GO
CREATE TABLE [RangePayments] (
  [Id] int IDENTITY (43,1) NOT NULL
, [PaymentFrom] datetime NOT NULL
, [PaymentTo] datetime NOT NULL
, [ChildId] int NOT NULL
, [PaymentDate] datetime NOT NULL
, [Description] nvarchar(255) NULL
, [MoneyPaymentByTarif] float DEFAULT 0 NOT NULL
);
GO
CREATE TABLE [ParentChild] (
  [ChildId] int NOT NULL
, [ParentId] int NOT NULL
, [ParentType] tinyint NOT NULL
, [ParentTypeText] nvarchar(32) NULL
);
GO
CREATE TABLE [MonthlyPayments] (
  [Id] int IDENTITY (103,1) NOT NULL
, [ChildId] int NOT NULL
, [PaymentDate] datetime NOT NULL
, [PaidMoney] float NOT NULL
, [PayDayCount] int NULL
, [MonthDayCount] int NULL
, [DebtAfterPaying] float DEFAULT 0 NOT NULL
, [Description] nvarchar(255) NULL
, [MoneyPaymentByTarif] float DEFAULT 0 NOT NULL
);
GO
CREATE TABLE [EnterChildHistory] (
  [Id] int IDENTITY (61,1) NOT NULL
, [ExpulsionNote] nvarchar(64) NULL
, [ChildId] int NOT NULL
, [EnterDate] datetime NOT NULL
, [ExpulsionDate] datetime NULL
);
GO
ALTER TABLE [Tarifs] ADD CONSTRAINT [PK_dbo.Tarifs] PRIMARY KEY ([Id]);
GO
ALTER TABLE [People] ADD CONSTRAINT [PK_dbo.People] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Parents] ADD CONSTRAINT [PK_dbo.Parents] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Incomes] ADD CONSTRAINT [PK_dbo.Incomes] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Groups] ADD CONSTRAINT [PK_dbo.Groups] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Expenses] ADD CONSTRAINT [PK_dbo.Expenses] PRIMARY KEY ([Id]);
GO
ALTER TABLE [Children] ADD CONSTRAINT [PK_dbo.Children] PRIMARY KEY ([Id]);
GO
ALTER TABLE [RangePayments] ADD CONSTRAINT [PK_dbo.RangePayments] PRIMARY KEY ([Id]);
GO
ALTER TABLE [ParentChild] ADD CONSTRAINT [PK_dbo.ParentChild] PRIMARY KEY ([ChildId],[ParentId]);
GO
ALTER TABLE [MonthlyPayments] ADD CONSTRAINT [PK_dbo.MonthlyPayments] PRIMARY KEY ([Id]);
GO
ALTER TABLE [EnterChildHistory] ADD CONSTRAINT [PK_dbo.EnterChildHistory] PRIMARY KEY ([Id]);
GO
CREATE INDEX [IX_Id] ON [Parents] ([Id] ASC);
GO
CREATE INDEX [IX_GroupId] ON [Children] ([GroupId] ASC);
GO
CREATE INDEX [IX_Id] ON [Children] ([Id] ASC);
GO
CREATE INDEX [IX_TarifId] ON [Children] ([TarifId] ASC);
GO
CREATE INDEX [IX_ChildId] ON [RangePayments] ([ChildId] ASC);
GO
CREATE INDEX [IX_ChildId] ON [ParentChild] ([ChildId] ASC);
GO
CREATE INDEX [IX_ParentId] ON [ParentChild] ([ParentId] ASC);
GO
CREATE INDEX [IX_ChildId] ON [MonthlyPayments] ([ChildId] ASC);
GO
CREATE INDEX [IX_ChildId] ON [EnterChildHistory] ([ChildId] ASC);
GO
ALTER TABLE [Parents] ADD CONSTRAINT [FK_dbo.Parents_dbo.People_Id] FOREIGN KEY ([Id]) REFERENCES [People]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE [Children] ADD CONSTRAINT [FK_dbo.Children_dbo.Groups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [Groups]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE [Children] ADD CONSTRAINT [FK_dbo.Children_dbo.People_Id] FOREIGN KEY ([Id]) REFERENCES [People]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE [Children] ADD CONSTRAINT [FK_dbo.Children_dbo.Tarifs_TarifId] FOREIGN KEY ([TarifId]) REFERENCES [Tarifs]([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION;
GO
ALTER TABLE [RangePayments] ADD CONSTRAINT [FK_dbo.RangePayments_dbo.Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children]([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;
GO
ALTER TABLE [ParentChild] ADD CONSTRAINT [FK_dbo.ParentChild_dbo.Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children]([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;
GO
ALTER TABLE [ParentChild] ADD CONSTRAINT [FK_dbo.ParentChild_dbo.Parents_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Parents]([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;
GO
ALTER TABLE [MonthlyPayments] ADD CONSTRAINT [FK_dbo.Payments_dbo.Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children]([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;
GO
ALTER TABLE [EnterChildHistory] ADD CONSTRAINT [FK_dbo.EnterChildHistory_dbo.Children_ChildId] FOREIGN KEY ([ChildId]) REFERENCES [Children]([Id]) ON DELETE CASCADE ON UPDATE NO ACTION;
GO

