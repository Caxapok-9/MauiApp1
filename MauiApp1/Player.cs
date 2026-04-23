using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class Player : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public bool _IsCaptain { get; set; }
        public bool _IsLibero { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Player()
        {
            Number = "";
            Name = "";
            _IsCaptain = false;
            _IsLibero = false;
        }

        public bool IsCaptain
        {
            get => _IsCaptain;
            set
            {
                if (_IsCaptain != value) // Меняем только если значение другое
                {
                    _IsCaptain = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }

        public bool IsLibero
        {
            get => _IsLibero;
            set
            {
                if (_IsLibero != value) // Меняем только если значение другое
                {
                    _IsLibero = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            }
        }
    }
}
