using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    internal class ProgressBarControl : IDisposable
    {
        private ProgressBar progressBar;
        private Label label;
        private int steps;

        private ProgressBarControl(ProgressBar progressBar, Label label, int steps)
        {
            this.progressBar = progressBar;
            this.steps = steps;
            this.label = label;
            progressBar.Minimum = 0;
            progressBar.Maximum = steps;
            progressBar.Value = 0;
            progressBar.Step = 1;
        }

        public static ProgressBarControl Start(ProgressBar progressBar, Label label, int steps)
        {
            return new ProgressBarControl(progressBar, label, steps);
        }

        public void Dispose()
        {
            progressBar.Value = 0;
            label.Text = "";
        }

        public void Increment(string text)
        {
            progressBar.PerformStep();
            label.Text = text + " (" + progressBar.Value + "/" + steps + ")";
        }
    }
}