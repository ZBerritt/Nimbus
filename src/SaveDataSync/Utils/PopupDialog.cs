using System.Windows.Forms;

namespace SaveDataSync.Utils
{
    internal class PopupDialog
    {
        private static void Popup(string message, string title, MessageBoxIcon icon)
        {
            MessageBox.Show(message,
                title,
                MessageBoxButtons.OK,
                icon);
        }

        private static DialogResult Prompt(string message, string title, MessageBoxIcon icon)
        {
            return MessageBox.Show(message,
                title,
                MessageBoxButtons.YesNo,
                icon);
        }
        public static void ErrorPopup(string message)
        {
            Popup(message, "Error", MessageBoxIcon.Error);
        }

        public static DialogResult ErrorPrompt(string message)
        {
            return Prompt(message, "Error", MessageBoxIcon.Error);
        }

        public static void WarningPopup(string message)
        {
            Popup(message, "Warning", MessageBoxIcon.Warning);
        }

        public static DialogResult WarningPrompt(string message)
        {
            return Prompt(message, "Warning", MessageBoxIcon.Warning);
        }

        public static void InfoPopup(string message)
        {
            Popup(message, "Info", MessageBoxIcon.Information);
        }

        public static DialogResult InfoPrompt(string message)
        {
            return Prompt(message, "Info", MessageBoxIcon.Information);
        }

        public static DialogResult QuestionPrompt(string message, string title)
        {
            return Prompt(message, title, MessageBoxIcon.Question);
        }
    }
}
