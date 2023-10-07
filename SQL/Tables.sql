CREATE TABLE [dbo].[Devices] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Token]        NVARCHAR (50) NOT NULL,
    [CreationTime] DATETIME2 (0) DEFAULT (sysutcdatetime()) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([Id] ASC),
    UNIQUE CLUSTERED ([Token] ASC)
);

CREATE TABLE [dbo].[Experiments] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50) NOT NULL,
    [CreationTime] DATETIME2 (0) DEFAULT (sysutcdatetime()) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC)
);

CREATE TABLE [dbo].[ExperimentsOptions] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [Value]        NVARCHAR (MAX)  NOT NULL,
    [Chance]       DECIMAL (10, 9) NOT NULL,
    [ExperimentId] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([ExperimentId]) REFERENCES [dbo].[Experiments] ([Id]) ON DELETE CASCADE,
    CHECK ([Chance]>(0) AND [Chance]<=(1))
);

CREATE TABLE [dbo].[DevicesExperimentsOptions] (
    [DeviceId]           INT NOT NULL,
    [ExperimentOptionId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([DeviceId] ASC, [ExperimentOptionId] ASC),
    FOREIGN KEY ([ExperimentOptionId]) REFERENCES [dbo].[ExperimentsOptions] ([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([DeviceId]) REFERENCES [dbo].[Devices] ([Id]) ON DELETE CASCADE
);

