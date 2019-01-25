using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivitiesVisualizer.ViewModels
{
    public class ErrorViewModel
    {
        public string Message { get; private set; }

        public ErrorType Type { get; private set; }

        public ErrorPositionInfo Position { get; private set; }

        public ErrorViewModel(ErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        public ErrorViewModel(ErrorType type, string message, ErrorPositionInfo position)
        {
            Type = type;
            Message = message;
            Position = position;
        }

        public bool Equals(ErrorViewModel other)
        {
            return Message.Equals(other.Message)
                && Type == other.Type
                && ((Position == null && other.Position == null) || (Position != null && other.Position != null && Position.Equals(other.Position)));
        }
    }

    public enum ErrorType
    {
        Error, ErrorButRenderAllowed, Warning
    }

    public sealed class ErrorPositionInfo
    {
        public int LineNumber { get; set; }

        internal static readonly ErrorPositionInfo Default = new ErrorPositionInfo()
        {
            LineNumber = 0
        };

        public bool Equals(ErrorPositionInfo other)
        {
            return LineNumber == other.LineNumber;
        }
    }
}
