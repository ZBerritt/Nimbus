using System;

namespace SaveDataSync
{
    public class InvalidSaveException : Exception
    {
        public InvalidSaveException(string message) : base($"Invalid save: {message}")
        {
        }
    }

    public class SaveTooLargeException : InvalidSaveException
    {
        public SaveTooLargeException() : base("Save file is too large!")
        {
        }
    }

    public class ImportException : Exception
    {
        public ImportException(string message) : base($"Import error: {message}")
        {
        }
    }

    public class ExportException : Exception
    {
        public ExportException(string message) : base($"Export error: {message}")
        {
        }
    }

    public class LoadException : Exception
    {
        public LoadException(string message) : base($"Data Loading Error: {message}")
        {

        }
    }
}