CREATE PROCEDURE [dbo].[GetOptionValueByDeviceIdExperimentId]
	@device_id int,
	@experiment_id int
AS
	SELECT TOP 1 ExperimentsOptions.[Value] as 'Value'
	FROM DevicesExperimentsOptions
	JOIN ExperimentsOptions on ExperimentsOptions.Id = DevicesExperimentsOptions.ExperimentOptionId
	WHERE DevicesExperimentsOptions.DeviceId = @device_id AND ExperimentsOptions.ExperimentId = @experiment_id
	
	
CREATE PROCEDURE [dbo].[GetOptionsWithDeviceCount]
AS
	SELECT ExperimentsOptions.Id, ExperimentsOptions.[Value], ExperimentsOptions.Chance, ExperimentsOptions.ExperimentId, COUNT(DevicesExperimentsOptions.DeviceId) as DeviceCount
	FROM ExperimentsOptions
	LEFT JOIN DevicesExperimentsOptions ON DevicesExperimentsOptions.ExperimentOptionId = ExperimentsOptions.Id
	GROUP BY ExperimentsOptions.Id, ExperimentsOptions.[Value], ExperimentsOptions.Chance, ExperimentsOptions.ExperimentId
	
	
CREATE PROCEDURE [dbo].[GetOptionsByExperimentId]
	@experiment_id int
AS
	SELECT Id, [Value], Chance
	FROM ExperimentsOptions
	WHERE ExperimentId = @experiment_id
	
	
CREATE PROCEDURE [dbo].[GetDeviceByToken]
	@token nvarchar(50)
AS
	SELECT TOP 1 Id, CreationTime
	FROM Devices
	WHERE Devices.Token = @token
	
	
CREATE PROCEDURE [dbo].[GetAllExperiments]
AS
	SELECT Id, [Name], CreationTime
	FROM Experiments
	
	
CREATE PROCEDURE [dbo].[CreateDeviceExperimentOption]
	@device_id int,
	@option_id int
AS
	INSERT INTO DevicesExperimentsOptions(DeviceId, ExperimentOptionId) VALUES(@device_id, @option_id)
	
	
CREATE PROCEDURE [dbo].[CreateDevice]
	@token nvarchar(50),
	@creation_time DateTime2(0) OUTPUT
AS
	SET @creation_time = sysutcdatetime()
	INSERT INTO Devices(Token, CreationTime) VALUES(@token, @creation_time)
RETURN @@IDENTITY