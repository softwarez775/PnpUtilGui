using System.ComponentModel;
using System.Windows.Forms;

namespace PnpUtilGui.Models
{
    internal class Driver : INotifyPropertyChanged
    {
        private bool _checked;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Checked
        {
            get { return _checked; }

            set
            {
                if (_checked == value)
                {
                    return;
                }

                _checked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Checked)));
            }
        }

        public string FileName { get; set; }

        public string SourceName { get; set; }

        public string Publisher { get; set; }

        public string DriverClass { get; set; }

        public string ClassGuid { get; set; }

        [Browsable(false)]
        public string ClassVersion { get; set; }

        public string DateAndVersion { get; set; }

        public string CertificateSignerName { get; set; }
    }
}
