IF COL_LENGTH(N'Auth.User', N'DarkModeEnabled') IS NULL
BEGIN
    ALTER TABLE [Auth].[User]
    ADD [DarkModeEnabled] bit NOT NULL
        CONSTRAINT [DF_User_DarkModeEnabled] DEFAULT (0);
END;
