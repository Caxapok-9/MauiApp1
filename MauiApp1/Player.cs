using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using SQLiteNetExtensions.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1
{
    public class Player : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int? Id { get; set; } = null;

        public string Name { get; set; }

        public string Number { get; set; }

        private bool _IsCaptain { get; set; }

        private bool _IsLibero { get; set; }

        [ForeignKey(typeof(Team))]
        public int TeamID { get; set; }

        [ForeignKey(typeof(Player))]
        public int ReplaceID { get; set; } = 0;

        private bool _IsSelected { get; set; }

        public string DisplayName => $"{Number} - {Name}";

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

        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (_IsSelected != value) // Меняем только если значение другое
                {
                    _IsSelected = value;
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
