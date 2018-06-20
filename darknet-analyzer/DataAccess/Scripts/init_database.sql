IF TYPE_ID(N'PacketSummaryInsert') IS NOT NULL
  DROP TYPE dbo.PacketSummaryInsert;

CREATE TYPE dbo.PacketSummaryInsert AS TABLE
(
    ReceivedDateTime DATETIME,
    Bytes INT,
    Protocol INT,
    SourceIp VARCHAR(255),
    SourcePort VARCHAR(10),
    DestinationIp VARCHAR(255),
    DestinationPort VARCHAR(10),
    FileId INT
)

IF TYPE_ID(N'ProbeInformationInsert') IS NOT NULL 
  DROP TYPE dbo.ProbeInformationInsert; 

CREATE TYPE dbo.ProbeInformationInsert AS TABLE
(
    SourceIp VARCHAR(255),
    NumTargetIps INT,
    NumTargetPorts INT,
    TotalBytes INT,
    TotalPackets INT,
    StartDateTime DATETIME,
    EndDateTime DATETIME
)

IF OBJECT_ID('dbo.PacketSummary', 'U') IS NOT NULL
  DROP TABLE dbo.PacketSummary;

CREATE TABLE dbo.PacketSummary
(
    ReceivedDateTime DATETIME,
    Bytes INT,
    Protocol INT,
    SourceIp VARCHAR(255),
    SourcePort VARCHAR(10),
    DestinationIp VARCHAR(255),
    DestinationPort VARCHAR(10),
    FileId INT
)

IF OBJECT_ID('dbo.ProbeInformation', 'U') IS NOT NULL
  DROP TABLE dbo.ProbeInformation;

CREATE TABLE dbo.ProbeInformation
(
    SourceIp VARCHAR(255) PRIMARY KEY,
    NumTargetIps INT,
    NumTargetPorts INT,
    TotalBytes INT,
    TotalPackets INT,
    StartDateTime DATETIME,
    EndDateTime DATETIME
)

IF OBJECT_ID('dbo.PcapFile', 'U') IS NOT NULL
  DROP TABLE dbo.PcapFile;

CREATE TABLE dbo.PcapFile
(
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    FilePath VARCHAR(500) UNIQUE,
    Analyzed BIT DEFAULT 0
)

ALTER TABLE dbo.PacketSummary ADD FOREIGN KEY (FileId) REFERENCES PcapFile (Id);
CREATE CLUSTERED INDEX idx_packetsummary_sourceip ON dbo.PacketSummary (SourceIp);

GO
IF OBJECT_ID('GetProbeInformationBatch', 'P') IS NOT NULL
    DROP PROCEDURE GetProbeInformationBatch

GO
CREATE PROCEDURE GetProbeInformationBatch 
    @BatchNum INT,
    @BatchSize INT
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    ;WITH ProbePage AS (
        SELECT DISTINCT PageSourceIp = SourceIp
        FROM dbo.PacketSummary s
        INNER JOIN dbo.PcapFile f ON s.FileId = f.Id
        WHERE f.Analyzed <> 1
        ORDER BY SourceIp
        OFFSET (@BatchNum * @BatchSize) ROWS 
        FETCH NEXT @BatchSize ROWS ONLY
    )
    
    SELECT
        SourceIp,
        NumTargetIps = COUNT(DISTINCT DestinationIp),
        NumTargetPorts = COUNT(DISTINCT DestinationPort),
        TotalBytes = SUM(BYTES),
        TotalPackets = COUNT(*),
        StartDateTime = MIN(ReceivedDateTime),
        EndDateTime = MAX(ReceivedDateTime)
    FROM dbo.PacketSummary
    INNER JOIN ProbePage ON SourceIp = PageSourceIp
    GROUP BY SourceIp
    ORDER BY SourceIp
END
GO
